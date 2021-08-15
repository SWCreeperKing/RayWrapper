// using System.Numerics;
// using RayWrapper.TreeViewShapes;
// using RayWrapper.Vars;
//
// namespace RayWrapperTester
// {
//     public class TreeControl : TreeViewControl
//     {
//         public override TreeViewShape[] GetNodes() =>
//             new TreeViewShape[]
//             {
//                 new Box("hi"), new Box("hi2"), new Circle("hi3"), new Circle("hi4"),
//                 new Circle(new Vector2(2, 1), "hi5"), new Line("hi", "hi2") { progression = false },
//                 new Line("hi3", "hi4") { progression = false },
//                 new Line("hi", "lineSpecial", true) { progression = false }
//             };
//
//         public override Vector2 GetPos(string id, string[] carry) =>
//             id switch
//             {
//                 "hi" => new Vector2(1, 1),
//                 "hi2" => new Vector2(1, 3),
//                 "hi3" => new Vector2(3, 1),
//                 "hi4" => new Vector2(3, 3),
//                 "hi5" => new Vector2(4, 8),
//                 "lineSpecial" => new Vector2(4.5f, 8),
//                 _ => Vector2.Zero
//             };
//
//         public override bool GetMarked(string id, string[] carry) =>
//             id switch
//             {
//                 "hi2" or "hi4" => true,
//                 _ => false
//             };
//
//         public override string GetTooltip(string id, string[] carry) =>
//             id switch
//             {
//                 "hi" or "hi2" or "hi3" or "hi4" => id.Replace("hi", "yo"),
//                 "hi5" => "yeet",
//                 _ => ""
//             };
//     }
// }