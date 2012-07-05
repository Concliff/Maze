using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Classes
{
    public enum Directions : uint
    {
        Right   = 0x001,
        Down    = 0x002,
        Left    = 0x004,
        Up      = 0x008,
        None    = 0x010,
    };

    public enum GridMapAttributes : uint
    {
        IsStart     = 0x001,
        IsFinish    = 0x002,
        HasDrop     = 0x004,
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
