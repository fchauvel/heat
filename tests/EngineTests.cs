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
        }
         
        [TestMethod]
        public void ShortenShouldUpdateTheDuration()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.Shorten();

            listener.Verify(m => m.DurationChangedTo(It.Is<int>(value => value.Equals(25))), Times.Once());
        }

        [TestMethod]
        public void ExtendShouldUpdateTheDuration()
        {
            var presenter = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(presenter.Object);

            engine.Extend();

            presenter.Verify(m => m.DurationChangedTo(It.Is<int>(value => value.Equals(35))), Times.Once());
        }

        [TestMethod]
        public void EasierShouldTriggerAnUpdateOfTheEffort()
        {
            var presenter = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(presenter.Object);

            engine.ReduceEffort();

            presenter.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(73))), Times.Once());
        }


        [TestMethod]
        public void HarderShouldTriggerAnUpdateOfTheEffort()
        {
            var presenter = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(presenter.Object);

            engine.AugmentEffort();

            presenter.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(77))), Times.Once());
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
            Assert.AreEqual(level.TotalDuration(8), 240);
        }

        [TestMethod]
        public void TestTotalTimeWithoutWorking()
        {
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.TotalDuration(8), 0);
        }

        [TestMethod]
        public void TestFullEffort()
        {
            var level = new Level(1, breakTime: 0, switchTime: 0, exerciseTime: 30);
            Assert.AreEqual(level.Effort(8), 100.00, 1e-6);
        }

        [TestMethod]
        public void TestMediumEffort()
        {
            var level = new Level(2, breakTime: 20, switchTime: 0, exerciseTime: 10);
            Assert.AreEqual(level.Effort(1), 50.00, 1e-3);
        }


        [TestMethod]
        public void TestNoEffort()
        {
            var level = new Level(1, breakTime: 10, switchTime: 0, exerciseTime: 0);
            Assert.AreEqual(level.Effort(8), 0.00, 1e-3);
        }

        [TestMethod]
        public void TestLevelOptimization()
        {
            const int expectedDuration = 30 * 60; // 30 minutes
            const double effort = 95D;
            const int exerciseCount = 8;

            var level = Level.match(exerciseCount, expectedDuration, effort);

            Assert.AreEqual(95D, level.Effort(exerciseCount), 1e-1);
            Assert.IsTrue((30 * 60 - level.TotalDuration(exerciseCount)) / (30 * 60) < 0.1);
        }

    }


}
