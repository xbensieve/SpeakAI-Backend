using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class VoucherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<VoucherBackgroundService> _logger;

    public VoucherBackgroundService(IServiceScopeFactory scopeFactory, ILogger<VoucherBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Voucher background service is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var voucherService = scope.ServiceProvider.GetRequiredService<IVoucherService>();

                    _logger.LogInformation("Running voucher cleanup process...");
                    await voucherService.CheckAndDisableVouchersAsync();
                    _logger.LogInformation("Voucher cleanup process completed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning up vouchers.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation("Voucher background service is stopping.");
    }
}
