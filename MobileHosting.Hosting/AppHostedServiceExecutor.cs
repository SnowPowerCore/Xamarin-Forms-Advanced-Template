﻿using AppHosting.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppHosting.Hosting
{
    internal class AppHostedServiceExecutor : IAppHostedServiceExecutor
    {
        private readonly IEnumerable<IHostedService> _services;
        private readonly ILogger<AppHostedServiceExecutor> _logger;

        public AppHostedServiceExecutor(
            ILogger<AppHostedServiceExecutor> logger, IEnumerable<IHostedService> services)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken token) =>
            ExecuteAsync(service => service.StartAsync(token));

        public Task ResumeAsync(CancellationToken token) =>
            ExecuteAsync(service => service.StartAsync(token));

        public Task SleepAsync(CancellationToken token) =>
            ExecuteAsync(service => service.StopAsync(token), throwOnFirstFailure: false);

        public Task StopAsync(CancellationToken token) =>
            ExecuteAsync(service => service.StopAsync(token), throwOnFirstFailure: false);

        private async Task ExecuteAsync(Func<IHostedService, Task> callback, bool throwOnFirstFailure = true)
        {
            List<Exception> exceptions = null;

            foreach (var service in _services)
            {
                try
                {
                    await callback(service);
                }
                catch (Exception ex)
                {
                    if (throwOnFirstFailure)
                    {
                        throw;
                    }

                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(ex);
                }
            }

            // Throw an aggregate exception if there were any exceptions
            if (exceptions != null)
                throw new AggregateException(exceptions);
        }
    }
}