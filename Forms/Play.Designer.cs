using Maze.Classes;
using System.Windows.Forms;
using System.Drawing;

namespace Maze.Forms
{
    partial class Play
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GridMapPB = new System.Windows.Forms.PictureBox();
            this.SystemTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.RightPanelPB = new System.Windows.Forms.PictureBox();
            this.ScoresPB = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.GridMapPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightPanelPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScoresPB)).BeginInit();
            this.SuspendLayout();
            // 
            // SystemTimer
            // 
            this.SystemTimer.Tick += new System.EventHandler(this.SystemTimerTick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(150, 0);
            this.pictureBox1.Name = "pictureBox1"; 
            this.pictureBox1.Size = new System.Drawing.Size(550, 60);
            this.pictureBox1.BackColor = Color.Gray;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(150, 410);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(550, 60);
            this.pictureBox2.BackColor = Color.Gray;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(0, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(150, 470);
            this.pictureBox3.BackColor = Color.Gray;
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            // 
            // Play
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 470);
            this.DoubleBuffered = true;
            this.Name = "Play";
            this.Text = "Play";
            this.Load += new System.EventHandler(this.Play_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Play_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.GridMapPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightPanelPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScoresPB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        //////////////
        private string ImageDirectoryPath = GlobalConstants.IMAGES_PATH;
        //////////////

        private void CustomInitialize()
        {
            this.KeyMgr = new KeyManager();

            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyMgr.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyUp);
            this.Shown += new System.EventHandler(Play_Shown);
            this.VisibleChanged += new System.EventHandler(Play_VisibleChanged);


            //
            // GridMapPB
            //
            this.GridMapPB.Location = new System.Drawing.Point(150, 60);
            this.GridMapPB.Name = "GridMapPB";
            this.GridMapPB.Size = new System.Drawing.Size(550, 350);
            this.GridMapPB.Visible = true;
            this.GridMapPB.Paint += new PaintEventHandler(GridMapPB_Paint);

            //
            // ScoresPB
            //
            this.ScoresPB.Location = new System.Drawing.Point(30, 20);
            this.ScoresPB.Name = "Total scores";
            this.ScoresPB.Size = new System.Drawing.Size(100, 25);
            this.ScoresPB.BackColor = Color.Gray;
            this.ScoresPB.Paint += new PaintEventHandler(RightPanelPBPaint);
            //
            // RightPanelPB
            //
            this.RightPanelPB.Location = new System.Drawing.Point(700, 0);
            this.RightPanelPB.Name = "Right Panel";
            this.RightPanelPB.Size = new System.Drawing.Size(150, 470);
            this.RightPanelPB.BackColor = Color.Gray;
            this.RightPanelPB.TabIndex = 7;
            this.RightPanelPB.TabStop = false;
            this.RightPanelPB.Paint += new PaintEventHandler(RightPanelPBPaint);

            GridMapGraphic = new GridMapGraph[GlobalConstants.GRIDMAP_WIDTH, GlobalConstants.GRIDMAP_HEIGHT];
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    GridMapGraphic[i, j] = new GridMapGraph();
                    //GridMapGraphic[i, j].Graphic = Graphics.FromHwnd(this.GridMapPB.Handle);
                }

            ////
            // Menu object PictureBoxes
            ////
            PausePB = new PictureBox();

            MenuNewGamePB = new PictureBox();
            MenuContinueGamePB = new PictureBox();
            MenuQuitPB = new PictureBox();
            PauseResumePB = new PictureBox();
            PauseMainMenuPB = new PictureBox();

            MenuNewGamePB.Name = "New Game";
            MenuContinueGamePB.Name = "Continue";
            MenuQuitPB.Name = "Quit";
            PauseResumePB.Name = "Resume";
            PauseMainMenuPB.Name = "Main Menu";

            PausePB.Hide();
            PausePB.BackColor = MenuNewGamePB.BackColor = MenuContinueGamePB.BackColor =
                MenuQuitPB.BackColor = PauseResumePB.BackColor = PauseMainMenuPB.BackColor = Color.Gray;
            // Hide All Menu (Show only ones that needed)
            MenuNewGamePB.Hide();
            PausePB.Hide();
            MenuContinueGamePB.Hide();
            MenuQuitPB.Hide();
            PauseResumePB.Hide();
            PauseMainMenuPB.Hide();

            MenuNewGamePB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            MenuContinueGamePB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            MenuQuitPB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            PauseResumePB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            PauseMainMenuPB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);

            MenuNewGamePB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            MenuContinueGamePB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            MenuQuitPB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            PauseResumePB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            PauseMainMenuPB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);

            MenuNewGamePB.Click += new System.EventHandler(MenuItemClick);
            MenuContinueGamePB.Click += new System.EventHandler(MenuItemClick);
            MenuQuitPB.Click += new System.EventHandler(MenuItemClick);
            PauseResumePB.Click += new System.EventHandler(MenuItemClick);
            PauseMainMenuPB.Click += new System.EventHandler(MenuItemClick);

            MenuNewGamePB.Paint += new PaintEventHandler(MenuItemPaint);
            MenuContinueGamePB.Paint += new PaintEventHandler(MenuItemPaint);
            MenuQuitPB.Paint += new PaintEventHandler(MenuItemPaint);
            PauseResumePB.Paint += new PaintEventHandler(MenuItemPaint);
            PauseMainMenuPB.Paint += new PaintEventHandler(MenuItemPaint);

            // Size
            MenuNewGamePB.Size = MenuContinueGamePB.Size = MenuQuitPB.Size =
                PauseResumePB.Size = PauseMainMenuPB.Size = new Size(150, 20);
            PausePB.Size = new Size(200, 200);

            PausePB.Location = new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - PausePB.Size.Width / 2,
                GridMapPB.Location.Y + GridMapPB.Size.Height / 2 - PausePB.Size.Height / 2);

            // Pause Menu Location
            PauseResumePB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - PauseResumePB.Size.Width / 2, GridMapPB.Location.Y + GridMapPB.Size.Height / 2 - 20);

            PauseMainMenuPB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - PauseMainMenuPB.Size.Width / 2, GridMapPB.Location.Y + GridMapPB.Size.Height / 2);

            byte MenuItemIterator;
            // Main menu Location
            MenuItemIterator = 1;
            MenuNewGamePB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - MenuNewGamePB.Size.Width / 2, GridMapPB.Location.Y + 40 * MenuItemIterator);

            MenuItemIterator = 2;
            MenuContinueGamePB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - MenuContinueGamePB.Size.Width / 2, GridMapPB.Location.Y + 40 * MenuItemIterator);

            MenuItemIterator = 3;
            MenuQuitPB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - MenuQuitPB.Size.Width / 2, GridMapPB.Location.Y + 40 * MenuItemIterator);

            MenuFont = new Font("Arial", 16);
            MenuUnselectedBrush = new SolidBrush(Color.White);
            MenuSelectedBrush = new SolidBrush(Color.Red);

        }

        private void AddControlsOrder()
        {
            // Add every control in the specific order
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.RightPanelPB);
            this.Controls.Add(this.ScoresPB);
            this.Controls.Add(this.pictureBox3);

            //Menu
            this.Controls.Add(this.MenuNewGamePB);
            this.Controls.Add(this.MenuContinueGamePB);
            this.Controls.Add(this.MenuQuitPB);
            this.Controls.Add(this.PauseResumePB);
            this.Controls.Add(this.PauseMainMenuPB);

            // need rework layot reapointing for PausePB
            //this.Controls.Add(this.PausePB);
            this.Controls.Add(this.GridMapPB);


        }

        private KeyManager KeyMgr;
        private System.Windows.Forms.PictureBox GridMapPB;
        private System.Windows.Forms.Timer SystemTimer;
        public  System.Windows.Forms.Label label1;
        private PictureBox ScoresPB;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox RightPanelPB;

        private GridMapGraph[,] GridMapGraphic;

        // Menu objects
        private PictureBox MenuNewGamePB;
        private PictureBox MenuContinueGamePB;
        private PictureBox MenuQuitPB;
        private PictureBox PausePB;
        private PictureBox PauseResumePB;
        private PictureBox PauseMainMenuPB;
        private Font MenuFont;
        private Brush MenuUnselectedBrush;
        private Brush MenuSelectedBrush;
    }
}
