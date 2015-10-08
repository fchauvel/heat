using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTestProject1
{
    class Timer
    {
        public delegate void TickHandler();

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
            for (int i= 0; i < seconds; i++)
            {
                handler();
                Sleep();
            }
        }

        protected void Sleep()
        {
            Thread.Sleep(steps);
        }

        private const int ONE_SECOND = 1000;
    }

    [TestClass]    
    public class TimerTests
    {

        private int counter = 0;

        private void handler()
        {
            counter++;
        }

        [TestMethod]
        public void TestTimerResponse()
        {
            Timer timer = new Timer(5, handler, 1);
            timer.DoWork();
            Assert.AreEqual(5, counter);
        }

    }
}
