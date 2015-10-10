using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;

namespace Sandbox
{


    [TestClass]    
    public class TimerTests
    {

        private List<int> values = new List<int>();

        private void handler(int v)
        {
            values.Add(v);
        }

        [TestMethod]
        public void TestTimerResponse()
        {
            Timer timer = new Timer(5, handler, 1);
            timer.DoWork();
            CollectionAssert.AreEqual(new int[] {5, 4, 3, 2, 1 }, values.ToArray(), values.ToString());
        }

    }
}
