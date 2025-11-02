using hhnl.HomeAssistantNet.Automations.HomeAssistantConnection;
using hhnl.HomeAssistantNet.Shared.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace hhnl.HomeAssistantNet.Automations.Automation
{
    public class EventFiredNotificationNotifcationHandler : INotificationHandler<HomeAssistantClient.EventFiredNotification>
    {
        private readonly IAutomationRegistry _automationRegistry;
        private readonly IAutomationService _automationService;

        public EventFiredNotificationNotifcationHandler(
            IAutomationRegistry automationRegistry,
            IAutomationService automationService)
        {
            _automationRegistry = automationRegistry;
            _automationService = automationService;
        }

        public async Task Handle(HomeAssistantClient.EventFiredNotification notification, CancellationToken cancellationToken)
        {
            var eventCallerEntityId = Events.Any.UniqueId;
            var automations = _automationRegistry.GetAutomationsTrackingEntity(eventCallerEntityId);

            // Run automations
            foreach (var automation in automations)
            {
                await _automationService.EnqueueAutomationAsync(automation, eventCallerEntityId
                    , Shared.Automation.AutomationRunInfo.StartReason.EventFired, $"Event {notification.Event.EventType} fired.", notification.Event);
            }
        }
    }
}
