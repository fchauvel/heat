﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Heat
{
    class Duration
    {
        private static readonly int MAXIMUM;
        private readonly int duration;

        public Duration(int durationInSeconds)
        {
            if (duration < 0) {
                var error = String.Format("Duration must be positive (found {0})", durationInSeconds);
                throw new ArgumentException(error);
            }
            this.duration = durationInSeconds;
        }

        public double normalize()
        {
            return ((double)duration) / MAXIMUM;
        }

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
