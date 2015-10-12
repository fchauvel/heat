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
        private Circuit circuit;
        private Level level;

        public Engine()
        {
            this.circuit = new Circuit(new string[] { "Burpees", "Push-ups" });
            this.level = new Level(2);
        }

        public void LoadCircuit(Circuit circuit)
        {
            this.circuit = circuit;
        }

        public void OnGo(UserInterface ui)
        {
            new Thread(() => { 
                var session = new Session(circuit, level);
                session.Run(new TraineeAdapter(ui, level));
            }).Start();
        }

    }

    public class TraineeAdapter : Trainee
    {
        private readonly UserInterface presenter;
        private readonly Level level;

        public TraineeAdapter(UserInterface presenter, Level level)
        {
            this.presenter = presenter;
            this.level = level;

        }

        public void Break()
        {
            presenter.ShowAction("BREAK");
            level.TimeBreak(tickHandler);
        }


        private void tickHandler(int now)
        {
            presenter.ShowTime(now);
        }

        public void Excercise(string move)
        {
            presenter.ShowAction(move);
            level.TimeExercise(tickHandler);
        }

        public void SwitchTo()
        {
            presenter.ShowAction("SWITCH");
            level.TimeSwitch(tickHandler);
        }
    }


    public interface UserInterface
    {

        void ShowAction(string action);

        void ShowTime(int remaining);

    }

    public class Level {


        public static Level match(int exerciseCount, int totalDuration, double effort)
        {
            var bestFit = new Level(roundCount: 1, exerciseTime: 5, switchTime: 0, breakTime: 5);
            var smallestError = error(bestFit, exerciseCount, totalDuration, effort);

            for (int eachRoundCount = 1; eachRoundCount < 10; eachRoundCount++)
            {
                for (int eachExerciseTime = 15; eachExerciseTime < 90; eachExerciseTime += 1)
                {
                    for (int eachBreakTime = 6; eachBreakTime < 30; eachBreakTime += 1)
                    {
                        var candidate = new Level(eachRoundCount, eachBreakTime, 0, eachExerciseTime);
                        var candidateError = error(candidate, exerciseCount, totalDuration, effort);
                        if (smallestError > candidateError) {
                            bestFit = candidate;
                            smallestError = candidateError;
                        }
                    }
                }
            }

            return bestFit;
        }

        public static double error(Level level, int exerciseCount, int desiredDuration, double desiredEffort)
        {
            double error = Math.Pow(scaleEffort(desiredEffort) - scaleEffort(level.Effort(exerciseCount)), 2);
            error += Math.Pow(scaleDuration(desiredDuration) - scaleDuration(level.TotalDuration(exerciseCount)), 2);
            return error;
        }

        private static double scaleDuration(int duration)
        {
            return ((double)duration) / MAXIMUM_DURATION;
        }

        private static double scaleEffort(double effort)
        {
            return effort * 100D;
        }

        private static readonly double MAXIMUM_DURATION = 90 * 60;


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

        public int TotalDuration(int moveCount)
        {
            return roundCount * ((exerciseDuration * moveCount) + (switchDuration * (moveCount - 1)))
                + (breakDuration * (roundCount - 1));
        }

        public double Effort(int moveCount)
        {
            double exerciseTime = roundCount * exerciseDuration * moveCount;
            int totalDuration = TotalDuration(moveCount); 
            return totalDuration == 0 ? 0D : 100D * exerciseTime / totalDuration;
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

    public class Circuit {

        public static Circuit fromYAML(TextReader input)
        {
            var yaml = new YamlStream();
            yaml.Load(input);
            var mapping = (YamlMappingNode) yaml.Documents[0].RootNode;

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
