`Actionable` and `ColorModule` are 2 extremely useful and wide spread types throughout RayWrapper

`Actionable` allows for either a static value OR a function to execute

`ColorModule` is just an extension to `Actionable`

if you just want to set a static, non changing value, thats simple

`Actionable<type> a = new(obj)`

however if you want to add a function, that is also simple but an extra step

```c#
Actionable<type> a = new(() => {
    // do stuff
})
```