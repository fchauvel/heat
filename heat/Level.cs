using System;
using System.Collections.Generic;
using System.Linq;

namespace Heat
{
    public class Scheduler
    {
        private readonly List<Level> candidateLevels;

        private Circuit circuit;
        private Duration duration;
        private Effort effort;
        private Level schedule;

        public Scheduler(Circuit circuit, Duration duration, Effort effort)
        {
            candidateLevels = new List<Level>();
            BuildCandidates();
            schedule = candidateLevels[0];
            this.circuit = circuit;
            this.duration = duration;
            this.effort = effort;
        }

        public Circuit Circuit
        {
            get { return circuit; }
            set { circuit = value; }
        }

        public Duration Duration
        {
            get { return duration; }
        }

        public Effort Effort
        {
            get { return effort; }
        }

        public Level Schedule
        {
            get
            {
                AdjustSchedule();
                return schedule;
            }
        }

        private void BuildCandidates()
        {
            for (int eachRoundCount = 2; eachRoundCount < 10; eachRoundCount++)
            {
                for (int eachExerciseTime = 15; eachExerciseTime < 90; eachExerciseTime += 1)
                {
                    for (int eachBreakTime = 0; eachBreakTime < 60; eachBreakTime += 1)
                    {
                        candidateLevels.Add(new Level(eachRoundCount, eachBreakTime, 0, eachExerciseTime));
                    }
                }
            }
        }

        public void Shorten()
        {
            duration = duration.Decrement();
        }

        public void Extend()
        {
            duration = duration.Increment();
        }

        public void Easier()
        {
            effort = effort.PreviousLevel();
        }

        public void Harder()
        {
            effort = effort.NextLevel();
        }

        private void AdjustSchedule()
        {
            var selection = new List<Level>() { candidateLevels[0] };
            var smallestError = Evaluate(candidateLevels[0]);
            foreach (var candidate in candidateLevels)
            {
                var error = Evaluate(candidate);
                if (error < smallestError)
                {
                    selection.Clear();
                    selection.Add(candidate);
                    smallestError = error;
                } else if (error == smallestError) {
                    selection.Add(candidate);
                }
            }
            schedule = selection.Find(level => level.RoundCount() == selection.Min(p => p.RoundCount()));
        }

        private double Evaluate(Level level)
        {
            return
                Math.Pow(effort.Normalized() - level.Effort(circuit).Normalized(), 2)
                + Math.Pow(duration.Normalized() - level.TotalDuration(circuit).Normalized(), 2);
        }

    }

    public class Phase
    {
        private int roundCount;
        private int breakDuration;
        private int exerciseDuration;

        public Phase(int roundCount, int breakDuration, int exerciseDuration)
        {
            this.roundCount = roundCount;
            this.breakDuration = breakDuration;
            this.exerciseDuration = exerciseDuration;
        }

        public int RoundCount
        {
            get { return roundCount; }
            set { roundCount = value; }
        }

        public int BreakDuration
        {
            get { return breakDuration; }
            set { breakDuration = value; }
        }

        public int ExerciseDuration
        {
            get { return exerciseDuration; }
            set { exerciseDuration = value; }
        }

        public Duration ActiveTime(List<string> exercises)
        {
            if (exercises.Any())
            {
                return new Duration(roundCount * exerciseDuration * exercises.Count);
            }
            return new Duration(0);
        }

        public Duration PassiveTime(List<string> exercises)
        {
            if (exercises.Any())
            {
                return new Duration(roundCount * breakDuration);
            }
            return new Duration(0);
        }

        public Duration TotalDuration(List<string> exercises)
        {
            return ActiveTime(exercises) + PassiveTime(exercises);
        }

    }

    public class Level
    {
        private readonly Phase warmupPhase;
        private readonly Phase workoutPhase;
        private readonly Phase stretchingPhase;

        public Level(int roundCount, int breakTime = 5, int switchTime = 3, int exerciseTime = 30)
        {
            warmupPhase = new Phase(1, 5, 30);
            workoutPhase = new Phase(roundCount, breakTime, exerciseTime);
            stretchingPhase = new Phase(1, 5, 30);
        }

        public Duration TotalDuration(Circuit circuit)
        {
            return ActiveTime(circuit) + PassiveTime(circuit);
        }

        private Duration ActiveTime(Circuit circuit)
        {
            return warmupPhase.ActiveTime(circuit.Warmup)
                  + workoutPhase.ActiveTime(circuit.Workout)
                  + stretchingPhase.ActiveTime(circuit.Stretching);
        }

        private Duration PassiveTime(Circuit circuit)
        {
            return warmupPhase.PassiveTime(circuit.Warmup)
                + workoutPhase.PassiveTime(circuit.Workout)
                + stretchingPhase.PassiveTime(circuit.Stretching);
        }

        public Effort Effort(Circuit circuit)
        {
            return ActiveTime(circuit) / TotalDuration(circuit);
        }

        public int RoundCount()
        {
            return workoutPhase.RoundCount;
        }

        public int ExerciseTime()
        {
            return workoutPhase.ExerciseDuration;
        }

        public int BreakTime()
        {
            return workoutPhase.BreakDuration;
        }

        public ICollection<int> rounds()
        {
            return Enumerable.Range(0, workoutPhase.RoundCount).ToArray();
        }

        public void TimeToWarmup(Timer.TickHandler tickHandler)
        {
            Countdown(warmupPhase.BreakDuration, tickHandler);
        }

        public void TimeToWorkout(Timer.TickHandler tickHandler)
        {
            Countdown(workoutPhase.BreakDuration, tickHandler);
        }

        public void TimeToStretching(Timer.TickHandler tickHandler)
        {
            Countdown(stretchingPhase.BreakDuration, tickHandler);
        }

        public void TimeBreak(Timer.TickHandler tickHandler)
        {
            Countdown(workoutPhase.BreakDuration, tickHandler);
        }

        public void TimeExercise(Timer.TickHandler tickHandler)
        {
            Countdown(workoutPhase.ExerciseDuration, tickHandler);
        }

        public void TimeSwitch(Timer.TickHandler tickHandler)
        {
            Countdown(0, tickHandler);
        }

        private void Countdown(int duration, Timer.TickHandler tickHandler)
        {
            Timer timer = new Timer(duration, tickHandler);
            timer.DoWork();
        }

    }
}
