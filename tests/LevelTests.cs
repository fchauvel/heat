using Heat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class LevelTests
    {
        [TestMethod]
        public void TestTotalTime()
        {
            var circuit = sampleCircuit();
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30);
            Assert.AreEqual(level.TotalDuration(circuit), Duration.fromSeconds(240));
        }

        [TestMethod]
        public void TestTotalTimeWithoutWorking()
        {
            var circuit = sampleCircuit();
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.TotalDuration(circuit), Duration.fromSeconds(0));
        }

        [TestMethod]
        public void TestFullEffort()
        {
            var circuit = sampleCircuit();
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30);
            Assert.AreEqual(level.Effort(circuit), new Effort(100));
        }

        [TestMethod]
        public void TestMediumEffort()
        {
            var circuit = sampleCircuit();
            var level = new Level(2, breakTime: 20, switchTime: 0, exerciseTime: 10);
            Assert.AreEqual(level.Effort(circuit), new Effort(50));
        }


        [TestMethod]
        public void TestNoEffort()
        {
            var circuit = sampleCircuit();
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.Effort(circuit), new Effort(0));
        }

        private static Circuit sampleCircuit()
        {
            return Circuit.SimpleWorkout("1", "2", "3", "4", "5", "6", "7", "8");
        }

        [TestMethod]
        public void TestLevelOptimization()
        {
            var expectedDuration = Duration.fromMinutes(30);
            var effort = new Effort(95);
            var circuit = sampleCircuit();

            var level = Level.match(circuit, expectedDuration, effort);

            Assert.AreEqual(Effort.FromRatio(0.95), level.Effort(circuit));
            Assert.AreEqual(30, level.TotalDuration(circuit).inMinutes());
        }

    }
}
