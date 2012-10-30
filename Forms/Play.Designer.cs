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

            SystemTimer.Stop();

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
            this.SystemTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.TopPanelPB = new System.Windows.Forms.PictureBox();
            this.BottomPanelPB = new System.Windows.Forms.PictureBox();
            this.LeftPanelPB = new System.Windows.Forms.PictureBox();
            this.RightPanelPB = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.TopPanelPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BottomPanelPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LeftPanelPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightPanelPB)).BeginInit();
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
            // Play
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 530);
            this.DoubleBuffered = true;
            this.Name = "Play";
            this.Text = "Play";
            this.Load += new System.EventHandler(this.Play_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Play_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.TopPanelPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BottomPanelPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LeftPanelPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightPanelPB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private void CustomInitialize()
        {
            this.KeyMgr = new KeyManager();

            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyMgr.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyUp);
            this.Shown += new System.EventHandler(Play_Shown);
            this.VisibleChanged += new System.EventHandler(Play_VisibleChanged);
            this.Deactivate += new System.EventHandler(this.KeyMgr.EventFormLostFocus);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);


            //
            // GridMapPB
            //
            this.GridMapPB = new System.Windows.Forms.PictureBox();
            this.GridMapPB.Location = new System.Drawing.Point(150, 90);
            this.GridMapPB.Name = "GridMapPB";
            this.GridMapPB.Size = new System.Drawing.Size(550, 350);
            this.GridMapPB.Visible = true;
            this.GridMapPB.Paint += new PaintEventHandler(GridMapPB_Paint);

            //
            // RightPanelPB
            //
            this.RightPanelPB.Location = new System.Drawing.Point(700, 0);
            this.RightPanelPB.Name = "Right Panel";
            this.RightPanelPB.Size = new System.Drawing.Size(150, this.Size.Height);
            this.RightPanelPB.BackColor = Color.Gray;
            this.RightPanelPB.TabIndex = 7;
            this.RightPanelPB.TabStop = false;
            this.RightPanelPB.Paint += new PaintEventHandler(RightPanelPB_Paint);
            //
            // LeftPanelPB
            //
            this.LeftPanelPB.Location = new System.Drawing.Point(0, 0);
            this.LeftPanelPB.Name = "pictureBox3";
            this.LeftPanelPB.Size = new System.Drawing.Size(150, this.Size.Height);
            this.LeftPanelPB.BackColor = Color.Gray;
            this.LeftPanelPB.TabIndex = 6;
            this.LeftPanelPB.TabStop = false;
            this.LeftPanelPB.Paint += new PaintEventHandler(LeftPanelPB_Paint);
            //
            // TopPanelPB
            //
            this.TopPanelPB.Location = new System.Drawing.Point(150, 0);
            this.TopPanelPB.Name = "pictureBox1";
            this.TopPanelPB.Size = new System.Drawing.Size(550, 90);
            this.TopPanelPB.BackColor = Color.Gray;
            this.TopPanelPB.TabIndex = 4;
            this.TopPanelPB.TabStop = false;
            //
            // BottomPanelPB
            //
            this.BottomPanelPB.Location = new System.Drawing.Point(150, 440);
            this.BottomPanelPB.Name = "pictureBox2";
            this.BottomPanelPB.Size = new System.Drawing.Size(550, 90);
            this.BottomPanelPB.BackColor = Color.Gray;
            this.BottomPanelPB.TabIndex = 5;
            this.BottomPanelPB.TabStop = false;

            this.AurasToolTip = new ToolTip();
            this.AurasToolTip.AutoPopDelay = 20000;
            this.AurasToolTip.InitialDelay = 50;
            this.AurasToolTip.ReshowDelay = 500;

            this.AuraIconPB = new PictureBox[5];
            for (int i = 0; i < 5; ++i)
            {
                AuraIconPB[i] = new PictureBox();
                AuraIconPB[i].Location = new Point(TopPanelPB.Location.X + TopPanelPB.Size.Width - 60 * (i + 1) - 50,
                    TopPanelPB.Location.Y + 5);
                AuraIconPB[i].Size = new Size(50, 80);
                AuraIconPB[i].Paint += new PaintEventHandler(AuraIconPB_Paint);
                AuraIconPB[i].BackColor = Color.Gray;
                AuraIconPB[i].Hide();
            }

            this.SpellBarPB = new SpellBarPictureBox[5];
            for (int i = 0; i < MAX_SPELLS_COUNT; ++i)
            {
                SpellBarPB[i] = new SpellBarPictureBox(i + 1);
                SpellBarPB[i].Location = new Point(BottomPanelPB.Location.X + 60 * (i + 1),
                    BottomPanelPB.Location.Y + 10);
                SpellBarPB[i].Size = new Size(50, 50);
                SpellBarPB[i].BackColor = Color.Gray;
                SpellBarPB[i].MouseClick += new MouseEventHandler(SpellBarPB_MouseClick);
                SpellBarPB[i].Hide();
            }

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
            MenuRandomGamePB = new PictureBox();
            MenuHighScoresPB = new PictureBox();
            MenuQuitPB = new PictureBox();
            PauseResumePB = new PictureBox();
            PauseMainMenuPB = new PictureBox();

            MenuNewGamePB.Name = "New Game";
            MenuRandomGamePB.Name = "Random Map";
            MenuHighScoresPB.Name = "High Scores";
            MenuQuitPB.Name = "Quit";
            PauseResumePB.Name = "Resume";
            PauseMainMenuPB.Name = "Main Menu";

            PausePB.Hide();
            PausePB.BackColor = MenuNewGamePB.BackColor = MenuRandomGamePB.BackColor = MenuHighScoresPB.BackColor =
                MenuQuitPB.BackColor = PauseResumePB.BackColor = PauseMainMenuPB.BackColor = Color.Gray;
            // Hide All Menu (Show only ones that needed)
            MenuNewGamePB.Hide();
            PausePB.Hide();
            MenuRandomGamePB.Hide();
            MenuHighScoresPB.Hide();
            MenuQuitPB.Hide();
            PauseResumePB.Hide();
            PauseMainMenuPB.Hide();

            MenuNewGamePB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            MenuRandomGamePB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            MenuHighScoresPB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            MenuQuitPB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            PauseResumePB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);
            PauseMainMenuPB.MouseEnter += new System.EventHandler(MenuItemMouseEnter);

            MenuNewGamePB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            MenuRandomGamePB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            MenuHighScoresPB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            MenuQuitPB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            PauseResumePB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);
            PauseMainMenuPB.MouseLeave += new System.EventHandler(MenuItemMouseLeave);

            MenuNewGamePB.Click += new System.EventHandler(MenuItemClick);
            MenuRandomGamePB.Click += new System.EventHandler(MenuItemClick);
            MenuHighScoresPB.Click += new System.EventHandler(MenuItemClick);
            MenuQuitPB.Click += new System.EventHandler(MenuItemClick);
            PauseResumePB.Click += new System.EventHandler(MenuItemClick);
            PauseMainMenuPB.Click += new System.EventHandler(MenuItemClick);

            MenuNewGamePB.Paint += new PaintEventHandler(MenuItemPaint);
            MenuRandomGamePB.Paint += new PaintEventHandler(MenuItemPaint);
            MenuHighScoresPB.Paint += new PaintEventHandler(MenuItemPaint);
            MenuQuitPB.Paint += new PaintEventHandler(MenuItemPaint);
            PauseResumePB.Paint += new PaintEventHandler(MenuItemPaint);
            PauseMainMenuPB.Paint += new PaintEventHandler(MenuItemPaint);

            // Size
            MenuNewGamePB.Size = MenuRandomGamePB.Size = MenuHighScoresPB.Size = MenuQuitPB.Size =
                PauseResumePB.Size = PauseMainMenuPB.Size = new Size(150, 30);
            PausePB.Size = new Size(200, 200);

            PausePB.Location = new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - PausePB.Size.Width / 2,
                GridMapPB.Location.Y + GridMapPB.Size.Height / 2 - PausePB.Size.Height / 2);

            // Pause Menu Location
            PauseResumePB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - PauseResumePB.Size.Width / 2, GridMapPB.Location.Y + GridMapPB.Size.Height / 2 - 30);

            PauseMainMenuPB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - PauseMainMenuPB.Size.Width / 2, GridMapPB.Location.Y + GridMapPB.Size.Height / 2);

            byte MenuItemIterator;
            // Main menu Location
            MenuItemIterator = 1;
            MenuNewGamePB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - MenuNewGamePB.Size.Width / 2, GridMapPB.Location.Y + 40 * MenuItemIterator);

            MenuItemIterator = 2;
            MenuRandomGamePB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - MenuRandomGamePB.Size.Width / 2, GridMapPB.Location.Y + 40 * MenuItemIterator);

            MenuItemIterator = 3;
            MenuHighScoresPB.Location =
                new Point(GridMapPB.Location.X + GridMapPB.Size.Width / 2 - MenuHighScoresPB.Size.Width / 2, GridMapPB.Location.Y + 40 * MenuItemIterator);

            MenuItemIterator = 4;
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
            for (int i = 0; i < 5; ++i)
                this.Controls.Add(this.SpellBarPB[i]);
            for (int i = 0; i < 5; ++i)
                this.Controls.Add(this.AuraIconPB[i]);
            this.Controls.Add(this.BottomPanelPB);
            this.Controls.Add(this.TopPanelPB);
            this.Controls.Add(this.RightPanelPB);
            this.Controls.Add(this.LeftPanelPB);

            //Menu
            this.Controls.Add(this.MenuNewGamePB);
            this.Controls.Add(this.MenuRandomGamePB);
            this.Controls.Add(this.MenuHighScoresPB);
            this.Controls.Add(this.MenuQuitPB);
            this.Controls.Add(this.PauseResumePB);
            this.Controls.Add(this.PauseMainMenuPB);

            // need rework layot reapointing for PausePB
            //this.Controls.Add(this.PausePB);
            this.Controls.Add(this.GridMapPB);


        }

        public KeyManager KeyMgr;
        private System.Windows.Forms.PictureBox GridMapPB;
        private System.Windows.Forms.Timer SystemTimer;
        public  System.Windows.Forms.Label label1;
        private PictureBox TopPanelPB;
        private PictureBox BottomPanelPB;
        private PictureBox LeftPanelPB;
        private PictureBox RightPanelPB;

        private GridMapGraph[,] GridMapGraphic;

        private PictureBox[] AuraIconPB;
        private SpellBarPictureBox[] SpellBarPB;
        private ToolTip AurasToolTip;

        // Menu objects
        private PictureBox MenuNewGamePB;
        private PictureBox MenuRandomGamePB;
        private PictureBox MenuHighScoresPB;
        private PictureBox MenuQuitPB;
        private PictureBox PausePB;
        private PictureBox PauseResumePB;
        private PictureBox PauseMainMenuPB;
        private Font MenuFont;
        private Brush MenuUnselectedBrush;
        private Brush MenuSelectedBrush;
    }
}
