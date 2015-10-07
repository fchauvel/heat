using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using heat;

namespace UnitTestProject1
{

    class TestListener: Listener
    {

        public string[] CallSequence()
        {
            return new string[] { "burpees", "rest", "push-ups" };
        }
    }

    [TestClass]
    public class TestCoach
    {
        [TestMethod]
        public void TestCoachRunCircuit()
        {
            Circuit circuit = new Circuit(new String[] { "burpees", "push-ups" });

            TestListener listener = new TestListener();
            Coach coach = new Coach(); 
            coach.Run(circuit, listener);

            CollectionAssert.AreEqual(listener.CallSequence(), new String[] { "burpees", "rest", "push-ups" });
        }
    }
}
