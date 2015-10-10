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

        public String ToString()
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
