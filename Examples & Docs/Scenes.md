RayWrapper has an object called `SceneMod`

normally you would have to make a new `GameObject` for a 'game page', and render it

`GameObject` has a lot of overrides, to compensate for this need I added `SceneMod`

`SceneMod` is a modification of `Scene` so it can be a `GameObject`

`SceneMod` is an abstract class, so you can add it to your 'game pages' and then use `RegisterGameObj` with it

`SceneMod` has its own `RegisterGameObj`, it also has an optional `Update` and `Render` loops you can override if need be  