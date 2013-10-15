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
        /// <summary>
        /// Arguments of the last KeyDown event.
        /// </summary>
        private KeyEventArgs lastKeyEventArgs;
        /// <summary>
        /// Collection of keys that are holding down at the moment.
        /// </summary>
        private List<Keys> keysDownList;

        /// <summary>
        /// Initializes a new instance of the KeyManager class.
        /// </summary>
        public KeyManager()
        {
            KeyPressed = Keys.None;
            keysDownList = new List<Keys>();
            lastKeyEventArgs = new KeyEventArgs(Keys.None);
        }

        /// <summary>
        /// Gets the count of keys that are holding down at this moment.
        /// </summary>
        public int KeysDownCount
        {
            get
            {
                return keysDownList.Count;
            }
        }

        /// <summary>
        /// Gets or sets the last pressed key.
        /// </summary>
        public Keys KeyPressed  { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        public bool Control
        {
            get
            {
                return lastKeyEventArgs.Control;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        public bool Alt
        {
            get
            {
                return lastKeyEventArgs.Alt;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        public bool Shift
        {
            get
            {
                return lastKeyEventArgs.Shift;
            }
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
        /// Gets the last pressed key and clear the record about that.
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
