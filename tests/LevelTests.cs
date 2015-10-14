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
            var circuit = sampleCircuit(0, 8, 0);
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30);
            Assert.AreEqual(Duration.fromSeconds(240), level.TotalDuration(circuit));
        }

        [TestMethod]
        public void TestTotalTimeWithoutWorking()
        {
            var circuit = sampleCircuit(0, 8, 0);
            var level = new Level(roundCount:1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(Duration.fromSeconds(10), level.TotalDuration(circuit));
        }

        [TestMethod]
        public void TestFullEffort()
        {
            var circuit = sampleCircuit(0, 8, 0);
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30);
            Assert.AreEqual(level.Effort(circuit), new Effort(100));
        }

        [TestMethod]
        public void TestMediumEffort()
        { 
            var circuit = sampleCircuit(0, 8, 0);
            var level = new Level(2, breakTime: 20, switchTime: 0, exerciseTime: 10);
            Assert.AreEqual(level.Effort(circuit), Effort.FromRatio((double)160 / 200));
        }

        [TestMethod]
        public void TestNoEffort()
        {
            var circuit = sampleCircuit(0, 8,0);
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.Effort(circuit), new Effort(0));
        }

        private static Circuit sampleCircuit(int warmupCount, int workoutCount, int stretchingCount)
        {
            var warmup = ListOfNExercises(warmupCount);
            var workout = ListOfNExercises(workoutCount);
            var stretching = ListOfNExercises(stretchingCount);
            return new Circuit("sample", warmup, workout, stretching);
        }

        private static List<string> ListOfNExercises(int n)
        {
            var results = new List<string>();
            for( int i=0; i<n; i++) { results.Add("x"); }
            return results;
        }

        [TestMethod]
        public void TestLevelOptimization()
        {
            var expectedDuration = Duration.fromMinutes(30);
            var effort = new Effort(95);
            var circuit = sampleCircuit(0, 8, 0);

            var finder = new Scheduler(circuit, expectedDuration, effort);
            var level = finder.Schedule;

            Assert.AreEqual(Effort.FromRatio(0.95), level.Effort(circuit));
            Assert.AreEqual(30, level.TotalDuration(circuit).inMinutes());
        }

    }

    [TestClass]
    public class PhaseTests
    {
        [TestMethod]
        public void TestActiveDuration()
        {
            var exercises = new List<string>() { "a", "b", "c" };
            var phase = new Phase(roundCount: 1, breakDuration: 5, exerciseDuration: 30);
            var actual = phase.ActiveTime(exercises);
            Assert.AreEqual(Duration.fromSeconds(90), actual);
        }

        [TestMethod]
        public void TestActiveDurationOnEmptyWorkout()
        {
            var exercises = new List<string>() {};
            var phase = new Phase(roundCount: 1, breakDuration: 5, exerciseDuration: 30);
            var actual = phase.ActiveTime(exercises);
            Assert.AreEqual(Duration.fromSeconds(0), actual);
        }

        [TestMethod]
        public void TestPassiveDuration()
        {
            var exercises = new List<string>() { "a", "b", "c" };
            var phase = new Phase(roundCount: 2, breakDuration: 5, exerciseDuration: 30);
            var actual = phase.PassiveTime(exercises);
            Assert.AreEqual(Duration.fromSeconds(10), actual);
        }

        [TestMethod]
        public void TestTotalDuration()
        {
            var exercises = new List<string>() { "a", "b", "c" };
            var phase = new Phase(roundCount: 2, breakDuration: 5, exerciseDuration: 30);
            var actual = phase.TotalDuration(exercises);
            Assert.AreEqual(Duration.fromSeconds(10 + 180), actual);
        }

    }
}
