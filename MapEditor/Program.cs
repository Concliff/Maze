using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Maze.Classes;
using MapEditor.Forms;

namespace MapEditor
{
    static class Program
    {
        private static Editor editorForm;
        /// <summary>
        /// Gets an Editor Form instance.
        /// </summary>
        public static Editor EditorForm
        {
            get
            {
                return Program.editorForm;
            }
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Load Pictures
            PictureManager.Load();

            editorForm = new Editor();

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(editorForm);
        }
    }
}
