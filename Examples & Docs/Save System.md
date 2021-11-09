the built in save system is (should be) very trust worthy

```C#
using static GameBox; // do this so you dont need to do GameBox.Gb all the time

new GameBox(new Program(), new Vector2(window width, window height, "title"))

public class Program : GameLoop 
{
    public override void Init() 
    {
        Gb.InitSaveSystem("developer name", "app name");
        
        // for every object you want to save
        // its best to nest save objects to not create ALOT of files
        Gb.RegisterSaveItem(obj, "file name");
    }
    
    public override void UpdateLoop()
    {
    }
    
    public override void RenderLoop()
    {
    }
}
```

to save/load items do:
make sure to execute AFTER the save items are registered in `Init()`

```c#
// load
Gb.LoadItems();

// save
Gb.SaveItem();
```

adding encryption is very easy to add
you should put this before the init of gamebox if you want to set
the encryption

```C#
ISave.Cypher = (s => /*this will be encryption, it will give you a string and it wants the encrypted string back*/,
    s => /*this will the the decryption, it will give the encrypted string and it wants the decrypted string*/); 
```

the encryption will do a check to see if the encryption is valid or not