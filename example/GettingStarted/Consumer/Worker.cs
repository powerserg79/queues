using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsumerContracts;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
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
            //Console.WriteLine("Hello World");
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                /*
                 * if set to rabbitmq://localhost then the broker will be reached by
                 * guest@localhost:5672
                 */
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "test_queue", ep =>
                {
                    ep.Handler<YourMessage>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            bus.Start(); // This is important!

            //bus.Publish(new YourMessage{Text = "Hi"});

            //Console.WriteLine("Press any key to exit");
            //Console.ReadKey();

            bus.Stop();
        }
    }
}
