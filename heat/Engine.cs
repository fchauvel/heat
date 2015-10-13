using System;
using System.Collections.Generic;
using System.IO;
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

        public override void GetReadyForStretching()
        {
            listener.ShowAction("Stretching");
            level.TimeToStretching(tickHandler);
        }

        public override void GetReadyForWorkout()
        {
            listener.ShowAction("Workout");
            level.TimeToWorkout(tickHandler);
        }

        public override void GetReadyForWarmup()
        {
            listener.ShowAction("Warmup");
            level.TimeToWarmup(tickHandler);
        }

        public override void Break()
        {
            listener.ShowAction("Break!");
            level.TimeBreak(tickHandler);
        }

        private void tickHandler(int now)
        {
            listener.ShowTime(now);
        }

        public override void Excercise(string move)
        {
            listener.ShowAction(move);
            level.TimeExercise(tickHandler);
        }

        public override void SwitchTo()
        {
            listener.ShowAction("SWITCH");
            level.TimeSwitch(tickHandler);
        }

        public override void CircuitCompleted()
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

   

}
