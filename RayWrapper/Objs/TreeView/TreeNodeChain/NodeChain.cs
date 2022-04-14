using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Objs.TreeView.TreeNodeChain;

public class NodeChain
{
    public ColorModule lineColor = new(0);
    public ColorModule completedLineColor = new(255);
    public List<NodeShape> nodes = new();
    public NodeShape branched;

    private List<string> _tooltipList = new();

    private IDictionary<NodeShape, List<NodeChain>> branches = new Dictionary<NodeShape, List<NodeChain>>();

    public NodeChain(params NodeShape[] nodes) => this.nodes.AddRange(nodes);

    public string Draw(Vector2 off, float scale)
    {
        _tooltipList.Clear();
        Update(off, scale);
        if (branched is not null && !branched.completed) return null;
        void DrawLine(Vector2 c1, Vector2 c2, Color cColor, float scale) => c1.DrawLine(c2, cColor, .25f * scale);
        if (nodes.Count == 1) return nodes[0].Draw(off, scale);

        for (var i = 1; i < nodes.Count; i++)
        {
            var n1 = nodes[i - 1];
            if (!n1.completed)
            {
                _tooltipList.Add(n1.Draw(off, scale));
                break;
            }

            var n2 = nodes[i];
            var n1C = n1.Center(off, scale);
            DrawLine(n1C, n2.Center(off, scale), n2.completed ? completedLineColor : lineColor, scale);
            if (branches.ContainsKey(n1))
            {
                foreach (var branch in branches[n1])
                {
                    if (!nodes.Any()) continue;
                    var n3 = branch.nodes[0];
                    DrawLine(n1C, n3.Center(off, scale), n3.completed ? completedLineColor : lineColor, scale);
                    _tooltipList.Add(branch.Draw(off, scale));
                }
            }

            _tooltipList.Add(n1.Draw(off, scale));
            if (i == nodes.Count - 1) _tooltipList.Add(n2.Draw(off, scale));
        }

        var remain = _tooltipList.Where(i => i is not null).ToArray();
        return !remain.Any() ? null : remain.First();
    }

    public void Update(Vector2 off, float scale)
    {
        if (nodes.Count == 1)
        {
            nodes[0].Update(off, scale, true, false);
            return;
        }

        for (var i = 1; i < nodes.Count; i++)
        {
            var n = nodes[i - 1];
            bool nxt = nodes[i].completed;
            if (branches.ContainsKey(n))
            {
                branches[n].Each(branch => branch.Update(off, scale));
                nxt = nxt || branches[n].Where(b => b.nodes.Any()).Any(b => b.nodes[0].completed);
            }

            n.Update(off, scale, i == 1 ? true : nodes[i - 2].completed, nxt);
            if (i == nodes.Count - 1) nodes[i].Update(off, scale, n.completed, false);
        }
    }

    public void Add(NodeChain nc) => nodes.AddRange(nc.nodes);
    public void Add(NodeShape ns) => nodes.Add(ns);

    public void AddBranch(NodeChain nc, int index = -1)
    {
        index = Math.Clamp(index, -1, nodes.Count - 1);
        var node = nodes[index == -1 ? ^1 : index];
        nc.branched = node;
        if (!branches.ContainsKey(node)) branches.Add(node, new List<NodeChain>());
        branches[node].Add(nc);
    }
}