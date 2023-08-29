namespace Application.Modules.Users.Models
{
    public class UserImpersonatorDto
    {
        public UserImpersonatorDto(bool isUserImpersonating)
        {
            IsUserImpersonating = isUserImpersonating;
        }

        public bool IsUserImpersonating { get; set; } = false;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int fk_RoleID { get; set; } = 0;

    }
}
