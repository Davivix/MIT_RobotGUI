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
        private Timer timer;
        private Action tickAction;

        public bool ClockSignal { get; private set; } = false;
        public int Interval_ms { get; private set; } = 1000; // doba půl periody, => 1Hz
        public double Frequency = 1;

        public bool IsRunning { get; private set; }

        public Clock(Action ontick)
        {
            tickAction = ontick;
            timer = new Timer(Ontick, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Ontick(object state)
        {
            ClockSignal = !ClockSignal;
            tickAction.Invoke();
        }

        public void SetFrequency(double hz)
        {
            if (hz > Robot.MaxFrequency)
                hz = Robot.MaxFrequency;

            if (hz < 0.5)
                hz = 0.5;

            Frequency = hz;
            Interval_ms = (int)(1000.0 / hz / 2);
            //Interval_ms = Math.Clamp(Interval_ms, 2, 2000); // 450 Hz => (2,22 / 2) ms, zaokrouhleno na 1, což je více než maximum (500hz), takže clamp na 2

            if (IsRunning)
                timer.Change(0, Interval_ms);
        }

        public void Start()
        {
            IsRunning = true;
            timer.Change(0, Interval_ms);
        }

        public void Stop()
        {
            IsRunning = false;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            timer.Dispose();
            timer = null;
        }

    }
}
