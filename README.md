
# Heat &mdash; Simple Circuit Trainer

![alt text](https://img.shields.io/appveyor/ci/fchauvel/heat.svg)
![codecov.io](https://img.shields.io/codecov/c/github/fchauvel/heat.svg)

Heat is a simple circuit trainer, a tool that helps you go through your workout routine by timing
all exercises. The workout routines are defined as YAML file, which can be edited outside of Heat.

````yaml
name: My daily routine
description:
  The routine, which I do everyday, before to take breakfast.
warmup:
  - Walk in Place
  - Knees Circles
  - Arm Circle
workout:
  - Front Kicks
  - Squats
  - Burpees
  - Standing Crunches
stretching:
  - Arm Pull Shoulder Stretch
  - Overhead Triceps Stretch
````

Heat will let you adjust the time and effort you put in each session.

