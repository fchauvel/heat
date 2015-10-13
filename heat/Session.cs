using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heat
{
    
    public class Session
    {
        private readonly Circuit circuit;
        private readonly Level level;

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
            trainee.GetReadyForWarmup();
            trainee.GoThrough(circuit.Warmup);
        }

        private void Workout(Trainee trainee)
        {
            trainee.GetReadyForWorkout();
            Cursor<int> rounds = new Cursor<int>(level.rounds());
            while (rounds.HasNext())
            {
                trainee.GoThrough(circuit.Workout);
                rounds.Next();
                if (rounds.HasNext()) { trainee.Break(); }
            }
        }

        private void Stretching(Trainee trainee)
        {
            trainee.GetReadyForStretching();
            trainee.GoThrough(circuit.Stretching);
            trainee.CircuitCompleted();
        }

    }

    public abstract class Trainee
    {
        public abstract void GetReadyForWarmup();

        public abstract void GetReadyForWorkout();

        public abstract void GetReadyForStretching();

        public abstract void Excercise(String move);

        public abstract void Break();

        public abstract void SwitchTo();

        public abstract void CircuitCompleted();

        public void GoThrough(List<String> exercises)
        {
            Cursor<string> move = new Cursor<string>(exercises);
            while (move.HasNext())
            {
                String currentMove = exercises[move.GetCurrent()];
                Excercise(currentMove);
                move.Next();
                if (move.HasNext()) { SwitchTo(); }
            }
        }

    }
}
