using API.Chargebee.Common;
using API.Chargebee.Models;
using API.Chargebee.Services.Customers;
using API.Chargebee.Services.PromotionalCredits;
using API.Chargebee.Services.Subscriptions;
using ChargeBee.Exceptions;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.Entities.GeneralModule;
using Domain.Entities.SubscriptionModule;
using Domain.Entities.UsersModule;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Domain.IContracts.IServices;
using MediatR;
using SubscriptionState = Domain.Entities.GeneralModule.SubscriptionState;

namespace Application.Modules.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ICustomerService _customerService;
    private readonly IPromotionalCreditService _promotionalCredit;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterUserCommandHandler(IUnitOfWork unitOfWork,
                                      IChargeBeeService chargeBeeService,
                                      ISubscriptionService subscriptionService,
                                      IPromotionalCreditService promotionalCredit,
                                      ICustomerService customerService)
    {
        _unitOfWork = unitOfWork;
        _chargeBeeService = chargeBeeService;
        _promotionalCredit = promotionalCredit;
        _subscriptionService = subscriptionService;
        _customerService = customerService;
    }

    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (await _unitOfWork.Users.ExistsAsync(x => x.Email.ToLower().Equals(request.Email.ToLower())))
            {
                throw new ClientException("A Customer already exist with " + request.Email + ". Please try a different email address.", System.Net.HttpStatusCode.BadRequest);
            }

            var chargebeeCustomer = _customerService.GetByEmail(request.Email);
            if (chargebeeCustomer is not null)
            {
                throw new ClientException("A Customer already exist with " + request.Email + ". Please try a different email address.", System.Net.HttpStatusCode.BadRequest);
            }

            var schedule = await _unitOfWork.PostCodeSchedules.GetFirstOrDefaultAsync(x => x.fk_DayID == request.fk_DayID &&
                                                                                           x.fk_TimeSlotID == request.fk_TimeSlotID &&
                                                                                           x.ServicePostalCode.PostalCode == request.PostCode,
                                                                                      x => x.Day,
                                                                                      x => x.Carrier,
                                                                                      x => x.TimeSlot);


            if (schedule is null)
            {
                throw new ClientException("Delivery Schedule Does not Exist", System.Net.HttpStatusCode.BadRequest);
            }

            #region Create source data from request

            var planVariation = await _unitOfWork.Plans.Variations.GetFirstOrDefaultAsync(x => x.fk_PlanID == request.PlanVariation.fk_PlanID &&
                                                                                               x.ItemsType == request.PlanVariation.ItemsType &&
                                                                                               x.IsOrganic == request.PlanVariation.IsOrganic &&
                                                                                               x.Frequency == request.PlanVariation.Frequency &&
                                                                                               !x.IsDeleted);
            var requestedPantryItemID = request.PantryItems.Select(x => new { x.ID, x.Frequency }).ToArray();
            var pantryItemVariations = await _unitOfWork.PantryItems.Variations.GetWhereAsync(x => requestedPantryItemID.Contains(new { ID = x.fk_PantryItemID, x.Frequency }) && !x.IsDeleted, x => x.PantryItem);

            var daysToAdd = request.PlanVariation.Frequency == ShippingFrequency.Fortnightly ? 14 : 7;
            var NextDeliveryDate = request.DeliveryDate.AddDays(daysToAdd);

            string pantryItemNotes = string.Empty;
            var addons = request.PantryItems.Select(x =>
            {
                var pantryItemVariation = pantryItemVariations.FirstOrDefault(y => y.ID == x.ID);
                if (!string.IsNullOrEmpty(x.Notes))
                {
                    pantryItemNotes = $"{pantryItemNotes}- {pantryItemVariation.PantryItem.Name} Notes : {x.Notes}\n";
                }
                return new ChargeBeeLineItem(pantryItemVariation.ChargeBeeID, x.Qty, LineItemType.Addon);
            }).ToList();
            var deliverycharges = await _chargeBeeService.GetDeliveryChargesByPostCodeAndFrequencyAsync(request.PostCode, request.PlanVariation.Frequency);
            addons.Add(new ChargeBeeLineItem(deliverycharges, 1, LineItemType.Addon));

            #endregion

            #region Create Subscription on Chargebee

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
                Billing = new ChargeBeeAddress()
                {
                    FirstName = request.Billing.FirstName,
                    LastName = request.Billing.LastName,
                    Email = request.Billing.Email,
                    PhoneNumber = request.Billing.PhoneNumber,
                    ExtendedAddr = request.Billing.Line1,
                    ExtendedAddr2 = request.Billing.Line2,
                    City = request.Billing.City,
                    State = request.Billing.State,
                    Zip = request.Billing.PostCode,
                    Country = "AU",
                },
                Shipping = new ChargeBeeAddress()
                {
                    FirstName = request.Shipping.FirstName,
                    LastName = request.Shipping.LastName,
                    Email = request.Shipping.Email,
                    PhoneNumber = request.Shipping.PhoneNumber,
                    ExtendedAddr = request.Shipping.Line1,
                    ExtendedAddr2 = request.Shipping.Line2,
                    City = request.Shipping.City,
                    State = request.Shipping.State,
                    Zip = request.Shipping.PostCode,
                    Country = "AU",
                },
                CustomFields = new KeyValuePair<string, object>[]
                {
                    new KeyValuePair<string, object>("cf_is_pause_subscription", false),
                    new KeyValuePair<string, object>("cf_pantry_item_notes", pantryItemNotes),
                    new KeyValuePair<string, object>("cf_first_delivery_date", request.DeliveryDate.ToString()),
                    new KeyValuePair<string, object>("cf_delivery_instructions", request.DeliveryInstructions ?? string.Empty),
                    new KeyValuePair<string, object>("cf_unwanted_products", request.UnwantedProducts ?? string.Empty),
                    new KeyValuePair<string, object>("cf_current_delivery_date", request.DeliveryDate.ToString()),
                    new KeyValuePair<string, object>("cf_delivery_day", schedule?.Day?.Value ?? string.Empty),
                    new KeyValuePair<string, object>("cf_delivery_time_slot", schedule?.TimeSlot?.Value ?? string.Empty),
                    new KeyValuePair<string, object>("cf_delivery_carrier", schedule?.Carrier?.Value ?? string.Empty),
                    new KeyValuePair<string, object>("cf_next_plan_id", planVariation.ChargeBeeID),
                    new KeyValuePair<string, object>("cf_next_plan_name", planVariation.ChargeBeeID ?? string.Empty),
                    new KeyValuePair<string, object>("cf_next_delivery_instructions", string.Empty),
                    new KeyValuePair<string, object>("cf_next_unwanted_products", string.Empty),
                    new KeyValuePair<string, object>("cf_next_delivery_date", NextDeliveryDate.ToString()),
                    new KeyValuePair<string, object>("cf_next_delivery_day", schedule?.Day?.Value ?? string.Empty),
                    new KeyValuePair<string, object>("cf_next_delivery_time_slot", schedule?.TimeSlot?.Value ?? string.Empty),
                    new KeyValuePair<string, object>("cf_next_delivery_carrier", schedule?.Carrier?.Value ?? string.Empty),
                }
            };

            if (!string.IsNullOrEmpty(request.Coupon))
            {
                createSubscription.Coupons = new List<string>()
                {
                    request.Coupon,
                };
            }

            var subscription = _subscriptionService.Create(createSubscription);

            #endregion

            #region Calculate Discount and apply balance as promotional credits

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

            #region Update Billing Date

            if (subscription.CurrentTermEnd != null)
            {
                _chargeBeeService.ChangeSubscriptionTermEndForFirstDeliveryDateTuesdayFridayLogic(request.DeliveryDate, subscription.ID, request.PlanVariation.Frequency == ShippingFrequency.Fortnightly);
            }

            #endregion

            #region Create User in DB

            var user = new User
            {
                FirstName = request?.FirstName,
                LastName = request?.LastName,
                Email = request?.Email,
                PhoneNumber = request?.PhoneNumber,
                ChargeBeeCustomerID = subscription.CustomerID,
                Subscriptions = new List<UserSubscription>()
                {
                    new UserSubscription()
                    {
                        Status = SubscriptionState.Active,
                        ChargeBeeID = subscription.ID,
                        Current = new SubscriptionSetting
                        {
                            TermStart = subscription.CurrentTermStart ?? DateTime.UtcNow,
                            TermEnd = subscription.CurrentTermEnd,
                            DeliveryDate = request.DeliveryDate,
                            UnwantedProducts = request.UnwantedProducts ?? string.Empty,
                            DeliveryInstructions = request.DeliveryInstructions ?? string.Empty,
                            Frequency = request.PlanVariation.Frequency,
                            fk_PlanVariationID = planVariation.ID,
                            fk_PostCodeScheduleID = schedule.ID,
                        },
                        Next = new SubscriptionSetting
                        {
                            TermStart = subscription.CurrentTermEnd ?? DateTime.UtcNow,
                            TermEnd = subscription.CurrentTermEnd?.AddDays(request.PlanVariation.Frequency == ShippingFrequency.Fortnightly ? 14 : 7),
                            DeliveryDate = NextDeliveryDate,
                            Frequency = request.PlanVariation.Frequency,
                            fk_PlanVariationID = planVariation.ID,
                            fk_PostCodeScheduleID = schedule.ID,
                        }
                    }
                },
                PantryItems = pantryItemVariations.Select(x => new UserPantryItem
                {
                    fk_PantryItemVariationID = x.ID,
                    Status = SubscriptionState.Active,
                    Qty = request.PantryItems.FirstOrDefault(y => y.ID == x.fk_PantryItemID).Qty,
                }).ToList(),
                IsOTPLogin = true,
                fk_RoleID = 2,
            };

            await _unitOfWork.Users.AddAsync(user);
            _unitOfWork.Complete();

            #endregion

            return true;
        }
        catch (Exception e)
        {
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
        }
    }
}
