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
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void ShortenShouldUpdateTheDuration()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.Shorten();

            listener.Verify(m => m.DurationChangedTo(It.Is<int>(value => value.Equals(25))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ExtendShouldUpdateTheDuration()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.Extend();

            listener.Verify(m => m.DurationChangedTo(It.Is<int>(value => value.Equals(35))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [TestMethod]
        public void EasierShouldTriggerAnUpdateOfTheEffort()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.ReduceEffort();

            listener.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(73))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }


        [TestMethod]
        public void HarderShouldTriggerAnUpdateOfTheEffort()
        {
            var listener = new Mock<Listener>();
            var engine = new Engine();
            engine.RegisterListener(listener.Object);

            engine.AugmentEffort();

            listener.Verify(mock => mock.EffortChangedTo(It.Is<int>(effort => effort.Equals(77))), Times.Once());
            listener.Verify(mock => mock.LevelChangedTo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }
    }

    [TestClass]
    public class TestCoach
    {

        [TestMethod]
        public void TestBreak()
        {
            const int BREAK_DURATION = 5;

            var uiMock = new Mock<Listener>();
            var level = new Level(2, breakTime: BREAK_DURATION);
            var trainee = new TraineeAdapter(uiMock.Object, level);
            trainee.Break();

            uiMock.Verify(m => m.ShowAction(It.IsAny<string>()), Times.Once());
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

        
    }


}
