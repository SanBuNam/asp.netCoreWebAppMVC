using System.Threading;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.General
{
    public class BackGroundService
    {
        protected virtual Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}