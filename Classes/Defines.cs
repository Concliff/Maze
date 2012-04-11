using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum Directions : byte
    {
        Right = 1,
        Down = 2,
        Left = 3,
        Up = 4,
        None = 5,
    };

    public enum Attributes : byte
    {
        IsStart = 1,
        IsFinish = 2,
        IsTeleport = 3,
        Attribute4,
        Attribute5,
        Attribute6,
        Attribute7,
        Attribute8,
        Attribute9,
        Attribute10,
    }

    public enum WorldNextAction
    {
        None,
        StartGame,
        GamePlay,
        MapEdit,
        ApplicationQuit,
    };
}
