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
            var newEffort = effort.NextLevel();
            Assert.IsTrue(newEffort.IsHarderThan(effort));
        }

        [TestMethod]
        public void ShouldBeDecrementable()
        {
            var effort = new Effort(75);
            var newEffort = effort.PreviousLevel();
            Assert.IsTrue(effort.IsHarderThan(newEffort));
        }

        [TestMethod]
        public void ShouldBeAvailableAsPercentage()
        {
            const int PERCENTAGE = 75;
            var effort = new Effort(PERCENTAGE);
            Assert.AreEqual(PERCENTAGE, effort.AsPercentage());
        }

        [TestMethod]
        public void NormalizedValueShouldBeAvailable()
        {
            var effort = new Effort(75);
            Assert.AreEqual(0.75D, effort.Normalized());
        }

        [TestMethod]
        public void ShouldBeComparable()
        {
            var effort1 = new Effort(50);
            var effort2 = new Effort(51);
            var effort3 = new Effort(50);

            Assert.AreEqual(effort1, effort1);
            Assert.AreEqual(effort1, effort3);
            Assert.AreNotEqual(effort1, effort2);
        }

        [TestMethod]
        public void ShouldBeAvailableFromADecimalValue()
        {
            var effort = Effort.FromRatio(0.75);  
            Assert.AreEqual(75, effort.AsPercentage()); 
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
