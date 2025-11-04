using System;
using System.ComponentModel.DataAnnotations;

namespace DaneshgahProject.Models
{
    public class TemperatureLog
    {
        public TemperatureLog(Guid id, double temperature, TimeSpan changeTime)
        {
            if (!Guard())
            {
                Id = id;
                Temperature = temperature;
                ChangeTime = changeTime;
            }
            else
            {
                Edit(id,temperature, changeTime);
            }
        }
        public void Edit(Guid id, double temperature, TimeSpan changeTime)
        {
                Id = id;
                Temperature = temperature;
                ChangeTime = changeTime;
            //با استفاده از هنگفایر یا بک گراند جاب 
            //میایم و هم دما رو دوثانیه یکبار میبریم بالا یا پایین
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public TimeSpan ChangeTime { get; set; }

        //[MaxLength(50)]
        //public string EventType { get; set; }

        public SystemState systemState { get; set; }
        public void SetSystemState(SystemState systemState)
        {
            this.systemState = systemState;
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
