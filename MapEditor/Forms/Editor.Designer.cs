using Maze.Classes;
using Maze.Forms;
using System.Windows.Forms;

namespace MapEditor.Forms
{
    partial class Editor
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
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "Editor";
            this.Text = "Ally Map Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void CustomInitialize()
        {

            this.KeyMgr = new KeyManager();
            this.PlayerPB = new PictureBox();
            this.pbRightPanel = new PictureBox();
            this.lblCurrentMap = new Label();
            this.levelUpDown = new System.Windows.Forms.NumericUpDown();
            this.lblDescription = new Label();

            this.Size = new System.Drawing.Size
                ((GlobalConstants.CELL_WIDTH) * (GlobalConstants.GRIDMAP_WIDTH - 2),
                (GlobalConstants.CELL_HEIGHT) * (GlobalConstants.GRIDMAP_HEIGHT - 2) + 100);
            this.FormClosing += new FormClosingEventHandler(MapEditorFormClosing);
            this.MouseClick +=new MouseEventHandler(BlockClick);

            this.pbRightPanel.Size = new System.Drawing.Size
                (100 - 2*FormBorderBarSize, this.Size.Height - FormBorderBarSize - FormTitleBarSize);
            this.pbRightPanel.Location = new System.Drawing.Point(this.Size.Width - 100, 0);
            this.pbRightPanel.BackColor = System.Drawing.Color.Gray;

            lblCurrentMap.AutoSize = true;
            lblCurrentMap.Location = new System.Drawing.Point(pbRightPanel.Location.X + 10, 40);
            lblCurrentMap.Text = "Level:";

            lblDescription.AutoSize = true;
            lblDescription.Location = new System.Drawing.Point(pbRightPanel.Location.X + 10, 40);
            lblDescription.Text = "Page Up\n - Next.\nPage Down\n - Previous";

            this.levelUpDown.Location = new System.Drawing.Point(pbRightPanel.Location.X + 10, 70);
            this.levelUpDown.Name = "Level";
            this.levelUpDown.Size = new System.Drawing.Size(50, 20);
            this.levelUpDown.TabIndex = 0;
            this.levelUpDown.ValueChanged += new System.EventHandler(levelUpDown_ValueChanged);
            this.levelUpDown.TabStop = false;

            this.PlayerPB.Size = new System.Drawing.Size(10, 10);
            PlayerPB.BackColor = System.Drawing.Color.Red;
            this.PlayerPB.Location = new System.Drawing.Point
                ((this.Size.Width - pbRightPanel.Size.Width - this.PlayerPB.Size.Width) / 2, 
                (this.Size.Height - this.PlayerPB.Size.Height) / 2 - (FormTitleBarSize - FormBorderBarSize));
            this.PlayerPB.Name = "PlayerPB";
            this.PlayerPB.TabIndex = 1;
            this.PlayerPB.TabStop = false;

            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyMgr.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyUp);

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            // Adding Controls Order

            this.Controls.Add(this.PlayerPB);
            //this.Controls.Add(this.lblCurrentMap);
            //this.Controls.Add(this.levelUpDown);
            this.Controls.Add(lblDescription);
            this.Controls.Add(this.pbRightPanel);

        }

        void levelUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            if (levelUpDown.Value < 0)
                levelUpDown.Value = 0;
            else if (levelUpDown.Value > 100)
                levelUpDown.Value = 100;

            Map.Instance.SetMap(Map.Instance.GetMap(), (int)levelUpDown.Value);
            oPlayer.Position.Location.Level = (int)levelUpDown.Value;

            this.Focus();
        }

        KeyManager KeyMgr;
        private System.Windows.Forms.PictureBox PlayerPB;
        private System.Windows.Forms.PictureBox pbRightPanel;
        private NumericUpDown levelUpDown;
        private Label lblCurrentMap;
        private Label lblDescription;
    }
}
