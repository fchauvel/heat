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
            this.circuit = new Circuit(new string[] { "Burpees", "Push-ups" });
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
            level = Level.match(circuit.GetMoves().Length, duration, effort);
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

        public void Break()
        {
            listener.ShowAction("BREAK");
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
            var yaml = new YamlStream();
            yaml.Load(input);
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            var exercises = new List<string>();
            var sequences = (YamlSequenceNode)mapping.Children[new YamlScalarNode("workout")];
            foreach (var eachExercise in sequences)
            {
                exercises.Add(((YamlScalarNode)eachExercise).Value);
            }

            return new Circuit(exercises.ToArray());
        }

        private string[] moves;

        public Circuit(string[] moves)
        {
            this.moves = moves;
        }

        public string[] GetMoves()
        {
            return this.moves;
        }

        public override String ToString()
        {
            string text = "";
            foreach (var move in moves) { text += move + " "; }
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
            Cursor<int> rounds = new Cursor<int>(level.rounds());
            while (rounds.HasNext())
            {
                GoThroughCircuit(trainee);
                rounds.Next();
                if (rounds.HasNext()) { trainee.Break(); }
            }
        }

        private void GoThroughCircuit(Trainee trainee)
        {
            Cursor<string> move = new Cursor<string>(circuit.GetMoves());
            while (move.HasNext())
            {
                String currentMove = circuit.GetMoves()[move.GetCurrent()];
                trainee.Excercise(currentMove);
                move.Next();
                if (move.HasNext()) { trainee.SwitchTo(); }
            }
        }
    }

    public interface Trainee
    {
        void Excercise(String move);

        void Break();

        void SwitchTo();

    }
}
