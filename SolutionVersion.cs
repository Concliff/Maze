﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    class SolutionVersion
    {
        private const uint Major    = 0;
        private const uint Minor    = 4;
        private const uint Build    = 4;
        private const uint Revision = 85;


        public static string GetVersion()
        {
            return  Major.ToString() + "." +
                    Minor.ToString() + "." +
                    Build.ToString() + "." +
                    Revision.ToString();

        }
    }
}