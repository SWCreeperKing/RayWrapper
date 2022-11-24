﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.SaveSystem;
using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;

new GameBox(new RayWrapperTester.Program(), new Vector2(1280, 720), "Hallo World");

namespace RayWrapperTester
{
    public partial class Program : GameLoop
    {
        public static TabView exampleView;
        public static List<string> localExamples;

        public override void Init()
        {
            // var biggest = Vector2.Zero;
            //
            // unsafe
            // {
            //     for (var i = 0; i < Text.defaultStyle.Font.glyphCount; i++)
            //     {
            //         var v2 = Text.defaultStyle.Font.recs[i].Size();
            //         biggest.X = Math.Max(biggest.X, v2.X);
            //         biggest.Y = Math.Max(biggest.Y, v2.Y);
            //     }
            // }
            //
            // Logger.Log($"biggest in font: [{biggest}]");

            SaveExt.InitSaveSystem("SW_CreeperKing", "SaveTesting");

            // basic encryption
            ISave.Cypher = (s => string.Join("", s.Reverse()), s => string.Join("", s.Reverse()));

            ExampleRegister.Init();
            localExamples = ExampleRegister.Examples.Keys.ToList();

            exampleView = new TabView(Vector2.Zero, GameBox.WindowSize.X);
            exampleView.AddTab("Home", new ExamplesHome());

            RegisterGameObj(exampleView);
        }

        public override void UpdateLoop(float dt)
        {
        }

        public override void RenderLoop()
        {
        }
    }
}