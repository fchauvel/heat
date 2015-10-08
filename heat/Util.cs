using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Heat
{
    class Cursor<T>
    {
        private ICollection<T> source;
        private int current;

        public Cursor(ICollection<T> source)
        {
            this.source = source;
            this.current = 0;
        }

        public bool HasNext()
        {
            return current < source.Count;
        }

        public int GetCurrent()
        {
            return current;
        }

        public void Next()
        {
            current = current + 1;
        }
    }

    public class Timer
    {
        public delegate void TickHandler(int now=35);

        public static void CountDown(int seconds, TickHandler handler)
        {
            Timer timer = new Timer(seconds, handler);
            Thread thread = new Thread(timer.DoWork);
            thread.Start();
            thread.Join();
        }

        private readonly int seconds;
        private readonly TickHandler handler;
        private readonly int steps;

        public Timer(int seconds, TickHandler handler, int steps = ONE_SECOND)
        {
            this.seconds = seconds;
            this.handler = handler;
            this.steps = steps;
        }

        public void DoWork()
        {
            for (int i = 0; i < seconds; i++)
            {
                handler(seconds - i);
                Sleep();
            }
        }

        protected void Sleep()
        {
            Thread.Sleep(steps);
        }

        private const int ONE_SECOND = 1000;
    }
}
