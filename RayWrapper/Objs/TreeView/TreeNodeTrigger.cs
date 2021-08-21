using System;
using System.Collections.Generic;

namespace RayWrapper.Objs.TreeView
{
    public class TreeNodeTrigger
    {
        public Func<bool> funcTest;
        public string prevTest;

        public bool Invoke(Dictionary<string, TreeNode> nodes) =>
            funcTest?.Invoke() ?? prevTest is not null && nodes[prevTest].isComplete.Invoke(nodes);

        public static implicit operator TreeNodeTrigger(string prevToTest) => new() { prevTest = prevToTest };
        public static implicit operator TreeNodeTrigger(Func<bool> func) => new() { funcTest = func };
        public static implicit operator TreeNodeTrigger(bool staticBool) => new() { funcTest = () => staticBool };
    }
}