using System.Linq;
using System.Numerics;
using RayWrapper;
using RayWrapper.Base;
using RayWrapper.Base.GameObject;
using RayWrapper.Objs;
using RayWrapper.Objs.AlertBoxes;
using RayWrapper.Objs.ListView;
using RayWrapper.Vars;
using RayWrapperTester.Example_Setup;
using static RayWrapperTester.Program;

namespace RayWrapperTester;

public class ExamplesHome : GameObject
{
    public class Test
    {
        public int i = 6;
    }

    public ExamplesHome()
    {
        DefaultListItem listItem = new(565, () => localExamples.Count, i => localExamples[i])
        {
            onClick = (_, s) =>
            {
                if (exampleView.ContainsTab(s))
                {
                    new AlertClose("Already Opened", "That tab is already open!").Show();
                    return;
                }

                exampleView.AddTab(s, ExampleRegister.Examples[s]);
            }
        };

        ListView exampleList = new(new Vector2(20, 60), listItem, 12);

        Text infoText = new(
            "This is the Example Project of Example Projects!\nUse the InputBox to Search:\n\n\nPressing enter or clicking outside the box\nwill execute the search\n\nyou can also press ` to open the console!",
            new Vector2(615, 150));

        InputBox exampleSearch = new(new Vector2(725, 230))
        {
            onEnter = Search,
            onExit = Search
        };

        Button saveTest = new(new Vector2(610, 600), "Test Saving/Loading (look at console)");
        saveTest.Clicked += SaveTesting;

        RegisterGameObj(exampleList, infoText, exampleSearch, saveTest);
    }

    public void Search(string key)
    {
        localExamples = ExampleRegister.Examples.Keys.ToList();
        var fixedKey = key.Replace(" ", "").ToLower();
        if (fixedKey == "") return;
        localExamples = localExamples.Where(s => s.ToLower().Replace(" ", "").Contains(fixedKey)).ToList();
    }
    
    public void SaveTesting()
    {
        var t = new Test();
        t.i = 10;
        GameBox.RegisterSaveItem(t, "test");

        Logger.Log($"i = {t.i}"); // 10
        GameBox.SaveItems();
        
        t.i = 2;
        Logger.Log($"i = {t.i}"); // 2
        GameBox.LoadItems();
        
        Logger.Log($"i = {t.i}"); // 10
        t.Set(new Test());
        Logger.Log($"i = {t.i}"); // 6
        GameBox.LoadItems();
        
        Logger.Log($"i = {t.i}"); // 10
        t = new Test();
        Logger.Log($"i = {t.i}"); // 6 
        GameBox.LoadItems();
        
        Logger.Log($"i = {t.i}"); // 6
    }
}