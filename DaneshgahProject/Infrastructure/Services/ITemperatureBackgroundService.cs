using DaneshgahProject.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaneshgahProject.Infrastructure.BackgroundServices
{
    public interface ITemperatureBackgroundService
    {
        Task StartAutoTemperatureChangeAsync();
        Task StopAutoTemperatureChangeAsync();
        Task IncreaseAndDecreaseTemperature(int temperature, TimeSpan timeSpan, CancellationToken cancellationToken);
    }

    public class TemperatureBackgroundService : Hub, IHostedService, ITemperatureBackgroundService
    {
        private Timer _timer;
        private bool _isRunning = false;
        private int _currentTemperature = 25;
        private readonly IHubContext<TemperatureBackgroundService> _hubContext;

        public TemperatureBackgroundService(IHubContext<TemperatureBackgroundService> hubContext)
        {
            _hubContext = hubContext;
            _timer = new Timer(async _ => await ExecuteMethod(), null, Timeout.Infinite, Timeout.Infinite);
        }

        private async Task ExecuteMethod()
        {
            if (!_isRunning) return;

            try
            {
                // محاسبه state بر اساس دمای فعلی
                State state = _currentTemperature.TemperatureConvertToState();

                // اعمال تغییرات دما بر اساس state
                if (state == State.S1)
                    _currentTemperature -= 2;
                else if (state == State.S2)
                    _currentTemperature -= 1;
                else if (state == State.S3)
                    _currentTemperature += 1;
                else if (state == State.Exit)
                    _currentTemperature += 2;

                // محدود کردن دما بین 10 تا 50
                _currentTemperature = Math.Max(10, Math.Min(50, _currentTemperature));

                // محاسبه مجدد state پس از تغییر دما
                state = _currentTemperature.TemperatureConvertToState();

                // محاسبه وضعیت دستگاه‌ها
                bool heater = state == State.Exit || state == State.S3;
                bool cooler = state == State.S1 || state == State.S2;

                // ایجاد شیء داده برای ارسال
                var temperatureData = new
                {
                    Temperature = _currentTemperature,
                    State = state.ToString(),
                    Heater = heater,
                    Cooler = cooler,
                    Message = $"تغییر خودکار2: {_currentTemperature}°C - حالت: {state}"
                };

                // ارسال به تمام کلاینت‌های متصل
                if (_hubContext.Clients != null)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveTemperator", temperatureData);
                }

                Console.WriteLine($"Background Service - دما: {_currentTemperature}°C, حالت: {state}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در اجرای متد Background Service: {ex.Message}");
            }
        }

        public async Task StartAutoTemperatureChangeAsync()
        {
            if (_isRunning) return;

            _isRunning = true;
            // تنظیم تایمر برای اجرای هر 1 ثانیه (1000 میلی‌ثانیه)
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));

            Console.WriteLine("سرویس تغییر خودکار دما شروع شد - هر 1 ثانیه");
            await Task.CompletedTask;
        }

        public async Task StopAutoTemperatureChangeAsync()
        {
            _isRunning = false;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            Console.WriteLine("سرویس تغییر خودکار دما متوقف شد");
            await Task.CompletedTask;
        }

        public async Task IncreaseAndDecreaseTemperature(int temperature, TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            // تنظیم دمای اولیه
            _currentTemperature = temperature;

            // شروع سرویس
            await StartAutoTemperatureChangeAsync();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Temperature Background Service شروع شد");

            // شروع خودکار سرویس هنگام راه‌اندازی
            await StartAutoTemperatureChangeAsync();

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            Console.WriteLine("Temperature Background Service متوقف شد");
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
//using DaneshgahProject.Models;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Threading;
//using static System.Net.Mime.MediaTypeNames;

//namespace DaneshgahProject.Infrastructure.BackgroundServices
//{
//    public interface ITemperatureBackgroundService
//    {

//        Task IncreaseAndDecreaseTemperature(int temperature, TimeSpan timeSpan, CancellationToken cancellationToken);
//    }
//    public class TemperatureBackgroundService : Hub, IHostedService, ITemperatureBackgroundService
//    {
//        private Timer _timer;
//        private bool _isRunning = false;

//        public TemperatureBackgroundService()
//        {
//            // تایمر با دوره اولیه صفر (غیرفعال) ایجاد می‌شود
//            _timer = new Timer(async _ => await ExecuteMethod(0, CancellationToken.None), null, Timeout.Infinite, Timeout.Infinite);
//        }

//        private async Task ExecuteMethod(int temperature, CancellationToken cancellationToken)
//        {
//            if (!_isRunning) return;

//            try
//            {
//                State state = temperature.TemperatureConvertToState();
//                if (state == State.S1)
//                    temperature -= 6;
//                if (state == State.S2)
//                    temperature -= 3;
//                if (state == State.S3)
//                    temperature += 3;
//                if (state == State.Exit)
//                    temperature += 6;
//                if (base.Clients?.All != null)
//                {
//                    await base.Clients.All.SendAsync("ReceiveTemperator", temperature, state, cancellationToken);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"خطا در اجرای متد: {ex.Message}");
//            }
//        }



//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }


//        public async Task IncreaseAndDecreaseTemperature(int temperature, TimeSpan timeSpan
//            , CancellationToken cancellationToken)
//        {
//            while (cancellationToken.IsCancellationRequested == false)
//            {
//                await StartAsync(temperature, timeSpan, cancellationToken);
//            }
//            await StopAsync(cancellationToken);

//        }

//        public async Task StartAsync(int temperature, TimeSpan timeSpan, CancellationToken cancellationToken)
//        {
//            if (_isRunning) return;

//            _isRunning = true;
//            _timer = new Timer(async _ => await ExecuteMethod(temperature, cancellationToken), null, TimeSpan.Zero, timeSpan);

//            Console.WriteLine("سرویس شروع شد - هر 3 ثانیه یکبار متد صدا زده می‌شود");
//            await Task.CompletedTask;
//        }

//        public async Task StopAsync(CancellationToken cancellationToken)
//        {
//            _isRunning = false;
//            _timer?.Dispose();
//            _timer = null;

//            Console.WriteLine("سرویس متوقف شد");
//            await Task.CompletedTask;
//        }

//        public async Task StartAsync(CancellationToken cancellationToken)
//        {
//            _timer = new Timer(async _ => await ExecuteMethod(0, CancellationToken.None), null, Timeout.Infinite, Timeout.Infinite);
//        }
//    }
//}