using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;
using Moq;


namespace UnitTestProject1
{

    class TestTrainee: Trainee
    {
        public static readonly String REST = "rest";
        public static readonly String NEXT_ROUND = "break";

        private List<String> calls;

        public TestTrainee()
        {
            this.calls = new List<String>();
        }

        public override void Excercise(string move)
        {
            calls.Add(move);
        }

        public override void Break()
        {
            calls.Add(NEXT_ROUND);
        }

        public override void SwitchTo()
        {
            calls.Add(REST);
        }

        public string[] CallSequence()
        {
            return calls.ToArray();
        }
    }


    [TestClass]
    public class TestCoach
    {
        [TestMethod]
        public void TestCoachRunCircuit()
        {
            Circuit circuit = PrepareCircuit();

            TestTrainee trainee = new TestTrainee();
            Session session = new Session(circuit, new Level(2));
            session.Run(trainee);

            CollectionAssert.AreEqual(trainee.CallSequence(), new String[] {
                "burpees", TestTrainee.REST,"push-ups",
                TestTrainee.NEXT_ROUND,
                "burpees", TestTrainee.REST, "push-ups" });
        }

        private static Circuit PrepareCircuit()
        {
            return new Circuit(new String[] { "burpees", "push-ups" });
        }

        [TestMethod]
        public void TestRunningCircuit()
        {
            const int BREAK_DURATION = 5;

            var uiMock = new Mock<UserInterface>();
            var level = new Level(2, breakTime: BREAK_DURATION);
            var trainee = new TraineeAdapter(uiMock.Object, level);
            trainee.Break();

            uiMock.Verify(m => m.ShowAction(It.Is<string>(text => text.Equals("BREAK"))), Times.Once());
            uiMock.Verify(m => m.ShowTime(It.IsAny<int>()), Times.Exactly(BREAK_DURATION));            
        }
    }


}
