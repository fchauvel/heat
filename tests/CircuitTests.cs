using Heat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class CircuitTests
    {
        [TestMethod]
        public void CircuitShouldIncludeWorkout()
        {
            Circuit circuit = PrepareCircuit();

            Assert.AreEqual(circuit.Name, "Sample Circuit");
            CollectionAssert.AreEqual(circuit.Warmup, new List<string>() { "Jumping Jacks", "Running in place" });
            CollectionAssert.AreEqual(circuit.Workout, new List<string>() { "Pushups", "Burpees", "Standing Crunches" });
            CollectionAssert.AreEqual(circuit.Stretching, new List<string>() { "Standing Quadriceps Stretch", "Standing Hamstring Stretch" });
        }

        private static Circuit PrepareCircuit()
        {
            const string yamlSnippet = @"
                name: Sample Circuit
                warmup:
                  - Jumping Jacks
                  - Running in place
                workout:
                  - Pushups
                  - Burpees
                  - Standing Crunches
                stretching:
                  - Standing Quadriceps Stretch
                  - Standing Hamstring Stretch
                ";

            var circuit = Circuit.fromYAML(new StringReader(yamlSnippet));
            return circuit;
        }

        [TestMethod]
        public void CircuitShouldHaveAName()
        {
            const string NAME = "name";
            var circuit = Circuit.NamedWorkout(NAME, new List<string>());

            Assert.AreEqual(NAME, circuit.Name); 
        }


    }
}
