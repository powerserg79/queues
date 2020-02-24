using System;
using System.Threading.Tasks;
using CourierConsumers.Messages;
using MassTransit;

namespace CourierConsumers.Consumers
{
    public class TestMessageConsumer : IConsumer<ITestMessage>
    {
        public async Task Consume(ConsumeContext<ITestMessage> context)
        {
            await Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
        }
    }
}