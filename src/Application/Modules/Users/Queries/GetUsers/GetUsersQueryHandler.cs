using API.Chargebee.Abstractions;
using API.Chargebee.Common;
using API.Chargebee.Models;
using API.Klaviyo.Models;
using AutoMapper;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Entities.UsersModule;
using Domain.Models.Pagination;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel;

namespace Application.Modules.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<GetUsersResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerService _customerService;
    private readonly ICurrentUserService _currentUserService;
    private readonly InfrastructureOptions _infrastructureOptions;
    private readonly DbContextOptions<ApplicationDbContext> _options;
    public GetUsersQueryHandler(InfrastructureOptions infrastructureOptions, DbContextOptions<ApplicationDbContext> options, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ICustomerService customerService)
    {
        _options = options;
        _unitOfWork = unitOfWork;
        _customerService = customerService;
        _currentUserService = currentUserService;
        _infrastructureOptions = infrastructureOptions;
    }
    public async Task<PaginatedList<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _unitOfWork.Users.GetUsersAsync<GetUsersResponse>(_currentUserService.ID, ((int)request.RoleFilter), request.FilterType, (request.Pagination ?? new Pagination()));
            //var users = await _unitOfWork.Users.GetUsersAsync<GetUsersResponse>(2, 2, new Pagination() { PageSize = -1 });

            //var userIDs = _unitOfWork.Users.ExecuteSqlQuery<GetUsersResponse>("SELECT ID FROM [dbo].[TempUser]").Select(x => x.ID);
            //var users = await _unitOfWork.Users.GetWhereNoTrackingAsync(x => !userIDs.Contains(x.ID) && x.ID > 2);

            //var totalFetchRecords = 0;
            //var chunkedList = new List<List<GetUsersResponse>>();
            //while (totalFetchRecords < users.Count())
            //{
            //    var chunkMatchRecords = users.Skip(totalFetchRecords).Take(500).ToList();
            //    totalFetchRecords += chunkMatchRecords.Count;
            //    chunkedList.Add(_mapper.Map<List<GetUsersResponse>>(chunkMatchRecords));
            //}

            //await Parallel.ForEachAsync(chunkedList, async (t,c) =>
            //{
            //    await UserFiltering(t);
            //});


            //var tasks = new List<Task>()
            //{
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 0, 500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 501, 1000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 1001, 1500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 1501, 2000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 2001, 2500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 2501, 3000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 3001, 3500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 3501, 4000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 4001, 4500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 4501, 5000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 5001, 5500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 5501, 6000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 6001, 6500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 6501, 7000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 7001, 7500); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 7501, 8000); } ),
            //    Task.Factory.StartNew(async () => { await UserFiltering(users.Items, 8001, 8500); } ),
            //    Task.Factory.StartNew(async () => { await UserFitltering(users.Items, 8501, 9000); } ),
            //};

            //Task.WaitAll(tasks.ToArray());

            //return null;
            //return users;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    private async Task UserFiltering(List<GetUsersResponse> users)
    {
        IGenericRepository<User> userRepository = new GenericRepository<User>(new ApplicationDbContext(_options, _currentUserService, _infrastructureOptions), _infrastructureOptions);
        IGenericRepository<UserSubscription> userSubscriptionRepository = new GenericRepository<UserSubscription>(new ApplicationDbContext(_options, _currentUserService, _infrastructureOptions), _infrastructureOptions);

        var userSubscriptionList = new List<GetUsersResponse>();
        foreach (var user in users)
        {
            UserSubscription userSubscription = null;
            var chargebeeSubscription = _customerService.GetSubscription(user.ChargeBeeCustomerID, null, SubscriptionStatus.Active);
            if (chargebeeSubscription is not null)
            {
                userSubscription = await userSubscriptionRepository.GetFirstOrDefaultNoTrackingAsync(x => x.fk_UserID == user.ID && x.ChargeBeeID.Equals(chargebeeSubscription.ID) && !x.IsDeleted);
                if (userSubscription is not null)
                {
                    userSubscription.Status = SubscriptionState.Active;
                }
            }
            else
            {
                chargebeeSubscription = _customerService.GetSubscription(user.ChargeBeeCustomerID);
                userSubscription = await userSubscriptionRepository.GetFirstOrDefaultNoTrackingAsync(x => x.fk_UserID == user.ID && x.ChargeBeeID.Equals(chargebeeSubscription.ID) && !x.IsDeleted);
                if (userSubscription is not null)
                {
                    userSubscription.Status = chargebeeSubscription.Status.ToString().ToEnum<SubscriptionState>();
                }
            }

            if (userSubscription is null)
            {
                userSubscription = new UserSubscription();
                userSubscription.ChargeBeeID = "NULL_IN_DB";
            }

            //user.ChargeBeeSubscriptionID = userSubscription.ChargeBeeID;
            //user.State = userSubscription.Status;

            //userRepository.ExecuteQueryNonScalar(
            //    $"INSERT INTO [dbo].[TempUser] ([ID] ,[Email] ,[FirstName] ,[LastName] ,[State] ,[ChargeBeeCustomerID] ,[ChargeBeeSubscriptionID]) " +
            //    $"VALUES ({user.ID} ,'{user.Email}' ,'{user.FirstName}' ,'{user.LastName}' ,{((int?)user.State)},'{user.ChargeBeeCustomerID}','{user.ChargeBeeSubscriptionID}')");
        }
    }
    private async Task UserFiltering(List<GetUsersResponse> users, int from, int to)
    {
        IGenericRepository<User> userRepository = new GenericRepository<User>(new ApplicationDbContext(_options, _currentUserService, _infrastructureOptions), _infrastructureOptions);
        IGenericRepository<UserSubscription> userSubscriptionRepository = new GenericRepository<UserSubscription>(new ApplicationDbContext(_options, _currentUserService, _infrastructureOptions), _infrastructureOptions);

        var usersTOAddTODB = users.Where(x => x.ID >= from && x.ID <= to).ToList();
        var userSubscriptionList = new List<GetUsersResponse>();
        foreach (var user in usersTOAddTODB)
        {
            UserSubscription userSubscription = null;
            var chargebeeSubscription = _customerService.GetSubscription(user.ChargeBeeCustomerID, null, SubscriptionStatus.Active);
            if (chargebeeSubscription is not null)
            {
                userSubscription = await userSubscriptionRepository.GetFirstOrDefaultAsync(x => x.fk_UserID == user.ID && x.ChargeBeeID.Equals(chargebeeSubscription.ID) && !x.IsDeleted);
                if (userSubscription is null)
                {
                    userSubscription.Status = SubscriptionState.Active;
                }
            }
            else
            {
                chargebeeSubscription = _customerService.GetSubscription(user.ChargeBeeCustomerID);
                userSubscription = await userSubscriptionRepository.GetFirstOrDefaultAsync(x => x.fk_UserID == user.ID && x.ChargeBeeID.Equals(chargebeeSubscription.ID) && !x.IsDeleted);
                if (userSubscription is null)
                {
                    userSubscription.Status = chargebeeSubscription.Status.ToString().ToEnum<SubscriptionState>();
                }

                userSubscription = await userSubscriptionRepository.GetFirstOrDefaultAsync(x => x.fk_UserID == user.ID && !x.IsDeleted, selector: y => y.ID);
            }

            //user.ChargeBeeSubscriptionID = userSubscription.ChargeBeeID;
            //user.State = userSubscription.Status;

            //userRepository.ExecuteSqlQuery(
            //    $"INSERT INTO [dbo].[TempUser] ([ID] ,[Email] ,[FirstName] ,[LastName] ,[State] ,[ChargeBeeCustomerID] ,[ChargeBeeSubscriptionID]) " +
            //    $"VALUES ({user.ID} ,'{user.Email}' ,'{user.FirstName}' ,'{user.LastName}' ,{user.State},'{user.ChargeBeeCustomerID}','{user.ChargeBeeSubscriptionID}')");
        }
    }
    private async Task UserFiltering(GetUsersResponse user)
    {
        IGenericRepository<User> userRepository = new GenericRepository<User>(new ApplicationDbContext(_options, _currentUserService, _infrastructureOptions), _infrastructureOptions);
        IGenericRepository<UserSubscription> userSubscriptionRepository = new GenericRepository<UserSubscription>(new ApplicationDbContext(_options, _currentUserService, _infrastructureOptions), _infrastructureOptions);

        UserSubscription userSubscription = null;
        var chargebeeSubscription = _customerService.GetSubscription(user.ChargeBeeCustomerID, null, SubscriptionStatus.Active);
        if (chargebeeSubscription is not null)
        {
            userSubscription = await userSubscriptionRepository.GetFirstOrDefaultAsync(x => x.fk_UserID == user.ID && x.ChargeBeeID.Equals(chargebeeSubscription.ID) && !x.IsDeleted);
            if (userSubscription is null)
            {
                userSubscription.Status = SubscriptionState.Active;
            }
        }
        else
        {
            chargebeeSubscription = _customerService.GetSubscription(user.ChargeBeeCustomerID);
            userSubscription = await userSubscriptionRepository.GetFirstOrDefaultAsync(x => x.fk_UserID == user.ID && x.ChargeBeeID.Equals(chargebeeSubscription.ID) && !x.IsDeleted);
            if (userSubscription is null)
            {
                userSubscription.Status = chargebeeSubscription.Status.ToString().ToEnum<SubscriptionState>();
            }

            userSubscription = await userSubscriptionRepository.GetFirstOrDefaultAsync(x => x.fk_UserID == user.ID && !x.IsDeleted, selector: y => y.ID);
        }

        //user.ChargeBeeSubscriptionID = userSubscription.ChargeBeeID;
        //user.State = userSubscription.Status;

        //userRepository.ExecuteSqlQuery(
        //    $"INSERT INTO [dbo].[TempUser] ([ID] ,[Email] ,[FirstName] ,[LastName] ,[State] ,[ChargeBeeCustomerID] ,[ChargeBeeSubscriptionID]) " +
        //    $"VALUES ({user.ID} ,{user.Email} ,{user.FirstName} ,{user.LastName} ,{user.State},{user.ChargeBeeCustomerID},{user.ChargeBeeSubscriptionID})");
    }

}
