namespace Domain.Abstractions.IAuth;

public interface ICurrentUserService
{
    int ID { get; }
    int RoleID { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    string AccessToken { get; }

    string ChargeBeeSubscriptionID { get; }
    string ChargeBeeCustomerID { get; }
    string Cin7CustomerID { get; }

    int LoggedInUser { get; }
    int LoggedInUserRole { get; }

    bool LoggedInAs(int RoleID);
    bool RoleIs(int RoleID);
}
