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

    public enum GridMapAttributes : uint
    {
        IsStart     = 0x001,
        IsFinish    = 0x002,
        HasCoin     = 0x004,
        Attribute4  = 0x008,
        Attribute5  = 0x010,
        Attribute6  = 0x020,
        Attribute7  = 0x040,
        Attribute8  = 0x080,
        Attribute9  = 0x100,
        Attribute10 = 0x200,
    };

    public enum GridMapOptions : uint
    {
        Portal = 0x001,
        Option2,
        Option3,
        Option4,
        Option5,
        Option6,
        Option7,
        Option8,
        Option9,
        Option10,
    };

    public enum WorldNextAction
    {
        None,
        StartGame,
        GamePlay,
        MapEdit,
        ApplicationQuit,
    };
}
