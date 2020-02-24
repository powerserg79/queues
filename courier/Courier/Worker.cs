using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CourierConsumers;
using CourierConsumers.Consumers;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GreenPipes;

namespace Courier
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);

                Consume();
            }
        }

        protected void Consume()
        {
            var retryAttempts = 5;
            var retryInterval = 60;
            var retryMaximumInterval = 86400;
            var retryDeltaInterval = 120;
            //Console.WriteLine("Hello World");
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                /*
                 * if set to rabbitmq://localhost then the broker will be reached by
                 * guest@localhost:5672
                 */
                var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.UseRetry(rcfg =>
                    rcfg.Exponential(retryAttempts,
                        TimeSpan.FromSeconds(retryInterval),
                        TimeSpan.FromSeconds(retryMaximumInterval),
                        TimeSpan.FromSeconds(retryDeltaInterval)));

                cfg.ReceiveEndpoint(host, "test_queue", e => e.Consumer<TestMessageConsumer>());
                //                cfg.ReceiveEndpoint(host, "test_queue", ep =>
                //                {
                //                    ep.Handler<TestMessage>(context =>
                //                    {
                //                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                //                    });
                //                });
            });

            bus.StartAsync();

            bus.Publish(new TestMessage { Text = "Hello, World." });

            Console.ReadLine();

            bus.Stop();
        }
    }
}