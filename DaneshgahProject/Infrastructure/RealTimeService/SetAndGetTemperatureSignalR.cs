using DaneshgahProject.Infrastructure.BackgroundServices;
using DaneshgahProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DaneshgahProject.Infrastructure.RealTimeService
{
    public class SetAndGetTemperatureSignalR : Hub
    {
        private readonly ISetAndGetTemperature _service;
        private readonly ITemperatureBackgroundService _temperatureService;
        private static bool _isSimulationRunning = false;

        public SetAndGetTemperatureSignalR(ISetAndGetTemperature service, ITemperatureBackgroundService temperatureService)
        {
            _service = service;
            _temperatureService = temperatureService;
        }

        public async Task<object> SendMessage(int temperature)
        {
            var result = _service.CreateAndGet(temperature);
            //await _temperatureService.IncreaseAndDecreaseTemperature(temperature,
            //    result.Result.systemState.temperatureLog.ChangeTime, CancellationToken.None);

            await Clients.All.SendAsync("ReceiveMessage", new
             {
                 temperature = result.Result.systemState.temperatureLog.Temperature,
                 state = result.Result.systemState.CurrentState.ToString(),
                 heater = result.Result.systemState.HeaterOn,
                 cooler = result.Result.systemState.CoolerOn,
                 message = result.Result.Reason
             });
            return new
            {
                temperature = result.Result.systemState.temperatureLog.Temperature,
                state = result.Result.systemState.CurrentState.ToString(),
                heater = result.Result.systemState.HeaterOn,
                cooler = result.Result.systemState.CoolerOn,
                message = result.Result.Reason
            };
        }
        public async Task StartSimulation()
        {
            _isSimulationRunning = true;
            await Clients.Caller.SendAsync("ReceiveMessage", new
            {
                message = "شبیه‌سازی شروع شد",
                temperature = 20,
                state = "S1",
                heater = false,
                cooler = false
            });

            // شروع شبیه‌سازی خودکار
            _ = StartAutoSimulation();
        }

        public async Task StopSimulation()
        {
            _isSimulationRunning = false;
            await Clients.Caller.SendAsync("ReceiveMessage", new
            {
                message = "شبیه‌سازی متوقف شد",
                temperature = 20,
                state = "S1",
                heater = false,
                cooler = false
            });
        }

        private async Task StartAutoSimulation()
        {
            var random = new Random();
            while (_isSimulationRunning)
            {
                // شبیه‌سازی تغییرات دما
                var temp = random.Next(15, 35);
                var result = _service.CreateAndGet(temp);
                await Clients.All.SendAsync("ReceiveMessage", result);

                await Task.Delay(3000); // هر 3 ثانیه
            }
        }
    }
}
