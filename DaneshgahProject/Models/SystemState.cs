using System;
using System.ComponentModel.DataAnnotations;

namespace DaneshgahProject.Models
{
    public class SystemState
    {
        public SystemState(Guid id, State currentState, bool heaterOn, bool coolerOn,
            DateTime lastUpdated, string lastEvent)
        {
            if (!Guard())
            {
                Id = id;
                CurrentState = currentState;
                HeaterOn = heaterOn;
                CoolerOn = coolerOn;
                LastUpdated = lastUpdated;
                LastEvent = lastEvent;
            }
            else
            {
                Edit(id, currentState, heaterOn, coolerOn, lastUpdated, lastEvent);
            }
        }
        public void Edit(Guid id, State currentState, bool heaterOn, bool coolerOn,
            DateTime lastUpdated, string lastEvent)
        {
            Id = id;
            CurrentState = currentState;
            HeaterOn = heaterOn;
            CoolerOn = coolerOn;
            LastUpdated = lastUpdated;
            LastEvent = lastEvent;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        public State CurrentState { get; set; }

        [Required]
        public bool HeaterOn { get; set; }

        [Required]
        public bool CoolerOn { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        [MaxLength(100)]
        public string LastEvent { get; set; }

        public DeviceLog deviceLog { get; set; }

        public TemperatureLog temperatureLog { get; set; }

        public void SetDevice(DeviceLog deviceLog)
        {
            this.deviceLog = deviceLog;
        }

        public void SetTemperature(TemperatureLog temperatureLog)
        {
            this.temperatureLog = temperatureLog;
        }

        public void ChaneState(State state)
        {
            CurrentState = state;
        }
        public bool Guard()
        {
            if (this is not null)
            {
                return true;
            }
            return false;
        }
    }
    public enum State
    {
        S1 = 0,
        S2 = 1,
        S3 = 2,
        Exit = 3,
    }

}
