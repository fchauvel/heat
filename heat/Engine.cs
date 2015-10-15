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
        private Scheduler scheduler;

        public Engine()
        {
            this.listener = null;
            this.scheduler 
                = new Scheduler(
                    Circuit.SimpleWorkout("Burpees", "Push-ups", "Squats"),
                    new Duration(),
                    new Effort()
                );
        }

        public void RegisterListener(Listener listener)
        {
            this.listener = listener;
            UpdateLevel();
        }

        public void LoadCircuit(Circuit circuit)
        {
            scheduler.Circuit = circuit;
            listener.CircuitChangedTo(circuit.Name);
            UpdateLevel();
        }

        public virtual void OnGo()
        {
            var session = new Session(scheduler.Circuit, scheduler.Schedule);
            session.Run(new TraineeAdapter(listener, scheduler.Schedule));
        }

        public void AugmentEffort()
        {
            scheduler.Harder();
            UpdateLevel();
        }

        public void ReduceEffort()
        {
            scheduler.Easier();
            UpdateLevel();
        }

        public void Shorten()
        {
            scheduler.Shorten();
            UpdateLevel();
        }

        public void Extend()
        {
            scheduler.Extend();
            UpdateLevel();
        }

        protected virtual void UpdateLevel()
        {
            listener.DurationChangedTo(scheduler.Duration.inMinutes());
            listener.EffortChangedTo(scheduler.Effort.AsPercentage());
            var schedule = scheduler.Schedule;
            listener.LevelChangedTo(schedule.RoundCount(), schedule.ExerciseTime(), schedule.BreakTime());
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
            // listener.ShowAction("SWITCH");
            // level.TimeSwitch(tickHandler);
        }

        public override void CircuitCompleted()
        {
            listener.ShowAction("Well Done!");
        }
    }


    public interface Listener
    {

        void ShowAction(string action);

        void ShowTime(int remaining);

        void DurationChangedTo(int newDurationInMinutes);

        void EffortChangedTo(int newEffort);

        void LevelChangedTo(int roundCount, int exerciseTime, int breakDurationInSeconds);

        void CircuitChangedTo(string circuitName);

    }

   

}
