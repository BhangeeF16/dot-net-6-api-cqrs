namespace Application.Pipeline.Authentication.Bearer.Extensions
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
