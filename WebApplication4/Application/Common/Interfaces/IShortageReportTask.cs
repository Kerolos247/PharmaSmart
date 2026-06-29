namespace WebApplication4.Application.Common.Interfaces
{
    public interface IShortageReportTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
