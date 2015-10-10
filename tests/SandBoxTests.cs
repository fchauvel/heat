using Microsoft.VisualStudio.TestTools.UnitTesting;
using Heat;
using System.IO;

namespace Sandbox
{
    [TestClass]
    public class TestSpeech
    {
        [TestMethod]
        public void sandbox()
        {
            const string yamlSnippet = @"
                name: Sample Workout
                workout:
                  - Pushups
                  - Burpees
                  - Standing Crunches
                ";

            var circuit = Circuit.fromYAML(new StringReader(yamlSnippet));

            var expected = new string[] { "Pushups", "Burpees", "Standing Crunches" }; 
            CollectionAssert.AreEqual(circuit.GetMoves(), expected, circuit.GetMoves().ToString());
        }


    }
}
