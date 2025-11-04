using DaneshgahProject.Models;

namespace DaneshgahProject.Infrastructure
{
    public static class ModelConverter
    {
        public static State TemperatureConvertToState(this int temperature)
        {
            return temperature switch
            {
                <= 25 => State.Exit,
                > 25 and <= 35 => State.S1,
                > 35 and <= 44 => State.S2,
                >= 45 => State.S3,
            };
        }
        public static bool GetState(out State state, int temperature)
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static bool CheckCoolerVisibility(this int temperature)
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
        public static bool CheckHeaterVisibility(this int temperature)
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

    }
}
