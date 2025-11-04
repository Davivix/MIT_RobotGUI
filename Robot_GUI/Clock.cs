using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Robot_GUI
{
    internal class Clock : IDisposable
    {
        // private
        private CancellationTokenSource Clock_cts;
        private Action TickAction;
        private bool IsRunning;
        private Stopwatch StopWatch;

        // public
        public bool ClockSignal { get; private set; } = false;
        public int Interval_ms { get; private set; } = 500;
        public double Frequency { get; private set; } = 1;

        public Clock(Action onTick)
        {
            TickAction = onTick;
            StopWatch = new Stopwatch();
        }

        private async Task RunLoopAsync(CancellationToken token)
        {
            double nextTick = 0;
            StopWatch.Restart();

            while (!token.IsCancellationRequested)
            {
                double elapsed = StopWatch.Elapsed.TotalMilliseconds;

                if (elapsed >= nextTick)
                {
                    ClockSignal = !ClockSignal;
                    TickAction.Invoke();

                    nextTick += Interval_ms;
                }

                // Sleep a tiny bit to reduce CPU usage
                await Task.Delay(1, token).ConfigureAwait(false);
            }

            StopWatch.Stop();
        }

        public void SetFrequency(double hz)
        {
            if (hz > Robot.MaxFrequency)
                hz = Robot.MaxFrequency;

            if (hz < 0.5)
                hz = 0.5;

            Frequency = hz;
            Interval_ms = (int)(1000.0 / hz / 2);
        }

        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            Clock_cts = new CancellationTokenSource();
            Task.Run(() => RunLoopAsync(Clock_cts.Token));
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            Clock_cts.Cancel();
            Clock_cts.Dispose();
            Clock_cts = null;
        }

        public void Dispose()
        {
            Stop();
            StopWatch = null;
            TickAction = null;
        }
    }

    public static class Precision
    {
        public static async Task DelayAsync(int targetMilliseconds, CancellationToken token)
        {
            var sw = Stopwatch.StartNew();
            long targetTicks = (long)(targetMilliseconds * (Stopwatch.Frequency / 1000.0));

            // Main wait loop
            while (sw.ElapsedTicks < targetTicks)
            {
                token.ThrowIfCancellationRequested();

                // Remaining time in milliseconds
                double remainingMs = (targetTicks - sw.ElapsedTicks) / (Stopwatch.Frequency / 1000.0);

                // Sleep briefly if enough time left, spin if very close
                if (remainingMs > 2)
                    await Task.Delay(1, token).ConfigureAwait(false);
                else
                    Thread.SpinWait(1000);
            }
        }
    }
}
