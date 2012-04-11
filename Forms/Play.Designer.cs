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
            this.PlayerPB = new System.Windows.Forms.PictureBox();
            this.SystemTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.GridMapPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // GridMapPB
            // 
            this.GridMapPB.Location = new System.Drawing.Point(150, 60);
            this.GridMapPB.Name = "GridMapPB";
            this.GridMapPB.Size = new System.Drawing.Size(550, 350);
            this.GridMapPB.TabIndex = 0;
            this.GridMapPB.TabStop = false;
            this.GridMapPB.Visible = false;
            // 
            // PlayerPB
            // 
            this.PlayerPB.Location = new System.Drawing.Point(0, 0);
            this.PlayerPB.Name = "PlayerPB";
            this.PlayerPB.Size = new System.Drawing.Size(100, 50);
            this.PlayerPB.TabIndex = 0;
            this.PlayerPB.TabStop = false;
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
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(700, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(150, 470);
            this.pictureBox4.BackColor = Color.Gray;
            this.pictureBox4.TabIndex = 7;
            this.pictureBox4.TabStop = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.PlayerPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        private void CustomInitialize()
        {
            this.KeyMgr = new KeyManager();

            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyMgr.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyUp);
            this.Shown += new System.EventHandler(Play_Shown);
            // 
            // PlayerPB
            // 
            this.PlayerPB.Size = new System.Drawing.Size(GlobalConstants.PLAYER_SIZE_WIDTH, GlobalConstants.PLAYER_SIZE_HEIGHT);
            this.PlayerPB.BackColor = System.Drawing.Color.Red;
            this.PlayerPB.Location = new System.Drawing.Point
                (this.GridMapPB.Location.X + (this.GridMapPB.Size.Width - this.PlayerPB.Size.Width) / 2, this.GridMapPB.Location.Y + (this.GridMapPB.Size.Height - this.PlayerPB.Size.Height) / 2);
            this.PlayerPB.Name = "PlayerPB";
            this.PlayerPB.TabIndex = 1;
            this.PlayerPB.TabStop = false;

            //
            // Start && Finish
            //
            this.FinishPB = new PictureBox();
            this.StartPB = new PictureBox();
            this.FinishPB.Size = new Size(40, 40);
            this.FinishPB.Image = GetWorldMap().FinishImage;
            this.FinishPB.Location = new Point(-100, -100);
            this.StartPB.Size = new Size(40, 40);
            this.StartPB.Image = GetWorldMap().StartImage;
            this.StartPB.Location = new Point(-100, -100);
            
            /*
            // GridMap
            this.GridMapArray = new System.Windows.Forms.PictureBox[GlobalConstants.GRIDMAP_WIDTH, GlobalConstants.GRIDMAP_HEIGHT];
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    this.GridMapArray[i, j] = new System.Windows.Forms.PictureBox();
                    this.GridMapArray[i, j].Name = "GridMapArray["+i.ToString()+", "+j.ToString()+"]";
                    this.GridMapArray[i, j].Size = new System.Drawing.Size(GlobalConstants.GRIDMAP_BLOCK_WIDTH, GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
                    this.GridMapArray[i, j].TabIndex = 0;
                    this.GridMapArray[i, j].TabStop = false;
                    this.GridMapArray[i, j].BackColor = System.Drawing.Color.Purple;
                    this.Paint += new System.Windows.Forms.PaintEventHandler(this.Play_Paint);
                    this.Controls.Add(this.GridMapArray[i, j]);
                    ((System.ComponentModel.ISupportInitialize)(this.GridMapArray[i, j])).EndInit();
                }
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            //this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            */

            // GridMapGraph
            GridMapGraphic = new GridMapGraph[GlobalConstants.GRIDMAP_WIDTH, GlobalConstants.GRIDMAP_HEIGHT];
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    GridMapGraphic[i, j] = new GridMapGraph();
                    //GridMapGraphic[i, j].Graphic = Graphics.FromHwnd(this.GridMapPB.Handle);
                }
        }


        private void AddControlsOrder()
        {
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PlayerPB);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.FinishPB);
            this.Controls.Add(this.StartPB);
            this.Controls.Add(this.GridMapPB);
            /*for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                    this.Controls.Add(this.GridMapArray[i, j]);*/

        }


        #endregion

        private KeyManager KeyMgr;
        private System.Windows.Forms.PictureBox[,] GridMapArray;
        private System.Windows.Forms.PictureBox PlayerPB;


        private System.Windows.Forms.PictureBox GridMapPB;
        private System.Windows.Forms.Timer SystemTimer;
        public  System.Windows.Forms.Label label1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox FinishPB;
        private PictureBox StartPB;

        private GridMapGraph[,] GridMapGraphic;
    }
}