using Maze.Classes;
using System.Windows.Forms;

namespace Maze.Forms
{
    partial class MapEditor
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
            this.SuspendLayout();
            // 
            // MapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "MapEditor";
            this.Text = "Ally Map Editor";
            this.Load += new System.EventHandler(this.MapEditor_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private void CustomInitialize()
        {
            this.Size = new System.Drawing.Size
                ((GlobalConstants.GRIDMAP_BLOCK_WIDTH + 1) * (GlobalConstants.GRIDMAP_WIDTH - 1),
                (GlobalConstants.GRIDMAP_BLOCK_HEIGHT + 1) * (GlobalConstants.GRIDMAP_HEIGHT - 1));
            this.FormClosing += new FormClosingEventHandler(MapEditorFormClosing);

            this.KeyMgr = new KeyManager();
            this.PlayerPB = new PictureBox();

            this.PlayerPB.Size = new System.Drawing.Size(8, 8);
            PlayerPB.BackColor = System.Drawing.Color.Red;
            this.PlayerPB.Location = new System.Drawing.Point
                ((this.Size.Width - this.PlayerPB.Size.Width) / 2 - 25, (this.Size.Height - this.PlayerPB.Size.Height) / 2 -25);
            this.PlayerPB.Name = "PlayerPB";
            this.PlayerPB.TabIndex = 1;
            this.PlayerPB.TabStop = false;
            this.Controls.Add(this.PlayerPB);

            this.FinishPB = new PictureBox();
            this.FinishPB.Size = new System.Drawing.Size(40, 40);
            this.FinishPB.Image = GetWorldMap().FinishImage;
            this.FinishPB.Location = new System.Drawing.Point(-100, -100);
            this.Controls.Add(this.FinishPB);

            this.StartPB = new PictureBox();
            this.StartPB.Size = new System.Drawing.Size(40, 40);
            this.StartPB.Image = GetWorldMap().StartImage;
            this.StartPB.Location = new System.Drawing.Point(-100, -100);
            this.Controls.Add(this.StartPB);

            this.CoinPB = new PictureBox();
            this.CoinPB.Size = new System.Drawing.Size(20, 30);
            this.CoinPB.Image = GetWorldMap().CoinImage;
            this.CoinPB.Location = new System.Drawing.Point(-100, -100);
            this.Controls.Add(this.CoinPB);
            
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyMgr.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyUp);

            this.GridMapArray = new System.Windows.Forms.PictureBox[GlobalConstants.GRIDMAP_WIDTH, GlobalConstants.GRIDMAP_HEIGHT];
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    this.GridMapArray[i, j] = new System.Windows.Forms.PictureBox();
                    this.GridMapArray[i, j].Name = "GridMapArray[" + i.ToString() + ", " + j.ToString() + "]";
                    this.GridMapArray[i, j].Size = new System.Drawing.Size(GlobalConstants.GRIDMAP_BLOCK_WIDTH, GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
                    this.GridMapArray[i, j].TabIndex = 0;
                    this.GridMapArray[i, j].TabStop = false;
                    this.GridMapArray[i, j].BackColor = System.Drawing.Color.Purple;
                    this.GridMapArray[i, j].Click += new System.EventHandler(BlockClick);
                    this.Controls.Add(this.GridMapArray[i, j]);
                    ((System.ComponentModel.ISupportInitialize)(this.GridMapArray[i, j])).EndInit();
                }
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                    this.Controls.Add(this.GridMapArray[i, j]);

            

        }

        KeyManager KeyMgr;
        private System.Windows.Forms.PictureBox[,] GridMapArray;
        private System.Windows.Forms.PictureBox PlayerPB;
        private System.Windows.Forms.PictureBox FinishPB;
        private System.Windows.Forms.PictureBox StartPB;
        private System.Windows.Forms.PictureBox CoinPB;

    }
}