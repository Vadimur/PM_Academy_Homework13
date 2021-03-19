using DepsWebApp.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DepsWebApp.Services
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class MigrationsService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            await using var productManagerContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            await productManagerContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
