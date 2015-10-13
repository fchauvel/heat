using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;
using Moq;

namespace Tests
{

    [TestClass]
    public class TestEngine
    {
        [TestMethod]
        public void SetListenerShouldUpdateBothDurationAndEffort()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();

            engine.RegisterListener(listener.Object);

            listener.Verify(m => m.DurationChangedTo(It.Is<int>(duration => duration.Equals(30))), Times.Once());
            listener.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(Effort.DEFAULT))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void ShortenShouldUpdateTheDuration()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.Shorten();

            listener.Verify(m => m.DurationChangedTo(It.Is<int>(value => value.Equals(25))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ExtendShouldUpdateTheDuration()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.Extend();

            listener.Verify(m => m.DurationChangedTo(It.Is<int>(value => value.Equals(35))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [TestMethod]
        public void EasierShouldTriggerAnUpdateOfTheEffort()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.ReduceEffort();

            listener.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(73))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));

        }


        [TestMethod]
        public void HarderShouldTriggerAnUpdateOfTheEffort()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.AugmentEffort();

            listener.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(77))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }
    }

    [TestClass]
    public class TestCoach
    {
        [TestMethod]
        public void TestCoachRunCircuit()
        {
            Circuit circuit = PrepareCircuit();

            var mockTrainee = new FakeTrainee();
            Session session = new Session(circuit, new Level(2));

            session.Run(mockTrainee);

            mockTrainee.VerifyCalls(new string[] { "burpees", "Switch", "push-ups", "Break", "burpees", "Switch", "push-ups" });
        }

        private class FakeTrainee : Trainee
        {
            private readonly List<string> calls = new List<String>();

            public void Break()
            {
                calls.Add("Break");
            }

            public void Excercise(string move)
            {
                calls.Add(move);
            }

            public void SwitchTo()
            {
                calls.Add("Switch");
            }

            public void VerifyCalls(params string[] expectedCalls)
            {
                CollectionAssert.AreEqual(calls, expectedCalls);
            }
        }

        private static Circuit PrepareCircuit()
        {
            return new Circuit(new String[] { "burpees", "push-ups" });
        }

        [TestMethod]
        public void TestBreak()
        {
            const int BREAK_DURATION = 5;

            var uiMock = new Mock<Listener>();
            var level = new Level(2, breakTime: BREAK_DURATION);
            var trainee = new TraineeAdapter(uiMock.Object, level);
            trainee.Break();

            uiMock.Verify(m => m.ShowAction(It.Is<string>(text => text.Equals("BREAK"))), Times.Once());
            uiMock.Verify(m => m.ShowTime(It.IsAny<int>()), Times.Exactly(BREAK_DURATION));            
        }

        [TestMethod]
        public void TestExercise()
        {
            const string BURPEES = "burpees";
            const int DURATION = 5;

            var uiMock = new Mock<Listener>();
            var level = new Level(2, exerciseTime: DURATION);
            var trainee = new TraineeAdapter(uiMock.Object, level);
            trainee.Excercise(BURPEES);

            uiMock.Verify(m => m.ShowAction(It.Is<string>(text => text.Equals(BURPEES))), Times.Once());
            uiMock.Verify(m => m.ShowTime(It.IsAny<int>()), Times.Exactly(DURATION));
        }


        [TestMethod]
        public void TestSwitch()
        {
            const string TEXT = "SWITCH";
            const int DURATION = 5;

            var uiMock = new Mock<Listener>();
            var level = new Level(2, switchTime: DURATION);
            var trainee = new TraineeAdapter(uiMock.Object, level);
            trainee.SwitchTo();

            uiMock.Verify(m => m.ShowAction(It.Is<string>(text => text.Equals(TEXT))), Times.Once());
            uiMock.Verify(m => m.ShowTime(It.IsAny<int>()), Times.Exactly(DURATION));
        }

        [TestMethod]
        public void TestTotalTime()
        {
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30 );
            Assert.AreEqual(level.TotalDuration(8), Duration.fromSeconds(240));
        }

        [TestMethod]
        public void TestTotalTimeWithoutWorking()
        {
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.TotalDuration(8), Duration.fromSeconds(0));
        }

        [TestMethod]
        public void TestFullEffort()
        {
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30);
            Assert.AreEqual(level.Effort(8), new Effort(100));
        }

        [TestMethod]
        public void TestMediumEffort()
        {
            var level = new Level(2, breakTime: 20, switchTime: 0, exerciseTime: 10);
            Assert.AreEqual(level.Effort(1), new Effort(50));
        }


        [TestMethod]
        public void TestNoEffort()
        {
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.Effort(8), new Effort(0));
        }

        [TestMethod]
        public void TestLevelOptimization()
        {
            var expectedDuration = Duration.fromMinutes(30);
            var effort = new Effort(95);
            const int exerciseCount = 8;

            var level = Level.match(exerciseCount, expectedDuration, effort);

            Assert.AreEqual(Effort.FromRatio(0.95), level.Effort(exerciseCount));
            Assert.AreEqual(30, level.TotalDuration(exerciseCount).inMinutes());
        }

    }


}
