using Microsoft.AspNetCore.SignalR;

namespace DaneshgahProject.Infrastructure.RealTimeService
{
    public class SetAndGetTemperatureSignalR : Hub
    {
        private readonly ISetAndGetTemperature _service;

        public SetAndGetTemperatureSignalR(ISetAndGetTemperature service)
        {
            _service = service;
        }
        public async Task<object> SendMessage(int temperature)
        {
            var result = _service.CreateAndGet(temperature);
            await Clients.All.SendAsync("ReceiveMessage", result);
            return result;
        }
       
    }
}
