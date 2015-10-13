using System;
using System.Collections.Generic;
using System.Threading;

namespace Heat
{

    public class Effort
    {
        public const int DEFAULT = 75;

        private readonly int effort;

        public Effort(int percentage = DEFAULT)
        {
            if (percentage < 0)
            {
                string error = string.Format("Effort value must be positive (found {0})", percentage);
                throw new ArgumentException(error);
            }
            if (percentage > 100)
            {
                string error = string.Format("Effort must a value within [0, 100] (found {0})", percentage);
            }
            effort = percentage;
        }

        public bool IsDefault()
        {
            return effort == DEFAULT;
        }

        public Effort NextIncrement()
        {
            return new Effort(effort + INCREMENT);
        }

        private const int INCREMENT = 2;

        public bool IsHarderThan(Effort other)
        {
            return effort > other.effort;
        }

        public Effort Decrement()
        {
            return new Effort(effort - INCREMENT);
        }

        public int asPercentage()
        {
            return effort;
        }
    }

    public class Duration
    {
        public static Duration fromSeconds(int durationInSeconds)
        {
            return new Duration(durationInSeconds);
        }

        public static Duration fromMinutes(int durationInMinutes)
        {
            return new Duration(durationInMinutes * SECONDS_IN_ONE_MINUTES);
        }

        public const int DEFAULT = 30 * SECONDS_IN_ONE_MINUTES;

        private readonly int duration;

        public Duration(int durationInSeconds = DEFAULT)
        {
            if (durationInSeconds <= 0) {
                var error = String.Format("Duration must be positive (found {0})", durationInSeconds);
                throw new ArgumentException(error);
            }
            this.duration = durationInSeconds;
        }

        public bool IsDefault()
        {
            return duration == DEFAULT;
        }

        public Duration Increment()
        {
            return new Duration(duration + INCREMENT);
        }

        public Duration Decrement()
        {
            return new Duration(duration - INCREMENT);
        }

        public Boolean IsLongerThan(Duration other)
        {
            return duration > other.duration;
        }

        public int inMinutes() {
            return (int) Math.Round((double) duration / SECONDS_IN_ONE_MINUTES);
        }

        public int inSeconds()
        {
            return duration;
        }

        private const int INCREMENT = 5 * SECONDS_IN_ONE_MINUTES;
        private const int SECONDS_IN_ONE_MINUTES = 60;

        public double normalize()
        {
            return ((double)duration) / MAXIMUM;
        }

        private const int MAXIMUM = 90 * SECONDS_IN_ONE_MINUTES;

    }

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
