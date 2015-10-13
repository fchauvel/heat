using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Heat
{
    [TestClass]
    public class EffortTest
    {
        [TestMethod]
        public void ShouldBeSetByDefault()
        {
            var effort = new Effort();
            Assert.IsTrue(effort.IsDefault());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNegativeValue()
        {
            var effort = new Effort(-34);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectValueAbove100()
        {
            var effort = new Effort(140);
        }

        [TestMethod]
        public void ShouldBeIncrementable()
        {
            var effort = new Effort(75);
            var newEffort = effort.NextIncrement();
            Assert.IsTrue(newEffort.IsHarderThan(effort));
        }

        [TestMethod]
        public void ShouldBeDecrementable()
        {
            var effort = new Effort(75);
            var newEffort = effort.Decrement();
            Assert.IsTrue(effort.IsHarderThan(newEffort));
        }

        [TestMethod]
        public void ShouldBeAvailableAsPercentage()
        {
            const int PERCENTAGE = 75;
            var effort = new Effort(PERCENTAGE);
            Assert.AreEqual(PERCENTAGE, effort.asPercentage());
        }

    }


   [TestClass]
   public class DurationTest
    {
        [TestMethod]
        public void ShouldBeSetByDefault()
        {
            var duration = new Duration();
            Assert.IsTrue(duration.IsDefault());
        }

        [TestMethod]
        public void ShouldAllowIncrementation()
        {
            var duration = Duration.fromMinutes(23);
            var result = duration.Increment();

            Assert.IsTrue(result.IsLongerThan(duration));
        }

        [TestMethod]
        public void ShouldBeAvailableInMinutes()
        {
            var duration = Duration.fromMinutes(40);
            Assert.AreEqual(40, duration.inMinutes());
        }

        [TestMethod]
        public void ShouldBeAvailableInSeconds()
        {
            var duration = Duration.fromMinutes(2);
            Assert.AreEqual(120, duration.inSeconds());
        }

        [TestMethod]
        public void ShouldAllowDecrementation()
        {
            var duration = Duration.fromMinutes(23);
            var result = duration.Decrement();

            Assert.IsTrue(duration.IsLongerThan(result));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectDecrementationBelowZero()
        {
            var duration = Duration.fromSeconds(3);
            var result = duration.Decrement();
        }

    }


}
