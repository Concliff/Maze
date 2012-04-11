using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Forms;

namespace Maze.Classes
{
    public class TimeControl
    {
        private Form PlayForm;
        System.Timers.Timer SystemTimer;

        public TimeControl(Play PlayForm)
        {
            this.PlayForm = PlayForm;
            SystemTimer = new System.Timers.Timer(GlobalConstants.TIMER_TICK_IN_MS);
            SystemTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.SystemTimerTick);
            SystemTimer.Start();
        }
        private void SystemTimerTick(object sender, EventArgs e)
        {
            //PlayForm.SystemTimerTick(sender, e);
        }
    }
}
