the scheduler system is a very powerful system

schedulers are for timing events

it is very easy to add a scheduler, however you should use low ms times with caution

```c#
GameBox.Gb.AddScheduler(new Scheduler(timeInMsToRepeat, onTime: () => {
    // what you want to happpen when it repeats
}, setTime: true by default
 /*if set time is false then the scheduler will execute the onTime event on Init*/))
```

the reason you should caution about low timeInMsToRepeat times is because it might not be fast enough to hit it again

it is recommended to go above 50