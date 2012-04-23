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

            this.KeyMgr = new KeyManager();
            this.PlayerPB = new PictureBox();
            this.pbRightPanel = new PictureBox();
            this.txtCurrentMap = new TextBox();
            this.lblCurrentMap = new Label();
            this.btnMapGo = new Button();

            this.Size = new System.Drawing.Size
                ((GlobalConstants.GRIDMAP_BLOCK_WIDTH) * (GlobalConstants.GRIDMAP_WIDTH - 2),
                (GlobalConstants.GRIDMAP_BLOCK_HEIGHT) * (GlobalConstants.GRIDMAP_HEIGHT - 2) + 100);
            this.FormClosing += new FormClosingEventHandler(MapEditorFormClosing);
            this.MouseClick +=new MouseEventHandler(BlockClick);

            this.pbRightPanel.Size = new System.Drawing.Size(100, this.Size.Height);
            this.pbRightPanel.Location = new System.Drawing.Point(this.Size.Width - 100, 0);
            this.pbRightPanel.BackColor = System.Drawing.Color.Gray;

            lblCurrentMap.AutoSize = true;
            lblCurrentMap.Location = new System.Drawing.Point(pbRightPanel.Location.X + 10, 40);
            lblCurrentMap.Text = "Level:";

            txtCurrentMap.Location = new System.Drawing.Point(pbRightPanel.Location.X + 10, 70);
            txtCurrentMap.Size = new System.Drawing.Size(50, 20);

            btnMapGo.Location = new System.Drawing.Point(pbRightPanel.Location.X + 10, 100);
            btnMapGo.Size = new System.Drawing.Size(50, 20);
            btnMapGo.Text = "Go";
            btnMapGo.Click += new System.EventHandler(btnMapGo_Click);

            this.PlayerPB.Size = new System.Drawing.Size(10, 10);
            PlayerPB.BackColor = System.Drawing.Color.Red;
            this.PlayerPB.Location = new System.Drawing.Point
                ((this.Size.Width - this.PlayerPB.Size.Width) / 2, (this.Size.Height - this.PlayerPB.Size.Height) / 2);
            this.PlayerPB.Name = "PlayerPB";
            this.PlayerPB.TabIndex = 1;
            this.PlayerPB.TabStop = false;

            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyMgr.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyMgr.EventKeyUp);


            GridMapGraphic = new GridMapGraph[GlobalConstants.GRIDMAP_WIDTH, GlobalConstants.GRIDMAP_HEIGHT];
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                    GridMapGraphic[i, j] = new GridMapGraph();

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            // Adding Controls Order

            this.Controls.Add(this.PlayerPB);
            this.Controls.Add(this.lblCurrentMap);
            this.Controls.Add(this.txtCurrentMap);
            this.Controls.Add(this.btnMapGo);
            this.Controls.Add(this.pbRightPanel);

        }

        void btnMapGo_Click(object sender, System.EventArgs e)
        {

        }

        KeyManager KeyMgr;
        private System.Windows.Forms.PictureBox PlayerPB;
        private System.Windows.Forms.PictureBox pbRightPanel;
        private TextBox txtCurrentMap;
        private Label lblCurrentMap;
        private Button btnMapGo;

        private GridMapGraph[,] GridMapGraphic;

    }
}