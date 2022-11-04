using System.Collections.Generic;
using System.Numerics;
using RayWrapper.Base;
using RayWrapper.Objs;
using RayWrapper.Objs.ListView;
using RayWrapperTester.Example_Setup;

namespace RayWrapperTester.Examples;

[Example("CheckBox, ListView & DropDown")]
public class ListViewDropDown : Example
{
    public ListViewDropDown(string tabName) : base(tabName)
    {
        var arr = new List<string>
        {
            "start", "1", "2", "22", "hi", "bye", "no", "u", "yeet", "8", "not 10", "double 1", "yes", "no", "e",
            "i", "o", "u", "a", "werwer", "letters", "words", "end"
        };

        DefaultListItem defItem = new(500, () => arr.Count, i => arr[i])
            { onClick = (i, _) => Logger.Log($"{i}: {arr[i]}") };

        var listViewTest = new ListView(new Vector2(40, 100), defItem, 12);
        
        var dropDownTest = new DropDown(new Vector2(700, 150), "option 1", "option duo", "option non", "option hi",
            "option option", "option setting", "option N");
        
        var checkBox = new Checkbox(new Vector2(800, 80), "Square Check");
        
        Button listViewButton = new(new Vector2(700, 100), "Clear");
        listViewButton.Clicked += () =>
        {
            arr.Clear();
            arr.Add("hi");
        };
        
        RegisterGameObj(listViewTest, listViewButton, dropDownTest, checkBox);
    }
}