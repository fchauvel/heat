using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;
using Moq;

namespace UnitTestProject1
{

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

            var uiMock = new Mock<UserInterface>();
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

            var uiMock = new Mock<UserInterface>();
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

            var uiMock = new Mock<UserInterface>();
            var level = new Level(2, switchTime: DURATION);
            var trainee = new TraineeAdapter(uiMock.Object, level);
            trainee.SwitchTo();

            uiMock.Verify(m => m.ShowAction(It.Is<string>(text => text.Equals(TEXT))), Times.Once());
            uiMock.Verify(m => m.ShowTime(It.IsAny<int>()), Times.Exactly(DURATION));
        }
    }


}
