using System;
using System.Collections.Generic;
using System.Linq;

namespace Heat
{
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

        public void TimeToWarmup(Timer.TickHandler tickHandler)
        {
            Countdown(5, tickHandler);
        }

        public void TimeToWorkout(Timer.TickHandler tickHandler)
        {
            Countdown(breakDuration, tickHandler);
        }

        public void TimeToStretching(Timer.TickHandler tickHandler)
        {
            Countdown(5, tickHandler);
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
}
