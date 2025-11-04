using DaneshgahProject.Models;

namespace DaneshgahProject.Infrastructure
{
    public interface ITemperatureService
    {
        TemperatureLog SetTemperature(int temperature);
        DeviceLog SetDevise(int temperature);
        SystemState SetSystemState(int temperature);
        DeviceLog Merge(TemperatureLog temperatureLog, DeviceLog deviceLog, SystemState systemState);
    }
    public class TemperatureService : ITemperatureService
    {
        public DeviceLog Merge(TemperatureLog temperatureLog, DeviceLog deviceLog, SystemState systemState)
        {
            deviceLog.SetSystemState(systemState);
            systemState.SetDevice(deviceLog);
            systemState.SetTemperature(temperatureLog);
            temperatureLog.SetSystemState(systemState);
            return deviceLog;
        }

        public DeviceLog SetDevise(int temperature)
        {
            return new DeviceLog(Guid.NewGuid(), "train", true, DateTime.Now, "Train Is Running");
        }

        public SystemState SetSystemState(int temperature)
        {
            return new SystemState(Guid.NewGuid(),
                CheckState(temperature), CheckHeaterVisibility(temperature),
                CheckCoolerVisibility(temperature), DateTime.Now, "Change Temperature");
        }

        public TemperatureLog SetTemperature(int temperature)
        {
            return new TemperatureLog(Guid.NewGuid(), temperature, TimeSpan.FromSeconds(2));
        }

        //😀بهتره تو پوشه جدا پیاده سازی بشود ولی عمدی اینجا پیاده سازی کردم

        #region Utilities

        private State CheckState(int temperature)
        {
            return temperature switch
            {
                <= 25 => State.Exit,
                > 25 and <= 35 => State.S1,
                > 35 and <= 44 => State.S2,
                >= 45 => State.S3,
            };
        }
        private bool GetState(out State state, int temperature)
        {
            try
            {
                state = temperature switch
                {
                    <= 25 => State.Exit,
                    > 25 and <= 35 => State.S1,
                    > 35 and <= 44 => State.S2,
                    >= 45 => State.S3,
                };
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private bool CheckCoolerVisibility(int temperature)
        {
            if (GetState(out State state, temperature))
            {
                return (state) switch
                {
                    State.Exit => false, // در حالت خروج کولر خاموش است
                    State.S1 => true,    // در حالت S1 کولر روشن است
                    State.S2 => true,    // در حالت S2 کولر روشن است
                    State.S3 => false,   // در حالت S3 فرضاً کولر خاموش است
                    _ => false
                };
            }
            return false;
        }
        private bool CheckHeaterVisibility(int temperature)
        {
            if (GetState(out State state, temperature))
            {
                return (state) switch
                {
                    State.Exit => true,  // در حالت خروج بخاری روشن است
                    State.S1 => false,   // در حالت S1 بخاری خاموش است
                    State.S2 => false,   // در حالت S2 بخاری خاموش است
                    State.S3 => false,   // در حالت S3 بخاری خاموش است
                    _ => false
                };
            }
            return false;
        }

        #endregion

    }
}
