the command system is a very powerful system when the ingame console is enabled (which it is by default)

the command system is also very easy to add to

to add to the command system you need to make a class for the commands

```c#
public class [class name] : ICommandModule
{
    // to add a command you need a static method and some attributes
    // the method needs to return a string and accept a string[]
    [Command("example")] // this attribute sets the command name
    [Help("displays message to the help command")] // this is optional 
    public static string ExampleCommand(string[] args 
    /*string[] are the arguments passed into the command*/)
    {
        return "message to the console";
    }
}
```

now inorder to register your commands to the ingame console you need to put this in the `Init()` of your `GameLoop`

`CommandRegister.RegisterCommand<[class name]>();`