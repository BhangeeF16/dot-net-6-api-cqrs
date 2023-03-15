namespace Domain.IContracts.IAuth
{
    public interface ICurrentUserService
    {
        int ID { get; }
        int RoleID { get; }
        string Email { get; }
        string FirstName { get; }
        string ChargeBeeSubscriptionID { get; }
        string ChargeBeeCustomerID { get; }
        string Cin7CustomerID { get; }
        bool IsAdmin { get; }
    }
}
