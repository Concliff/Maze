using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;


namespace Maze.Classes
{
    public class KeyManager
    {
        public int KeysDownCount { get; private set; }
        public Keys KeyPressed  { get; private set; }

        private bool IsAltDown;
        private bool IsControlDown;
        private bool IsShiftDown;
        //private KeyEventArgs LastArg;
        private ArrayList KeysDownList;

        public KeyManager()
        {
            KeysDownCount = 0;
            KeyPressed = Keys.None;
            KeysDownList = new ArrayList();
            IsAltDown = false;
            IsControlDown = false;
            IsShiftDown = false;
        }

        public void EventKeyDown(object sender, KeyEventArgs e)
        {
            if (KeysDownList.Contains(e.KeyCode))
                return;

            //LastArg = e;
            KeyPressed = e.KeyCode;
            IsShiftDown = e.Shift;
            IsAltDown = e.Alt;
            IsControlDown = e.Control;
            KeysDownList.Add(e.KeyCode);
            ++KeysDownCount;
            
        }

        public void EventKeyUp(object sender, KeyEventArgs e)
        {
            KeysDownList.Remove(e.KeyCode);
            --KeysDownCount;
        }

        public void EventKeyPress(object sender, KeyPressEventArgs e)
        {
            // TO DO
        }

        public Keys KeyDown(int Number) { return (Keys)KeysDownList[Number]; }

        public Keys ExtractKeyPressed()
        {
            Keys KeyToReturn;
            KeyToReturn = KeyPressed;
            KeyPressed = Keys.None;
            return KeyToReturn;
        }

        public bool Control()
        {
            //return LastArg.Control;
            return IsControlDown;
        }
            
    }
}
