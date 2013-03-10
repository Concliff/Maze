using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Maze
{
    public static class SolutionVersion
    {
        private const uint Major    = 0;
        private const uint Minor    = 7;
        private const uint Build    = 10;
        private const uint Revision = 250;


        public static string GetVersion()
        {
            return  Major.ToString() + "." +
                    Minor.ToString() + "." +
                    Build.ToString() + "." +
                    Revision.ToString();

        }
    }
}
