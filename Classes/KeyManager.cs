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
        public int KeysDownCount
        {
            get
            {
                return keysDownList.Count;
            }
            private set { ;}
        }

        /// <summary>
        /// Returns the last pressed KeyCode
        /// </summary>
        public Keys KeyPressed  { get; private set; }

        /// <summary>
        /// Is Control key down
        /// </summary>
        public bool Control
        {
            get
            {
                return lastKeyEventArgs.Control;
            }
            private set { ;}
        }
        /// <summary>
        /// Is Alt key down
        /// </summary>
        public bool Alt
        {
            get
            {
                return lastKeyEventArgs.Alt;
            }
            private set { ;}
        }
        /// <summary>
        /// Is Shift key down
        /// </summary>
        public bool Shift
        {
            get
            {
                return lastKeyEventArgs.Shift;
            }
            private set { ;}
        }

        private KeyEventArgs lastKeyEventArgs;
        private List<Keys> keysDownList;

        public KeyManager()
        {
            KeyPressed = Keys.None;
            keysDownList = new List<Keys>();
            lastKeyEventArgs = new KeyEventArgs(Keys.None);
        }

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        public void EventKeyDown(object sender, KeyEventArgs e)
        {

            KeyPressed = e.KeyCode;
            lastKeyEventArgs = e;

            // Do not double add the same key
            if (!keysDownList.Contains(e.KeyCode))
                keysDownList.Add(e.KeyCode);
        }

        /// <summary>
        /// Occurs when a key is released
        /// </summary>
        public void EventKeyUp(object sender, KeyEventArgs e)
        {
            keysDownList.Remove(e.KeyCode);
        }

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        public void EventKeyPress(object sender, KeyPressEventArgs e)
        {
            // TODO: remove or something else
        }

        public Keys KeyDown(int Number) { return (Keys)keysDownList[Number]; }
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

        /// <summary>
        /// Clear All Keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventFormLostFocus(object sender, EventArgs e)
        {
            KeyPressed = Keys.None;
            keysDownList.Clear();
            lastKeyEventArgs = new KeyEventArgs(Keys.None);
        }
    }
}
