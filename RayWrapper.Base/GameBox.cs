using System.Numerics;

namespace RayWrapper.Base;

public class GameBox
{
    public static GameBox Gb { get; private set; } // singleton

    public GameBox(Scene gameScene, Vector2 windowSize)
    {
        if (Gb is not null) throw new Exception("Only one GameBox can be initialized at a time");
        Gb = this;
    }
}