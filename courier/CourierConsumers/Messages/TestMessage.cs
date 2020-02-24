using CourierConsumers.Messages;

namespace CourierConsumers
{
    public class TestMessage : ITestMessage
    {
        public string Text { get; set; }
    }
}