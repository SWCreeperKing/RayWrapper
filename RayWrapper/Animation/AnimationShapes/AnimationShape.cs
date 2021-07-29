using System;
using System.Numerics;

namespace RayWrapper.Animation.AnimationShapes
{
    public abstract class AnimationShape
    {
        public string id;
        public Vector2 pos;
        public Vector2 size;
        public bool isVisible = true;

        public AnimationShape endState;

        protected AnimationShape(string id) => this.id = id;

        public void Update(long deltaTime, float timeFactor, string op, string[] args, bool snap)
        {
            try
            {
                Vector2 v2;
                Vector2 aV2;
                switch (op.ToLower())
                {
                    case "move":
                        v2 = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
                        if (snap) endState.pos += v2;
                        else pos += v2;
                        break;
                    case "slide":
                        v2 = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
                        aV2 = new Vector2(Math.Abs(v2.X), Math.Abs(v2.Y));
                        if (snap)
                        {
                            endState.pos += v2;
                            endState.size += aV2;
                        }
                        else
                        {
                            pos += v2;
                            size += aV2;
                        }

                        break;

                    case "slip":
                        v2 = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
                        aV2 = new Vector2(-Math.Abs(v2.X), -Math.Abs(v2.Y));
                        if (snap)
                        {
                            endState.pos += v2;
                            endState.size += aV2;
                        }
                        else
                        {
                            pos += v2;
                            size += aV2;
                        }

                        break;
                    case "grow" or "shrink":
                        v2 = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
                        switch (snap)
                        {
                            case true when op == "shrink":
                                endState.size -= v2;
                                break;
                            case true:
                                endState.size += v2;
                                break;
                            default:
                            {
                                if (op == "shrink") size -= v2;
                                else size += v2;
                                break;
                            }
                        }

                        break;
                    case "vistog":
                        endState.isVisible = !endState.isVisible;
                        isVisible = !isVisible;
                        break;
                    case "visset":
                        isVisible = bool.Parse(args[0]);
                        endState.isVisible = bool.Parse(args[0]);
                        break;
                    default:
                        MiscOp(deltaTime, timeFactor, op, args, snap);
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine(
                    $"Err: operation `{op}` with args: [{string.Join(",", args)}] has parsed incorrectly");
            }
        }

        public void NewState() => endState = CopyState();

        public abstract void DrawShape();
        public abstract AnimationShape CopyState();

        public virtual void MiscOp(float deltaTime, float timeFactor, string op, string[] args, bool snap)
        {
        }
    }
}