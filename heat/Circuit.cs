using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Heat
{
    public class Circuit
    {

        public static Circuit fromYAML(TextReader input)
        {
            var document = readYAML(input);
            var name = fetchName(document);
            var warmup = fetchList(document, "warmup");
            var workout = fetchList(document, "workout");
            var stretching = fetchList(document, "stretching");
            return new Circuit(name, warmup, workout, stretching);
        }

        private static string fetchName(YamlMappingNode document)
        {
            try
            {
                var nameNode = (YamlScalarNode)document.Children[new YamlScalarNode("name")];
                var name = nameNode.Value;
                return name;
            }
            catch (KeyNotFoundException)
            {
                return DEFAULT_NAME;
            }
        }

        private static YamlMappingNode readYAML(TextReader input)
        {
            var yaml = new YamlStream();
            yaml.Load(input);
            return (YamlMappingNode)yaml.Documents[0].RootNode;
        }

        private static List<string> fetchList(YamlMappingNode container, string listName)
        {
            var results = new List<string>();
            try
            {
                var sequences = (YamlSequenceNode)container.Children[new YamlScalarNode(listName)];
                foreach (var eachExercise in sequences)
                {
                    results.Add(((YamlScalarNode)eachExercise).Value);
                }

            }
            catch (KeyNotFoundException)
            {
            }
            return results;
        }

        public static Circuit SimpleWorkout(params string[] exercises)
        {
            return NamedWorkout(DEFAULT_NAME, new List<string>(exercises));
        }

        public static Circuit NamedWorkout(string name, List<string> exercises)
        {
            return new Circuit(name, new List<string>(), exercises, new List<string>());
        }

        private const string DEFAULT_NAME = "No Name";

        private readonly string name;
        private readonly List<string> warmup;
        private readonly List<string> workout;
        private readonly List<string> stretching;

        public Circuit(string name, List<string> warmup, List<string> workout, List<string> stretching)
        {
            this.name = name;
            this.warmup = new List<string>(warmup);
            this.workout = new List<string>(workout);
            this.stretching = new List<string>(stretching);
        }

        public string Name
        {
            get { return name; }
        }

        public List<String> Warmup
        {
            get { return warmup; }
        }

        public bool HasWarmup
        {
            get { return warmup.Count > 0; }
        }

        public List<string> Workout
        {
            get { return workout; }
        }

        public List<string> Stretching
        {
            get { return stretching; }
        }

        public override String ToString()
        {
            string text = "";
            foreach (var move in workout) { text += move + " "; }
            return text;
        }

    }
}
