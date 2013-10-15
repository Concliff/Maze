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
        public static Editor EditorForm;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Load Datas
            DBStores.Load();

            // Load Pictures
            PictureManager.Load();

            Map.Instance.SetMap(0, 0);

            EditorForm = new Editor();

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(EditorForm);
        }
    }
}
