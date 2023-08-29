using API.Chargebee.Abstractions;
using API.Chargebee.Common;
using API.Chargebee.Models;
using API.CIN7.Abstractions;
using API.CIN7.Models;
using API.FirstPromoter.Abstractions;
using API.FirstPromoter.Models.Promoter;
using API.Klaviyo.Service;
using Application.Common.Constants;
using Application.Common.Extensions;
using Application.Modules.Users.Models;
using AutoMapper;
using ChargeBee.Exceptions;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Entities.SubscriptionModule;
using Domain.Entities.UsersModule;
using MediatR;
using SubscriptionState = Domain.Entities.GeneralModule.SubscriptionState;

namespace Application.Modules.Users.Commands.RegisterUserPostCodeBasedSchedule;

public class RegisterUserPostCodeBasedScheduleCommandHandler : IRequestHandler<RegisterUserPostCodeBasedScheduleCommand, string>
{
    #region CONSTRUCTORS AND LOCALS

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationOptions _options;
    private readonly IKlaviyoService _klaviyoService;
    private readonly IContactService _contactService;
    private readonly IPromoterService _promoterService;
    private readonly ICustomerService _customerService;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPromotionalCreditService _promotionalCredit;
    public RegisterUserPostCodeBasedScheduleCommandHandler(IMapper mapper,
                                                         IUnitOfWork unitOfWork,
                                                         ApplicationOptions options,
                                                         IKlaviyoService klaviyoService,
                                                         IContactService contactService,
                                                         IPromoterService promoterService,
                                                         ICustomerService customerService,
                                                         IChargeBeeService chargeBeeService,
                                                         ISubscriptionService subscriptionService,
                                                         IPromotionalCreditService promotionalCredit)
    {
        _mapper = mapper;
        _options = options;
        _unitOfWork = unitOfWork;
        _contactService = contactService;
        _klaviyoService = klaviyoService;
        _customerService = customerService;
        _promoterService = promoterService;
        _chargeBeeService = chargeBeeService;
        _promotionalCredit = promotionalCredit;
        _subscriptionService = subscriptionService;
    }

    #endregion

    public async Task<string> Handle(RegisterUserPostCodeBasedScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            #region VALIDATIONS

            var aesNow = DateTime.UtcNow.UtcToTimeZone();
            var nearestValidDate = request.DeliveryDate.DayOfWeek.GetNearestDate(DateTime.UtcNow.AddDays(7));
            if (
                    request.fk_DayID == (int)DayOfWeek.Friday
                    &&
                    request.DeliveryDate.DayOfWeek == DayOfWeek.Friday
                    &&
                    request.DeliveryDate.Date == nearestValidDate // Start with a date at least 7 days after the current date
                    &&
                    aesNow.DayOfWeek == DayOfWeek.Friday
                    &&
                    aesNow.TimeOfDay >= new TimeSpan(22, 50, 0)
               )
            {
                throw new ClientException("Delivery date must be at least 7 days after today", System.Net.HttpStatusCode.BadRequest);
            }

            if (await _unitOfWork.Users.ExistsAsync(x => x.Email.ToLower().Equals(request.Email.ToLower())))
            {
                throw new ClientException("A Customer already exist with " + request.Email + ". Please try a different email address.", System.Net.HttpStatusCode.BadRequest);
            }

            var chargebeeCustomer = _customerService.GetByEmail(request.Email);
            if (chargebeeCustomer is not null)
            {
                throw new ClientException("A Customer already exist with " + request.Email + ". Please try a different email address.", System.Net.HttpStatusCode.BadRequest);
            }

            var postCode = request.Shipping.PostCode;
            var schedule = await _unitOfWork.PostCodes.DeliverySchedules.GetFirstOrDefaultAsync(x => x.fk_DayID == request.fk_DayID &&
                                                                                                     x.fk_TimeSlotID == request.fk_TimeSlotID &&
                                                                                                     x.PostCode.PostalCode == postCode,
                                                                                                x => x.Day,
                                                                                                x => x.Carrier,
                                                                                                x => x.TimeSlot,
                                                                                                x => x.PostCode) ?? throw new ClientException("Delivery Schedule Does not Exist", System.Net.HttpStatusCode.BadRequest);

            var planVariation = await _unitOfWork.Plans.Variations.GetFirstOrDefaultAsync(x => x.fk_PlanID == request.PlanVariation.fk_PlanID &&
                                                                                               x.Frequency == request.PlanVariation.Frequency &&
                                                                                               x.ItemsType == request.PlanVariation.ItemsType &&
                                                                                               x.IsOrganic == request.PlanVariation.IsOrganic &&
                                                                                               !x.IsDeleted) ?? throw new ClientException("Plan Does not Exist", System.Net.HttpStatusCode.BadRequest);

            #endregion

            #region REGISTRATION IN CHARGEBEE

            #region CREATE SOURCE DATA FROM REQUEST

            var produces = await _unitOfWork.Produces.GetWhereAsync(x => request.UnwantedProduces.Distinct().Contains(x.ID) || request.ReplacementProduces.Distinct().Contains(x.ID));
            var unwantedproduces = produces.FilterProduces(request.UnwantedProduces.Distinct());
            var replacementProduces = produces.FilterProduces(request.ReplacementProduces.Distinct());

            var requestedPantryItemIDs = request.PantryItems.Select(y => y.ID).ToArray();
            var pantryItemVariations = await _unitOfWork.PantryItems.Variations.GetWhereAsync(x => requestedPantryItemIDs.Contains(x.fk_PantryItemID) &&
                                                                                                   (

                                                                                                            request.PlanVariation.Frequency != ShippingFrequency.OneOff
                                                                                                            &&
                                                                                                            x.ShippingFrequency == request.PlanVariation.Frequency

                                                                                                        ||

                                                                                                            request.PlanVariation.Frequency == ShippingFrequency.OneOff
                                                                                                            &&
                                                                                                            x.Frequency == PantryItemFrequency.JustOnce

                                                                                                   )
                                                                                                   &&
                                                                                                   !x.IsDeleted,
                                                                                              x => x.PantryItem,
                                                                                              x => x.PantryItem.PantryItemStates);

            var state = schedule.PostCode.fk_StateID;
            if (pantryItemVariations != null && pantryItemVariations.Any() && pantryItemVariations.Any(x => !x.PantryItem.PantryItemStates.Select(c => c.fk_StateID).Contains(state)))
            {
                throw new ClientException("Some of the provided pantry items are not in service in given Post code.", System.Net.HttpStatusCode.NotFound, pantryItemVariations.Where(x => !x.PantryItem.PantryItemStates.Select(c => c.fk_StateID).Contains(state)));
            }

            var NextDeliveryDate = request.DeliveryDate.AddDays(request.PlanVariation.Frequency == ShippingFrequency.Fortnightly ? 14 : 7);

            string pantryItemNotes = string.Empty;
            var addons = new List<ChargeBeeLineItem>();
            if (request.PantryItems != null && request.PantryItems.Any())
            {
                if (pantryItemVariations is null)
                {
                    throw new ClientException("Invalid pantry items provided", System.Net.HttpStatusCode.BadRequest);
                }

                addons = request.PantryItems.Select(x =>
                {
                    var pantryItemVariation = pantryItemVariations.FirstOrDefault(y => y.fk_PantryItemID == x.ID && y.Frequency == x.Frequency);
                    if (!string.IsNullOrEmpty(x.Notes))
                    {
                        pantryItemNotes = $"{pantryItemNotes}- {pantryItemVariation.PantryItem.Name} Notes : {x.Notes}\n";
                    }
                    return new ChargeBeeLineItem(pantryItemVariation.ChargeBeeID, x.Qty, ItemEntityType.Addon);
                }).ToList();
            }

            var deliverycharges = await _chargeBeeService.GetPostCodeDeliveryChargesAsync(schedule.PostCode.fk_StateID, postCode, request.PlanVariation.Frequency);
            addons.Add(new ChargeBeeLineItem(deliverycharges, 1, ItemEntityType.Addon));

            var customFields = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("cf_is_pause_subscription", false),
                new KeyValuePair<string, object>("cf_pantry_item_notes", pantryItemNotes),
                new KeyValuePair<string, object>("cf_first_delivery_date", request.DeliveryDate.GetDateForChargebee()),
                new KeyValuePair<string, object>("cf_current_delivery_date", request.DeliveryDate.GetDateForChargebee()),
                new KeyValuePair<string, object>("cf_delivery_instructions", request.DeliveryInstructions ?? string.Empty),
                new KeyValuePair<string, object>("cf_delivery_day", schedule?.Day?.Value ?? string.Empty),
                new KeyValuePair<string, object>("cf_delivery_time_slot", schedule?.TimeSlot?.Value ?? string.Empty),
                new KeyValuePair<string, object>("cf_delivery_carrier", schedule?.Carrier?.Value ?? string.Empty),
                new KeyValuePair<string, object>("cf_next_plan_id", planVariation.ChargeBeeID),
                new KeyValuePair<string, object>("cf_next_plan_name", planVariation.InvoiceName ?? string.Empty),
                new KeyValuePair<string, object>("cf_next_delivery_instructions", request.DeliveryInstructions ?? string.Empty),
                new KeyValuePair<string, object>("cf_next_delivery_date", NextDeliveryDate.GetDateForChargebee()),
                new KeyValuePair<string, object>("cf_next_delivery_day", schedule?.Day?.Value ?? string.Empty),
                new KeyValuePair<string, object>("cf_next_delivery_time_slot", schedule?.TimeSlot?.Value ?? string.Empty),
                new KeyValuePair<string, object>("cf_next_delivery_carrier", schedule?.Carrier?.Value ?? string.Empty),
            };

            if (unwantedproduces != null && unwantedproduces.Any())
            {
                customFields.Add(new KeyValuePair<string, object>("cf_unwanted_products", unwantedproduces.GetProduces()));
            }

            if (replacementProduces != null && replacementProduces.Any())
            {
                customFields.Add(new KeyValuePair<string, object>("cf_replacement_products", replacementProduces.GetProduces()));
            }

            #endregion

            #region CREATE SUBSCRIPTION ON CHARGEBEE

            var createSubscription = new ChargeBeeSubscription()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                ChargebeeToken = request.ChargebeeToken,
                StartDate = DateTime.UtcNow.UtcToTimeZone(),
                PlanID = planVariation.ChargeBeeID,
                Addons = addons.ToArray(),
                Billing = _mapper.Map<ChargeBeeAddress>(request.Billing),
                Shipping = _mapper.Map<ChargeBeeAddress>(request.Shipping),
                CustomFields = customFields.ToArray(),
                Coupons = !string.IsNullOrEmpty(request.Coupon) ? new List<string>() { request.Coupon } : null
            };
            var subscription = _subscriptionService.Create(createSubscription) ?? throw new ClientException("Subscription creation failed", System.Net.HttpStatusCode.InternalServerError);

            #endregion

            #region CALCULATE DISCOUNT AND APPLY BALANCE AS PROMOTIONAL CREDITS

            var calculation = _chargeBeeService.CalculateDiscount(request.Coupon, request.TotalAmount);
            if (calculation.CanCouponBeApplied)
            {
                _promotionalCredit.Create(new ChargeBeePromotionalCredit
                {
                    CustomerID = subscription.CustomerID,
                    Amount = calculation.RemainingBalance,
                    Reference = "From_Gift_Cards",
                    Description = "Balance Left From Gift Card"
                });
            }

            #endregion

            #region UPDATE BILLING DATE

            if (subscription.CurrentTermEnd != null && request.PlanVariation.Frequency != ShippingFrequency.OneOff)
            {
                subscription.CurrentTermEnd = _chargeBeeService.ChangeSubscriptionTermEndForFirstDeliveryDateTuesdayFridayLogic(request.DeliveryDate, subscription.ID, request.PlanVariation.Frequency == ShippingFrequency.Fortnightly);
            }

            #endregion

            #endregion

            #region REGISTRATION IN CIN 7

            var cin7Contact = _mapper.Map<Contact>(createSubscription);
            cin7Contact = _contactService.AddCustomerContact(cin7Contact);

            #endregion

            #region REGISTRATION IN FIRST PROMOTER

            var promoter = _promoterService.Create(new CreatePromoter()
            {
                Email = request?.Email,
                FirstName = request?.FirstName,
                LastName = request?.LastName,
                CustomerID = subscription.CustomerID,
                CampaignID = "18912", // compaign name = Referral
                SkipEmailNotification = true,
            });

            #endregion

            #region REGISTRATION IN KLAVIYO

            _klaviyoService.SendMembersToKlaviyoList(KlaviyoListLegend.Promoters, new List<object>
            {
                new PromoterKlaviyoMember
                {
                    Email = request?.Email,
                    FirstName = request?.FirstName,
                    LastName = request?.LastName,
                    PhoneNumber = request?.PhoneNumber,
                    ChargeBeeCustomerID = subscription.CustomerID,
                    Cin7CustomerID =  cin7Contact.ID.ToString(),
                    FirstPromoterID = promoter.ID.ToString(),
                    FirstPromoterReferralID = promoter.DefaultRefID,
                    FirstPromoterReferralUrl = $"{_options.RegistrationUrl}?fp_ref={promoter.DefaultRefID}",
                }
            });

            #endregion

            #region REGISTRATION IN DB

            #region USER PANTRY ITEMS

            List<UserPantryItem> userPantryItems = null;
            if (request.PantryItems != null && request.PantryItems.Any())
            {
                if (pantryItemVariations is null)
                {
                    throw new ClientException("Invalid pantry items provided", System.Net.HttpStatusCode.BadRequest);
                }

                userPantryItems = request.PantryItems.Select(x =>
                {
                    var pantryItemVariation = pantryItemVariations.FirstOrDefault(y => y.fk_PantryItemID == x.ID && y.Frequency == x.Frequency);
                    var userPantryItem = new UserPantryItem()
                    {
                        Qty = x.Qty,
                        Status = request.PlanVariation.Frequency == ShippingFrequency.OneOff ? SubscriptionState.NonRenewing : SubscriptionState.Active,
                        RenewalDate = request.PlanVariation.Frequency == ShippingFrequency.OneOff ? null : pantryItemVariation.GetRenewalDate(subscription.CurrentTermEnd.Value),
                        fk_PantryItemVariationID = pantryItemVariation.ID,
                    };

                    return userPantryItem;
                }).ToList();
            }

            #endregion

            #region DB SUBSCRIPTION CONFIG/SETTING

            var dbSubscription = new UserSubscription()
            {
                BoxIndexToDeliver = 1,
                Status = SubscriptionState.NonRenewing,
                ChargeBeeID = subscription.ID,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Current = new SubscriptionSetting(DeliveryScheduleType.PostCodeDelivery, schedule.ID)
                {
                    DeliveryDate = request.DeliveryDate,
                    fk_PlanVariationID = planVariation.ID,
                    Frequency = request.PlanVariation.Frequency,
                    TermStart = subscription.CurrentTermStart ?? DateTime.UtcNow,
                    DeliveryInstructions = request.DeliveryInstructions ?? string.Empty,
                },
            };

            if (request.PlanVariation.Frequency != ShippingFrequency.OneOff)
            {
                dbSubscription.Status = SubscriptionState.Active;
                dbSubscription.Current.TermEnd = subscription.CurrentTermEnd;
                dbSubscription.Next = new SubscriptionSetting(DeliveryScheduleType.PostCodeDelivery, schedule.ID)
                {
                    DeliveryDate = NextDeliveryDate,
                    fk_PlanVariationID = planVariation.ID,
                    Frequency = request.PlanVariation.Frequency,
                    TermStart = subscription.CurrentTermEnd ?? DateTime.UtcNow,
                    DeliveryInstructions = request.DeliveryInstructions ?? string.Empty,
                    TermEnd = subscription.CurrentTermEnd?.AddDays(request.PlanVariation.Frequency == ShippingFrequency.Fortnightly ? 14 : 7),
                };
            }

            if (unwantedproduces != null && unwantedproduces.Any()) dbSubscription.UnwantedProduces = unwantedproduces.Select(x => new SubscriptionUnwantedProduce(x.ID)).ToList();
            if (replacementProduces != null && replacementProduces.Any()) dbSubscription.ReplacementProduces = replacementProduces.Select(x => new SubscriptionReplacementProduce(x.ID)).ToList();


            #endregion

            #region USER

            var user = new User()
            {
                Email = request?.Email,
                FirstName = request?.FirstName,
                LastName = request?.LastName,
                PhoneNumber = request?.PhoneNumber,

                ChargeBeeCustomerID = subscription.CustomerID,
                Cin7CustomerID = cin7Contact.ID.ToString(),
                FirstPromoterID = promoter.ID.ToString(),
                FirstPromoterAuthToken = promoter.AuthToken,
                FirstPromoterReferralID = promoter.DefaultRefID,

                PantryItems = userPantryItems,
                IsOTPLogin = true,
                fk_RoleID = 2,
                Subscriptions = new List<UserSubscription>() { dbSubscription }
            };

            #endregion

            await _unitOfWork.Users.AddAsync(user);
            _unitOfWork.Complete();

            #endregion

            return user.ChargeBeeCustomerID;
        }
        catch (Exception e)
        {
            #region EXCEPTION HANDLING

            if (e is InvalidRequestException iEx)
            {
                throw new ClientException(iEx.Message, System.Net.HttpStatusCode.BadRequest);
            }
            else if (e is PaymentException pEx)
            {
                throw new ClientException(pEx.Message, System.Net.HttpStatusCode.UnprocessableEntity);
            }
            else if (e is OperationFailedException ofEx)
            {
                throw new ClientException(ofEx.Message, System.Net.HttpStatusCode.NotModified);
            }
            else if (e is ArgumentException argEx)
            {
                throw new ClientException(argEx.Message, System.Net.HttpStatusCode.InternalServerError);
            }
            else if (e is ClientException)
            {
                throw;
            }
            else
            {
                throw new ClientException("Un able to Process", System.Net.HttpStatusCode.InternalServerError);
            }

            #endregion
        }
    }
}
