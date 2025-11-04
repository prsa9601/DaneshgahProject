using DaneshgahProject.Models;

namespace DaneshgahProject.Infrastructure.RealTimeService
{
    public interface ISetAndGetTemperature 
    {
        Task<DeviceLog> CreateAndGet(int temperature);
    }
    public class SetAndGetTemperature : ISetAndGetTemperature
    {
        private readonly ITemperatureService _service;

        public SetAndGetTemperature(ITemperatureService service)
        {
            _service = service;
        }

        public async Task<DeviceLog> CreateAndGet(int temperature)
        {
            var device = _service.SetDevise(temperature);
            var temperatureLog = _service.SetTemperature(temperature);
            var systemState = _service.SetSystemState(temperature);
            var Merge = _service.Merge(temperatureLog, device, systemState);
            return Merge;
        }
    }
}
