using System;
using MassTransit;

namespace GettingStarted
{
    public class YourMessage
    {
        public string Text { get; set; }
    }

    class Program
    {
        private static void Main(string[] args)
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

            // bus.Publish(new YourMessage{Text = "Hi"});

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();
        }
    }
}
