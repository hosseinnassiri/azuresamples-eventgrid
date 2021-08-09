using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGridPublisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventPublisherController : ControllerBase
    {
        private readonly EventGridPublisherClient _eventGridClient;

        public EventPublisherController(EventGridPublisherClient eventGridClient)
        {
            _eventGridClient = eventGridClient;
        }

        [HttpPost]
        public async Task<ActionResult> Publish([FromBody] MyCustomBusinessEvent message)
        {
            var eventsList = new List<EventGridEvent>
            {
                new EventGridEvent(
                    "ExampleEventSubject",
                    typeof(MyCustomBusinessEvent).Name,
                    "1.0",
                    message)
            };

            await _eventGridClient.SendEventsAsync(eventsList).ConfigureAwait(false);
            return Ok();
        }
    }

    public class MyCustomBusinessEvent
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
    }
}
