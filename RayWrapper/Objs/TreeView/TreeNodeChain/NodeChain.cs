using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs.TreeView.TreeNodeChain
{
    public class NodeChain
    {
        public ColorModule lineColor = new(0);
        public ColorModule completedLineColor = new(255);
        public bool nonSequential = false;
        public List<NodeShape.NodeShape> nodes = new();
        public NodeShape.NodeShape branched;

        private List<NodeChain> branches = new();

        public NodeChain(params NodeShape.NodeShape[] nodes) => this.nodes.AddRange(nodes);

        public string Draw(Vector2 off, float scale, bool startContext = true)
        {
            var tool = "";
            var context = true;
            var branchedN = branches.Select(b => (b.branched, b)).ToArray();
            Dictionary<NodeShape.NodeShape, List<NodeShape.NodeShape>> nbs = new();
            if (branchedN.Any())
            {
                foreach (var (ns, b) in branchedN)
                {
                    if (!b.nodes.Any()) continue;
                    if (nbs.ContainsKey(ns)) nbs[ns].Add(b.nodes[0]);
                    else nbs.Add(ns, new List<NodeShape.NodeShape> { b.nodes[0] });
                }
            }

            for (var i = 1; i < nodes.Count; i++)
            {
                var nxt = i != nodes.Count - 1 && (bool)nodes[i + 1].completed;
                if (i == 1)
                {
                    nodes[i - 1].Update(off, scale, !nonSequential, startContext, nodes[i].completed);
                    context = nodes[i - 1].completed;
                }

                if (nbs.ContainsKey(nodes[i]))
                    nxt = nxt || nbs[nodes[i]].Select(n => (bool)n.completed).Aggregate((b1, b2) => b1 || b2);
                nodes[i].Update(off, scale, !nonSequential, context, nxt);
                context = nodes[i].completed;
            }

            void UpdateString(string s) => tool = s != "" ? s : tool;
            var centers = nodes.Select(n => (n.Center(off, scale), n.completed)).ToArray();
            foreach (var nodeChain in branches) UpdateString(nodeChain.Draw(off, scale));
            if (!nodes.Any()) return tool;
            if (nodes.Count == 1)
            {
                var n = nodes[0];
                n.Update(off, scale, !nonSequential, branched is null ? true : branched.completed, false);
                if (branched is not null)
                {
                    if (!branched.completed && !nonSequential) return tool;
                    var c = branched.Center(off, scale);
                    c.DrawLine(n.Center(off, scale), n.completed ? completedLineColor : lineColor, .25f * scale);
                }

                UpdateString(n.Draw(off, scale));
                return tool;
            }

            for (var i = 1; i < centers.Length; i++)
            {
                var (c1, cV) = centers[i - 1];
                if (branched is not null && i == 1)
                {
                    if (!branched.completed && !nonSequential) break;
                    var c = branched.Center(off, scale);
                    c.DrawLine(c1, cV ? completedLineColor : lineColor, .25f * scale);
                }

                if (!cV && !nonSequential)
                {
                    UpdateString(nodes[i - 1].Draw(off, scale));
                    if (!nonSequential) break;
                }

                var (c2, cB) = centers[i];
                c1.DrawLine(c2, cB ? completedLineColor : lineColor, .25f * scale);
                UpdateString(nodes[i].Draw(off, scale));
                if (cV) UpdateString(nodes[i - 1].Draw(off, scale));
            }

            if (nonSequential) UpdateString(nodes[centers.Length - 1].Draw(off, scale));

            return tool;
        }

        public void Add(NodeChain nc) => nodes.AddRange(nc.nodes);
        public void Add(NodeShape.NodeShape ns) => nodes.Add(ns);

        public void AddBranch(NodeChain nc)
        {
            var brancher = nodes[^1];
            nc.branched = brancher;
            branches.Add(nc);
        }
    }
}