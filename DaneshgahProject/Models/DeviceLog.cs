using System.ComponentModel.DataAnnotations;

namespace DaneshgahProject.Models
{

    public class DeviceLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string DeviceName { get; set; }

        [Required]
        public bool Action { get; set; } // true=ON, false=OFF

        [Required]
        public DateTime Timestamp { get; set; }

        [MaxLength(200)]
        public string Reason { get; set; }

        public SystemState systemState { get; set; }

        public DeviceLog(Guid id, string deviceName, bool action, DateTime timestamp, string reason)
        {
            if (!Guard())
            {
                Id = id;
                DeviceName = deviceName;
                Action = action;
                Timestamp = timestamp;
                Reason = reason;
            }
            else
            {
                Edit(id, deviceName, action, timestamp, reason);
            }
        }
        public void Edit(Guid id, string deviceName, bool action, DateTime timestamp, string reason)
        {
            Id = id;
            DeviceName = deviceName;
            Action = action;
            Timestamp = timestamp;
            Reason = reason;
        }
        public void SetSystemState(SystemState systemStates)
        {
            this.systemState = systemStates;
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
}
