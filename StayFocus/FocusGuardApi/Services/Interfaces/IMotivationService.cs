namespace FocusGuardApi.Services.Interfaces
{
    public interface IMotivationService
    {
        Task<string> GetRandomQuoteAsync();
        Task<string> GetMotivationalMessageAsync(int sessionCount);
    }
}
