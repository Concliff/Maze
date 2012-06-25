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

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        public void EventKeyDown(object sender, KeyEventArgs e)
        {
            // If key is pressed already
            if (KeysDownList.Contains(e.KeyCode))
                return;

            KeyPressed = e.KeyCode;
            IsShiftDown = e.Shift;
            IsAltDown = e.Alt;
            IsControlDown = e.Control;
            KeysDownList.Add(e.KeyCode);
            ++KeysDownCount;
            
        }

        /// <summary>
        /// Occurs when a key is released
        /// </summary>
        public void EventKeyUp(object sender, KeyEventArgs e)
        {
            KeysDownList.Remove(e.KeyCode);
            --KeysDownCount;
        }

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        public void EventKeyPress(object sender, KeyPressEventArgs e)
        {
            // TODO: remove or something else
        }

        public Keys KeyDown(int Number) { return (Keys)KeysDownList[Number]; }
        /// <summary>
        /// Receive last pressed key, clear the record about last key pressed
        /// </summary>
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
