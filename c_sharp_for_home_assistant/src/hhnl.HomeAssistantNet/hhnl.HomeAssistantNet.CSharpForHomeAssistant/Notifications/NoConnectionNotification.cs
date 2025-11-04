using System;
using System.Threading;
using System.Threading.Tasks;
using hhnl.HomeAssistantNet.CSharpForHomeAssistant.Hubs;
using hhnl.HomeAssistantNet.CSharpForHomeAssistant.Services;
using hhnl.HomeAssistantNet.CSharpForHomeAssistant.Web.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace hhnl.HomeAssistantNet.CSharpForHomeAssistant.Notifications
{
    public class NoConnectionNotification : INotification
    {
        public static readonly NoConnectionNotification Instance = new();

        private NoConnectionNotification()
        {
        }

        public class Handler : INotificationHandler<NoConnectionNotification>
        {
            private readonly IBuildService _buildService;
            private readonly ILogger<Handler> _logger;
            private readonly IHubContext<SupervisorApiHub, ISupervisorApiClient> _supervisorApiHub;

            public Handler(IBuildService buildService, ILogger<Handler> logger, IHubContext<SupervisorApiHub, ISupervisorApiClient> supervisorApiHub)
            {
                _buildService = buildService;
                _logger = logger;
                _supervisorApiHub = supervisorApiHub;
            }

            public async Task Handle(NoConnectionNotification notification, CancellationToken cancellationToken)
            {
                await _supervisorApiHub.Clients.All.OnConnectionChanged(null);
                
                _logger.LogDebug($"{DateTime.Now} No client connections. Starting new instance.");

                // А разве тут не должен закрыть приложение?
                // Соединение пропадает не только потому, что приложение выключилось.
                // Там просто бывает рсигнал отрубается.
                // а тут запускает новую.
                // там по макс соединениям (10 рсигнал держит) не пускает новое подключение.
                // и в итоге будет запускать всё больше копий программы...

                // Добавлю выключение.


                _buildService.StopDeployedApplication();

                await _buildService.WaitForBuildAndDeployAsync();

                _buildService.RunDeployedApplication();
            }
        }
    }
}