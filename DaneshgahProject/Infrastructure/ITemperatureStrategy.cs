namespace DaneshgahProject.Infrastructure
{
    public interface ITemperatureStrategy<T>
    {
        T Create(int Temperature);
        T Edit(int Temperature);
    }
}
