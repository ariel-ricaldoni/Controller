using System;
using System.Threading;

using Timer = System.Timers.Timer;

namespace ControllerLib
{
    public class Synchronizer
    {
        public Synchronizer()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.AutoReset = true;
            _timer.Elapsed += Elapsed;
        }

        public const Int32 DefaultTargetRefreshRate = 60;
        public const Int32 TargetRefreshRateMinLimit = 20;
        public const Int32 TargetRefreshRateMaxLimit = 144;

        public Boolean IsRunning { get; private set; }
        public Int32 RefreshRate { get; private set; } = DefaultTargetRefreshRate;
        public Int32 TargetRefreshRate
        {
            get { return _targetRefreshRate; }
            set { _targetRefreshRate = (value < TargetRefreshRateMinLimit ? TargetRefreshRateMinLimit : (value > TargetRefreshRateMaxLimit ? TargetRefreshRateMaxLimit : value)); }
        }

        public void While(Action action)
        {
            Start();

            while (IsRunning)
            {
                AddIteration();

                Thread.Sleep(_sleepTime);

                action?.Invoke();
            }
        }
        public void Start()
        {
            IsRunning = true;

            ResetIterations();
            _timer.Start();
        }
        public void Stop()
        {
            IsRunning = false;

            ResetIterations();
            _timer.Stop();
        }

        private Int32 _targetRefreshRate = DefaultTargetRefreshRate;
        private Int32 _sleepTime { get; set; } = 15;
        private Int32 _iterations { get; set; } = 0;
        private Timer _timer { get; set; }

        private void AddIteration()
        {
            if (_iterations < Int32.MaxValue) _iterations++;
        }
        private void ResetIterations()
        {
            _iterations = 0;
        }
        private void SetSleepTime()
        {
            var newSleepTime = (_iterations * _sleepTime) / TargetRefreshRate;

            _sleepTime = newSleepTime > 0 ? newSleepTime : 1;
        }
        private void SetRefreshRate()
        {
            RefreshRate = _iterations;
        }
        private void Elapsed(Object sender, EventArgs e)
        {
            SetSleepTime();
            SetRefreshRate();
            ResetIterations();
        }
    }
}
