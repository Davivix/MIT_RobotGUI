using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Robot_GUI
{
    internal class Clock
    {
        private Timer Timer;
        private Action TickAction;

        private bool IsRunning;

        public bool ClockSignal { get; private set; } = false;
        public int Interval_ms { get; private set; } = 500; // doba půl periody, => 1Hz
        public double Frequency = 1;

      

        public Clock(Action ontick)
        {
            TickAction = ontick;
            Timer = new Timer(Ontick, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Ontick(object state)
        {
            ClockSignal = !ClockSignal;
            TickAction.Invoke();
        }

        public void SetFrequency(double hz)
        {
            if (hz > Robot.MaxFrequency)
                hz = Robot.MaxFrequency;

            if (hz < 0.5)
                hz = 0.5;

            Frequency = hz;
            Interval_ms = (int)(1000.0 / hz / 2);

            if (IsRunning)
                Timer.Change(0, Interval_ms);
        }

        public void Start()
        {
            IsRunning = true;
            Timer.Change(0, Interval_ms);
        }

        public void Stop()
        {
            IsRunning = false;
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            Timer.Dispose();
            Timer = null;
        }

    }
}
