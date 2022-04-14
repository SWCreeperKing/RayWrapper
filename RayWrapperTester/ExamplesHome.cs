using System.Linq;
using System.Numerics;
using RayWrapper.Objs;
using RayWrapper.Vars;
using RayWrapperTester.Example_Setup;
using static Program;

namespace RayWrapperTester;

public class ExamplesHome : GhostObject
{
    public ExamplesHome()
    {
        DefaultListItem listItem = new(425, () => localExamples.Count, i => localExamples[i])
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
            "This is the Example Project of Example Projects!\nUse the InputBox to Search:\n\n\nPressing enter or clicking outside the box\nwill execute the search",
            new Vector2(475, 150));

        InputBox exampleSearch = new(new Vector2(590, 230))
        {
            onEnter = Search,
            onExit = Search
        };

        RegisterGameObj(exampleList, infoText, exampleSearch);
    }

    public void Search(string key)
    {
        localExamples = ExampleRegister.Examples.Keys.ToList();
        var fixedKey = key.Replace(" ", "").ToLower();
        if (fixedKey == "") return;
        localExamples = localExamples.Where(s => s.ToLower().Replace(" ", "").Contains(fixedKey)).ToList();
    }
}