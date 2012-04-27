using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    class SolutionVersion
    {
        private const uint Major    = 0;
        private const uint Minor    = 5;
        private const uint Build    = 0;
        private const uint Revision = 95;


        public static string GetVersion()
        {
            return  Major.ToString() + "." +
                    Minor.ToString() + "." +
                    Build.ToString() + "." +
                    Revision.ToString();

        }
    }
}
