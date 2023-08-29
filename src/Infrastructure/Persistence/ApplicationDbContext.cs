#nullable disable

using Domain.Abstractions.IAuth;
using Domain.Common.DomainEvent;
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Entities.LoggingModule;
using Domain.Entities.LookupsModule;
using Domain.Entities.OrdersModule;
using Domain.Entities.PantryItemModule;
using Domain.Entities.PlansModule;
using Domain.Entities.PostCodeModule;
using Domain.Entities.SchoolsModule;
using Domain.Entities.SubscriptionModule;
using Domain.Entities.UsersModule;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        #region CONSTRUCTORS AND LOCALS

        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ICurrentUserService _currentUserService;
        private readonly InfrastructureOptions _options;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, InfrastructureOptions infrastructureOptions) : base(options)
        {
            _currentUserService = currentUserService;
            _options = infrastructureOptions;
        }

        //private readonly ICurrentUserService _currentUserService;
        //private readonly ConnectionInfo _connectionInfo;
        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, ConnectionInfo infrastructureOptions, IDomainEventDispatcher dispatcher) : base(options)
        //{
        //    _currentUserService = currentUserService;
        //    _connectionInfo = infrastructureOptions;
        //    _dispatcher = dispatcher;
        //}

        #endregion

        #region OVERRIDES
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_options.ConnectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplicationDbContextSeed.SeedSampleDataAsync(modelBuilder);
        }

        public override int SaveChanges()
        {
            var userID = _currentUserService.ID;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = (userID == 0) ? null : userID;
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsActive = true;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = (userID == 0) ? null : userID;
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = (userID == 0) ? null : userID;
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = (userID == 0) ? null : userID;
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<Role>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = (userID == 0) ? null : userID;
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = (userID == 0) ? null : userID;
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                }
            }

            PreSaveChanges().GetAwaiter().GetResult();
            var response = 0;
            try
            {
                response = base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            PostSaveChanges().GetAwaiter().GetResult();

            return response;
        }
        private async Task PreSaveChanges()
        {
            await Task.CompletedTask;
        }
        private async Task PostSaveChanges()
        {
            await DispatchDomainEvents();
        }
        private async Task DispatchDomainEvents()
        {
            var domainEventEntities = ChangeTracker.Entries<IHasDomainEventEntity>()
                .Select(po => po.Entity)
                .Where(po => po.DomainEvents.Any())
                .ToArray();

            foreach (var entity in domainEventEntities)
            {
                while (entity.DomainEvents.TryTake(out IDomainEvent dev))
                    await _dispatcher.Dispatch(dev);
            }
        }

        #endregion

        #region MODULES

        #region GENERAL MODULES

        public virtual DbSet<ApiCallLog> ApiCallLogs { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<MiddlewareLog> MiddlewareLogs { get; set; }

        #endregion

        #region LOOKUPS MODULES

        public virtual DbSet<Carrier> Carriers { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Day> Days { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }

        #endregion

        #region SCHOOL MODULE

        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<SchoolDeliverySchedule> SchoolDeliverySchedules { get; set; }

        #endregion

        #region ORDERS MODULE

        public virtual DbSet<OrderMapping> OrderMappings { get; set; }
        public virtual DbSet<SaleOrder> SaleOrders { get; set; }

        #endregion

        #region PANTRY ITEMS MODULE

        public virtual DbSet<PantryItem> PantryItems { get; set; }
        public virtual DbSet<PantryItemState> PantryItemStates { get; set; }
        public virtual DbSet<PantryItemCategory> PantryItemCategories { get; set; }
        public virtual DbSet<PantryItemVariation> PantryItemVariations { get; set; }

        #endregion

        #region POST SCHEDULE MODULE

        public virtual DbSet<PostCode> PostCodes { get; set; }
        public virtual DbSet<PostCodeDeliverySchedule> PostCodeDeliverySchedules { get; set; }
        public virtual DbSet<PostCodeServiceRequest> RequestServiceCustomers { get; set; }

        #endregion

        #region SUBSCRIPTION MODULE

        public virtual DbSet<Produce> Produces { get; set; }

        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<PlanVariation> PlanVariations { get; set; }

        public virtual DbSet<SubscriptionSetting> SubscriptionSettings { get; set; }
        public virtual DbSet<SubscriptionUnwantedProduce> SubscriptionUnwantedProduces { get; set; }
        public virtual DbSet<SubscriptionReplacementProduce> SubscriptionReplacementProduces { get; set; }

        #endregion

        #region USERS MODULES

        public virtual DbSet<NonSubscribingUser> NonSubscribingUsers { get; set; }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLoginHistory> UserLoginHistories { get; set; }
        public virtual DbSet<UserPantryItem> UserPantryItems { get; set; }
        public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

        #endregion

        #endregion
    }
}
