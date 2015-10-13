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
    public class SessionTests
    {
        [TestMethod]
        public void TestCoachRunCircuit()
        {
            Circuit circuit = PrepareCircuit();

            var mockTrainee = new FakeTrainee();
            Session session = new Session(circuit, new Level(2));

            session.Run(mockTrainee);

            mockTrainee.VerifyCalls(
                "warmup", 
                "m1", "m2",
                "workout",
                "m3", "m4", 
                "break", "m3", "m4",
                "stretching",
                "m5", "m6",
                "well done!");
        }

        private static Circuit PrepareCircuit()
        {
            var warmup = new List<string> { "m1", "m2" };
            var workout = new List<string> { "m3", "m4" };
            var stretching = new List<string> { "m5", "m6" };
            return new Circuit("sample", warmup, workout, stretching);
        }

    }

    class FakeTrainee : Trainee
    {
        private readonly List<string> calls = new List<String>();
   
        private void Record(string signal) {
            calls.Add(signal);
        }

        public override void GetReadyForStretching()
        {
            Record("stretching");
        }

        public override void GetReadyForWarmup()
        {
            Record("warmup");
        }

        public override void GetReadyForWorkout()
        {
            Record("workout");
        }

        public override void Break()
        {
            Record("break");
        }

        public override void Excercise(string move)
        {
            Record(move);
        }

        public override void SwitchTo()
        {
            //Record("switch");
        }

        public override void CircuitCompleted()
        {
            Record("well done!");
        }

        public void VerifyCalls(params string[] expectedCalls)
        {
            CollectionAssert.AreEqual(calls, expectedCalls);
        }
    }
}
