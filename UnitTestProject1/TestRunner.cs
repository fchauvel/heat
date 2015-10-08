using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;

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

        public override void GoFor(string move)
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
            Circuit circuit = new Circuit(new String[] { "burpees", "push-ups" });
            
            TestTrainee trainee = new TestTrainee();
            Session session = new Session(circuit, new Level(2)); 
            session.Run(trainee);

            CollectionAssert.AreEqual(trainee.CallSequence(), new String[] {
                "burpees", TestTrainee.REST,"push-ups",
                TestTrainee.NEXT_ROUND,
                "burpees", TestTrainee.REST, "push-ups" });
        }
    }
}
