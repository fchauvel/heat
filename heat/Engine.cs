using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using YamlDotNet.RepresentationModel;

namespace Heat
{

    public class Engine
    {
        private Listener listener;

        private Circuit circuit;
        private Level level;
        private Duration duration;
        private Effort effort;

        public Engine()
        {
            this.listener = null;
            this.circuit = Circuit.SimpleWorkout("Burpees", "Push-ups", "Squats");
            this.level = new Level(2);
            this.duration = new Duration();
            this.effort = new Effort();
        }

        public void RegisterListener(Listener listener)
        {
            this.listener = listener;
            this.listener.DurationChangedTo(duration.inMinutes());
            this.listener.EffortChangedTo(effort.AsPercentage());
            UpdateLevel();
        }

        public void LoadCircuit(Circuit circuit)
        {
            this.circuit = circuit;
        }

        public virtual void OnGo()
        {
            var session = new Session(circuit, level);
            session.Run(new TraineeAdapter(listener, level));
        }

        public void AugmentEffort()
        {
            effort = effort.NextLevel();
            UpdateEffort();
        }

        private void UpdateEffort()
        {
            listener.EffortChangedTo(effort.AsPercentage());
            UpdateLevel();
        }

        public void ReduceEffort()
        {
            effort = effort.PreviousLevel();
            UpdateEffort();
        }

        public void Shorten()
        {
            duration = this.duration.Decrement();
            UpdateDuration();
        }

        private void UpdateDuration()
        {
            listener.DurationChangedTo(duration.inMinutes());
            UpdateLevel();
        }

        public void Extend()
        {
            duration = this.duration.Increment();
            UpdateDuration();

        }

        protected virtual void UpdateLevel()
        {
            level = Level.match(circuit.Workout.Count, duration, effort);
            listener.LevelChangedTo(level.RoundCount(), level.BreakTime());
        }

    }

    public class AsynchronousEngine : Engine
    {

        protected override void UpdateLevel()
        {
            new Thread(() =>
            {
                base.UpdateLevel();
            }).Start();

        }

        public override void OnGo()
        {
            new Thread(() =>
            {
                base.OnGo();
            }).Start();
        }
    }

    public class TraineeAdapter : Trainee
    {
        private readonly Listener listener;
        private readonly Level level;

        public TraineeAdapter(Listener listener, Level level)
        {
            this.listener = listener;
            this.level = level;
        }

        public void Break(string kind)
        {
            listener.ShowAction(kind);
            level.TimeBreak(tickHandler);
        }

        private void tickHandler(int now)
        {
            listener.ShowTime(now);
        }

        public void Excercise(string move)
        {
            listener.ShowAction(move);
            level.TimeExercise(tickHandler);
        }

        public void SwitchTo()
        {
            listener.ShowAction("SWITCH");
            level.TimeSwitch(tickHandler);
        }

        public void CircuitCompleted()
        {

        }
    }


    public interface Listener
    {

        void ShowAction(string action);

        void ShowTime(int remaining);

        void DurationChangedTo(int newDurationInMinutes);

        void EffortChangedTo(int newEffort);

        void LevelChangedTo(int roundCount, int breakDurationInSeconds);

    }

    public class Level
    {


        public static Level match(int exerciseCount, Duration duration, Effort effort)
        {
            var bestFit = new Level(roundCount: 1, exerciseTime: 5, switchTime: 0, breakTime: 5);
            var smallestError = error(bestFit, exerciseCount, duration, effort);

            for (int eachRoundCount = 1; eachRoundCount < 10; eachRoundCount++)
            {
                for (int eachExerciseTime = 15; eachExerciseTime < 90; eachExerciseTime += 1)
                {
                    for (int eachBreakTime = 6; eachBreakTime < 30; eachBreakTime += 1)
                    {
                        var candidate = new Level(eachRoundCount, eachBreakTime, 0, eachExerciseTime);
                        var candidateError = error(candidate, exerciseCount, duration, effort);
                        if (smallestError > candidateError)
                        {
                            bestFit = candidate;
                            smallestError = candidateError;
                        }
                    }
                }
            }

            return bestFit;
        }

        public static double error(Level level, int exerciseCount, Duration desiredDuration, Effort desiredEffort)
        {
            double error = Math.Pow(desiredEffort.Normalized() - level.Effort(exerciseCount).Normalized(), 2);
            error += Math.Pow(desiredDuration.Normalized() - level.TotalDuration(exerciseCount).Normalized(), 2);
            return error;
        }


        private readonly int roundCount;
        private readonly int breakDuration;
        private readonly int switchDuration;
        private readonly int exerciseDuration;

        public Level(int roundCount, int breakTime = 5, int switchTime = 3, int exerciseTime = 30)
        {
            this.roundCount = roundCount;
            this.breakDuration = breakTime;
            this.switchDuration = switchTime;
            this.exerciseDuration = exerciseTime;
        }

        public Duration TotalDuration(int moveCount)
        {
            var seconds = roundCount * ((exerciseDuration * moveCount) + (switchDuration * (moveCount - 1)))
                + (breakDuration * (roundCount - 1));
            return new Duration(seconds);
        }

        public Effort Effort(int moveCount)
        {
            double exerciseTime = roundCount * exerciseDuration * moveCount;
            Duration totalDuration = TotalDuration(moveCount);
            return totalDuration.inSeconds() == 0 ? new Effort(0) : Heat.Effort.FromRatio(exerciseTime / totalDuration.inSeconds());
        }

        public int RoundCount()
        {
            return roundCount;
        }

        public int BreakTime()
        {
            return breakDuration;
        }

        public ICollection<int> rounds()
        {
            return Enumerable.Range(0, roundCount).ToArray();
        }

        public void TimeBreak(Timer.TickHandler tickHandler)
        {
            Countdown(breakDuration, tickHandler);
        }

        public void TimeExercise(Timer.TickHandler tickHandler)
        {
            Countdown(exerciseDuration, tickHandler);
        }

        public void TimeSwitch(Timer.TickHandler tickHandler)
        {
            Countdown(switchDuration, tickHandler);
        }

        private void Countdown(int duration, Timer.TickHandler tickHandler)
        {
            Timer timer = new Timer(duration, tickHandler);
            timer.DoWork();
        }

    }

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
            try {
                var nameNode = (YamlScalarNode)document.Children[new YamlScalarNode("name")];
                var name = nameNode.Value;
                return name;
            } catch (KeyNotFoundException)
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
            try {
                var sequences = (YamlSequenceNode)container.Children[new YamlScalarNode(listName)];
                foreach (var eachExercise in sequences)
                {
                    results.Add(((YamlScalarNode)eachExercise).Value);
                }

            } catch (KeyNotFoundException)
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

    public class Session
    {
        private Circuit circuit;
        private Level level;

        public Session(Circuit circuit, Level level)
        {
            this.circuit = circuit;
            this.level = level;
        }

        public void Run(Trainee trainee)
        {
            Warmup(trainee);
            Workout(trainee);
            Stretching(trainee);
        }

        private void Warmup(Trainee trainee)
        {
            trainee.Break("warmup");
            GoThrough(trainee, circuit.Warmup);
        }

        private void Workout(Trainee trainee)
        {
            trainee.Break("workout");
            Cursor<int> rounds = new Cursor<int>(level.rounds());
            while (rounds.HasNext())
            {
                GoThrough(trainee, circuit.Workout);
                rounds.Next();
                if (rounds.HasNext()) { trainee.Break("break"); }
            }
        }

        private void Stretching(Trainee trainee)
        {
            trainee.Break("stretching");
            GoThrough(trainee, circuit.Stretching);
            trainee.CircuitCompleted();
        }

        // TODO Should be the responsibility of the trainee
        private void GoThrough(Trainee trainee, List<String> exercises)
        {
            Cursor<string> move = new Cursor<string>(exercises);
            while (move.HasNext())
            {
                String currentMove = exercises[move.GetCurrent()];
                trainee.Excercise(currentMove);
                move.Next();
                if (move.HasNext()) { trainee.SwitchTo(); }
            }
        }
    }

    public interface Trainee
    {
        void Excercise(String move);

        void Break(string kind);

        void SwitchTo();

        void CircuitCompleted();

    }
}
