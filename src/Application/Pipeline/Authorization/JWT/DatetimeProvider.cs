namespace Application.Pipeline.Authorization.JWT
{
    public interface IDatetimeProvider
    {
        DateTime UtcNow { get; }
    }
    public class DatetimeProvider : IDatetimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
