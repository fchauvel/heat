using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;

namespace UnitTestProject1
{

    class TestListener: Trainee
    {
        private List<String> calls;

        public TestListener()
        {
            this.calls = new List<String>();
        }

        public override void GoFor(string move)
        {
            calls.Add(move);
        }

        public override void Relax()
        {
            calls.Add("rest");
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
            Level level = new Level(2); 

            TestListener trainee = new TestListener();
            Session coach = new Session(circuit, level); 
            coach.Run(trainee);

            CollectionAssert.AreEqual(trainee.CallSequence(), new String[] { "burpees", "rest", "push-ups", "rest", "burpees", "rest", "push-ups" });
        }
    }
}
