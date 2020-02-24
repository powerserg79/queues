using System;
using ConsumerContracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageProducerController : ControllerBase
    {
        private readonly ILogger<MessageProducerController> _logger;

        public MessageProducerController(ILogger<MessageProducerController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public void Post(string message)
        {
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
            });

            bus.Start(); // This is important!

            bus.Publish(new YourMessage{Text = message});
            bus.Stop();
        }
    }
}
