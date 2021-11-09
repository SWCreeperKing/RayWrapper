discord integration isn't very difficult to use

first you need to init the discord integration

in the `Init()` of your `GameLoop` do the following:

```C#
GameBox.discordAppId = // this should be the app id used for richpresence
GameBox.Gb.InitDiscord();
```
now you need to set discord integration stuff, there are alot of values you can play with for discord integration

```c#
Func<string> details;
Func<string> state;
Func<string> largeImage;
Func<string> largeText;
Func<string> smallImage;
Func<string> smallText;
```


