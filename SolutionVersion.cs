using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Maze
{
    public static class SolutionVersion
    {
        private const uint Major    = 0;
        private const uint Minor    = 6;
        private const uint Build    = 1;
        private const uint Revision = 233;


        public static string GetVersion()
        {
            return  Major.ToString() + "." +
                    Minor.ToString() + "." +
                    Build.ToString() + "." +
                    Revision.ToString();

        }
    }
}
