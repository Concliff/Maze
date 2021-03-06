﻿using Maze.Classes;
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

            this.systemTimer.Stop();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialize all the Form properties and components.
        /// </summary>
        private void Initialize()
        {
            this.components = new System.ComponentModel.Container();

            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 530);
            this.DoubleBuffered = true;
            this.Name = "Maze";
            this.Text = "Maze";
            this.Load += new System.EventHandler(this.Play_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Play_Paint);
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
            // System Timer
            //
            this.systemTimer = new System.Windows.Forms.Timer(this.components);
            this.systemTimer.Tick += new System.EventHandler(this.SystemTimerTick);

            //
            // pbGridMap
            //
            this.pbGridMap = new System.Windows.Forms.PictureBox();
            this.pbGridMap.Location = new System.Drawing.Point(150, 90);
            this.pbGridMap.Name = "GridMap";
            this.pbGridMap.Size = new System.Drawing.Size(550, 350);
            this.pbGridMap.Visible = true;
            this.pbGridMap.Paint += new PaintEventHandler(CellPB_Paint);

            //
            // Right Panel
            //
            this.pbRightPanel = new System.Windows.Forms.PictureBox();
            this.pbRightPanel.Location = new System.Drawing.Point(700, 0);
            this.pbRightPanel.Name = "Right Panel";
            this.pbRightPanel.Size = new System.Drawing.Size(150, this.Size.Height);
            this.pbRightPanel.BackColor = Color.Gray;
            this.pbRightPanel.TabIndex = 7;
            this.pbRightPanel.TabStop = false;
            this.pbRightPanel.Paint += new PaintEventHandler(RightPanelPB_Paint);

            //
            // Left Panel
            //
            this.pbLeftPanel = new System.Windows.Forms.PictureBox();
            this.pbLeftPanel.Location = new System.Drawing.Point(0, 0);
            this.pbLeftPanel.Name = "Left Panel";
            this.pbLeftPanel.Size = new System.Drawing.Size(150, this.Size.Height);
            this.pbLeftPanel.BackColor = Color.Gray;
            this.pbLeftPanel.TabIndex = 6;
            this.pbLeftPanel.TabStop = false;
            this.pbLeftPanel.Paint += new PaintEventHandler(LeftPanelPB_Paint);

            //
            // Top Panel
            //
            this.pbTopPanel = new System.Windows.Forms.PictureBox();
            this.pbTopPanel.Location = new System.Drawing.Point(150, 0);
            this.pbTopPanel.Name = "Top Panel";
            this.pbTopPanel.Size = new System.Drawing.Size(550, 90);
            this.pbTopPanel.BackColor = Color.Gray;
            this.pbTopPanel.TabIndex = 4;
            this.pbTopPanel.TabStop = false;

            //
            // Bottom Panel
            //
            this.pbBottomPanel = new System.Windows.Forms.PictureBox();
            this.pbBottomPanel.Location = new System.Drawing.Point(150, 440);
            this.pbBottomPanel.Name = "Bottom Panel";
            this.pbBottomPanel.Size = new System.Drawing.Size(550, 90);
            this.pbBottomPanel.BackColor = Color.Gray;
            this.pbBottomPanel.TabIndex = 5;
            this.pbBottomPanel.TabStop = false;

            //
            // ToolTips for Auras
            //
            this.toolTipAuras = new ToolTip();
            this.toolTipAuras.AutoPopDelay = 20000;
            this.toolTipAuras.InitialDelay = 50;
            this.toolTipAuras.ReshowDelay = 500;

            this.pbAuraIcons = new PictureBox[5];
            for (int i = 0; i < 5; ++i)
            {
                this.pbAuraIcons[i] = new PictureBox();
                this.pbAuraIcons[i].Location = new Point(this.pbTopPanel.Location.X + this.pbTopPanel.Size.Width - 60 * (i + 1) - 50,
                    this.pbTopPanel.Location.Y + 5);
                this.pbAuraIcons[i].Size = new Size(50, 80);
                this.pbAuraIcons[i].Paint += new PaintEventHandler(pbAuraIcons_Paint);
                this.pbAuraIcons[i].BackColor = Color.Gray;
                this.pbAuraIcons[i].Hide();
            }

            this.pbSpellBars = new SpellBarPictureBox[5];
            for (int i = 0; i < MAX_SPELLS_COUNT; ++i)
            {
                this.pbSpellBars[i] = new SpellBarPictureBox(i + 1);
                this.pbSpellBars[i].Location = new Point(this.pbBottomPanel.Location.X + 60 * (i + 1),
                    pbBottomPanel.Location.Y + 10);
                this.pbSpellBars[i].Size = new Size(50, 50);
                this.pbSpellBars[i].BackColor = Color.Gray;
                this.pbSpellBars[i].MouseClick += new MouseEventHandler(SpellBarPB_MouseClick);
                this.pbSpellBars[i].Hide();
                this.pbSpellBars[i].Paint += new PaintEventHandler(SpellBarPB_Paint);
            }

            //
            // Menu object PictureBoxes
            //
            this.pbPause = new PictureBox();
            this.pbPause.Size = new Size(200, 200);
            this.pbPause.BackColor = Color.Gray;
            this.pbPause.Location = new Point(this.pbGridMap.Location.X + this.pbGridMap.Size.Width / 2 - this.pbPause.Size.Width / 2,
                this.pbGridMap.Location.Y + this.pbGridMap.Size.Height / 2 - this.pbPause.Size.Height / 2);

            this.pbPauseResume = new PictureBox();
            this.pbPauseResume.Size = new Size(150, 30);
            this.pbPauseResume.Location =
                new Point(pbGridMap.Location.X + this.pbGridMap.Size.Width / 2 - this.pbPauseResume.Size.Width / 2, this.pbGridMap.Location.Y + this.pbGridMap.Size.Height / 2 - 30);
            this.pbPauseResume.BackColor = Color.Gray;
            this.pbPauseResume.Name = "Resume";
            this.pbPauseResume.MouseEnter += new System.EventHandler(pbMenuItems_MouseEnter);
            this.pbPauseResume.MouseLeave += new System.EventHandler(pbMenuItems_MouseLeave);
            this.pbPauseResume.Click += new System.EventHandler(MenuItem_Click);
            this.pbPauseResume.Paint += new PaintEventHandler(pbMenuItems_Paint);


            this.pbPauseMainMenu = new PictureBox();
            this.pbPauseMainMenu.Size = new Size(150, 30);
            this.pbPauseMainMenu.Location =
                new Point(pbGridMap.Location.X + this.pbGridMap.Size.Width / 2 - this.pbPauseMainMenu.Size.Width / 2, this.pbGridMap.Location.Y + this.pbGridMap.Size.Height / 2);
            this.pbPauseMainMenu.BackColor = Color.Gray;
            this.pbPauseMainMenu.Name = "Main Menu";
            this.pbPauseMainMenu.MouseEnter += new System.EventHandler(pbMenuItems_MouseEnter);
            this.pbPauseMainMenu.MouseLeave += new System.EventHandler(pbMenuItems_MouseLeave);
            this.pbPauseMainMenu.Click += new System.EventHandler(MenuItem_Click);
            this.pbPauseMainMenu.Paint += new PaintEventHandler(pbMenuItems_Paint);
            
            // Hide All Menu (Show only ones that needed)
            this.pbPause.Hide();
            this.pbPauseResume.Hide();
            this.pbPauseMainMenu.Hide();

            // Fonts and Brushes
            this.fontMenu = new Font("Arial", 16);
            this.brushMenuUnselected = new SolidBrush(Color.White);
            this.brushMenuSelected = new SolidBrush(Color.Red);


            // Add every control in the specific order
            for (int i = 0; i < 5; ++i)
                this.Controls.Add(this.pbSpellBars[i]);
            for (int i = 0; i < 5; ++i)
                this.Controls.Add(this.pbAuraIcons[i]);
            this.Controls.Add(this.pbBottomPanel);
            this.Controls.Add(this.pbTopPanel);
            this.Controls.Add(this.pbRightPanel);
            this.Controls.Add(this.pbLeftPanel);

            // Menu
            this.Controls.Add(this.pbPauseResume);
            this.Controls.Add(this.pbPauseMainMenu);

            // need rework layot reapointing for pbPause
            //this.Controls.Add(this.PausePB);
            this.Controls.Add(this.pbGridMap);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.PictureBox pbGridMap;
        private System.Windows.Forms.Timer systemTimer;
        private PictureBox pbTopPanel;
        private PictureBox pbBottomPanel;
        private PictureBox pbLeftPanel;
        private PictureBox pbRightPanel;

        private PictureBox[] pbAuraIcons;
        private SpellBarPictureBox[] pbSpellBars;
        private ToolTip toolTipAuras;

        private PictureBox[] pbMenuItems;

        // Menu objects
        private PictureBox pbPause;
        private PictureBox pbPauseResume;
        private PictureBox pbPauseMainMenu;
        private Font fontMenu;
        private Brush brushMenuUnselected;
        private Brush brushMenuSelected;
    }
}
