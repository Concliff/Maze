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
            this.Text = "Maze Map Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void CustomInitialize()
        {
            int rightBarWidth = 250;

            this.Size = new System.Drawing.Size
                ((GlobalConstants.CELL_WIDTH) * 10 + rightBarWidth,
                (GlobalConstants.CELL_HEIGHT) * 10);
            this.FormClosing += Editor_FormClosing;
            this.Load += Editor_Load;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            //this.MaximumSize = Size;
            //this.MinimumSize = Size;

            this.pbMap = new PictureBox();
            this.pbMap.Size = new System.Drawing.Size(this.ClientSize.Width - rightBarWidth, this.ClientSize.Height);/*(this.Size.Width - RightBarWidth - 2 * FormBorderBarSize, this.Size.Height - FormBorderBarSize - FormTitleBarSize)*/;
            this.pbMap.Location = new System.Drawing.Point(0, 0);
            this.pbMap.BackColor = System.Drawing.Color.Gray;
            this.pbMap.Paint += pbMap_Paint;
            this.pbMap.MouseClick += pbMap_MouseClick;
            this.pbMap.MouseDown += pbMap_MouseDown;
            this.pbMap.MouseUp += pbMap_MouseUp;
            this.pbMap.MouseMove += pbMap_MouseMove;

            this.pbRightPanel = new PictureBox();
            this.pbRightPanel.Size = new System.Drawing.Size(rightBarWidth, this.ClientSize.Height);/*(RightBarWidth, this.Size.Height - FormBorderBarSize - FormTitleBarSize);*/
            this.pbRightPanel.Location = new System.Drawing.Point(this.pbMap.Size.Width, 0);
            this.pbRightPanel.BackColor = System.Drawing.Color.Gray;

            // Labels
            this.lblCurrentMap = new Label();
            this.lblCurrentMap.AutoSize = true;
            this.lblCurrentMap.Text = "Map";
            this.lblCurrentMap.Font = new System.Drawing.Font("Arial", 14);
            this.lblCurrentMap.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentMap.ForeColor = System.Drawing.Color.White;

            this.lblMapName = new Label();
            this.lblMapName.AutoSize = true;
            this.lblMapName.Text = "Name";
            this.lblMapName.Font = new System.Drawing.Font("Arial", 14);
            this.lblMapName.BackColor = System.Drawing.Color.Transparent;
            this.lblMapName.ForeColor = System.Drawing.Color.White;

            this.lblCurrentLevel = new Label();
            this.lblCurrentLevel.AutoSize = true;
            this.lblCurrentLevel.Text = "Level";
            this.lblCurrentLevel.Font = new System.Drawing.Font("Arial", 14);
            this.lblCurrentLevel.ForeColor = System.Drawing.Color.White;
            this.lblCurrentLevel.BackColor = System.Drawing.Color.Transparent;

            this.cboCurrentMap = new ComboBox();
            this.cboCurrentMap.Size = new System.Drawing.Size(95, 20);
            this.cboCurrentMap.Font = new System.Drawing.Font("Arial", 10);
            this.cboCurrentMap.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboCurrentMap.SelectedValueChanged += cboCurrentMap_SelectedValueChanged;

            this.tbxMapName = new TextBox();
            this.tbxMapName.Size = new System.Drawing.Size(95, 10);
            this.tbxMapName.Font = new System.Drawing.Font("Arial", 10);
            this.tbxMapName.TextChanged += tbxMapName_TextChanged;

            this.nudCurrentLevel = new NumericUpDown();
            this.nudCurrentLevel.Name = "Current Level";
            this.nudCurrentLevel.Size = new System.Drawing.Size(95, 20);
            this.nudCurrentLevel.Font = new System.Drawing.Font("Arial", 10);
            this.nudCurrentLevel.ValueChanged += new System.EventHandler(nudCurrentLevel_ValueChanged);

            this.btnAddMap = new Button();
            this.btnAddMap.Text = "+";
            this.btnAddMap.Size = new System.Drawing.Size(23, 23);
            this.btnAddMap.Font = new System.Drawing.Font("Arial", 14);
            this.btnAddMap.BackColor = System.Drawing.Color.Transparent;
            this.btnAddMap.Click += btnAddMap_Click;

            this.btnRemoveMap = new Button();
            this.btnRemoveMap.Text = "-";
            this.btnRemoveMap.Size = new System.Drawing.Size(23, 23);
            this.btnRemoveMap.Font = new System.Drawing.Font("Arial", 14);
            this.btnRemoveMap.BackColor = System.Drawing.Color.Transparent;
            this.btnRemoveMap.Click += btnRemoveMap_Click;

            this.btnAddLevel = new Button();
            this.btnAddLevel.Text = "+";
            this.btnAddLevel.Size = new System.Drawing.Size(23, 23);
            this.btnAddLevel.Font = new System.Drawing.Font("Arial", 14);
            this.btnAddLevel.BackColor = System.Drawing.Color.Transparent;
            this.btnAddLevel.Click += btnAddLevel_Click;

            this.btnRemoveLevel = new Button();
            this.btnRemoveLevel.Text = "-";
            this.btnRemoveLevel.Size = new System.Drawing.Size(23, 23);
            this.btnRemoveLevel.Font = new System.Drawing.Font("Arial", 14);
            this.btnRemoveLevel.BackColor = System.Drawing.Color.Transparent;
            this.btnRemoveLevel.Click += btnRemoveLevel_Click;

            this.btnSave = new Button();
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.Text = "Save";
            this.btnSave.Font = new System.Drawing.Font("Arial", 10);
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.Click += btnSave_Click;

            this.lblIsMapSaved = new Label();
            this.lblIsMapSaved.AutoSize = true;
            this.lblIsMapSaved.Text = "";
            this.lblIsMapSaved.Font = new System.Drawing.Font("Arial", 12);
            this.lblIsMapSaved.ForeColor = System.Drawing.Color.White;
            this.lblIsMapSaved.BackColor = System.Drawing.Color.Transparent;

            // ToolTips
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.btnAddMap, "Add New Map");
            toolTip = new ToolTip();
            toolTip.SetToolTip(this.btnRemoveMap, "Remove Selected Map");
            toolTip = new ToolTip();
            toolTip.SetToolTip(this.btnAddLevel, "Add New Level");
            toolTip = new ToolTip();
            toolTip.SetToolTip(this.btnRemoveLevel, "Remove Current Level");

            // Adding Controls Order
            this.Controls.Add(this.lblCurrentMap);
            this.Controls.Add(this.lblMapName);
            this.Controls.Add(this.lblCurrentLevel);
            this.Controls.Add(this.tbxMapName);
            this.Controls.Add(this.cboCurrentMap);
            this.Controls.Add(this.nudCurrentLevel);
            this.Controls.Add(this.btnAddMap);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblIsMapSaved);
            this.Controls.Add(this.pbMap);
            this.Controls.Add(this.pbRightPanel);
        }

        void Editor_Load(object sender, System.EventArgs e)
        {
            // Place controls on the right panel
            // This helps to make Transparent background
            this.lblCurrentMap.Parent = this.pbRightPanel;
            this.lblCurrentMap.Location = new System.Drawing.Point(20, 25);

            this.lblMapName.Parent = this.pbRightPanel;
            this.lblMapName.Location = new System.Drawing.Point(20, 60);

            this.lblCurrentLevel.Parent = this.pbRightPanel;
            this.lblCurrentLevel.Location = new System.Drawing.Point(20, 95);

            this.cboCurrentMap.Parent = this.pbRightPanel;
            this.cboCurrentMap.Location = new System.Drawing.Point(90, 23);

            this.tbxMapName.Parent = this.pbRightPanel;
            this.tbxMapName.Location = new System.Drawing.Point(90, 60);

            this.nudCurrentLevel.Parent = this.pbRightPanel;
            this.nudCurrentLevel.Location = new System.Drawing.Point(90, 95);

            this.btnAddMap.Parent = this.pbRightPanel;
            this.btnAddMap.Location = new System.Drawing.Point(191, 23);

            this.btnRemoveMap.Parent = this.pbRightPanel;
            this.btnRemoveMap.Location = new System.Drawing.Point(218, 23);

            this.btnAddLevel.Parent = this.pbRightPanel;
            this.btnAddLevel.Location = new System.Drawing.Point(191, 95);

            this.btnRemoveLevel.Parent = this.pbRightPanel;
            this.btnRemoveLevel.Location = new System.Drawing.Point(218,95);

            this.btnSave.Parent = this.pbRightPanel;
            this.btnSave.Location = new System.Drawing.Point(this.pbRightPanel.Size.Width - this.btnSave.Size.Width - 9, 140);

            this.lblIsMapSaved.Parent = this.pbRightPanel;
            this.lblIsMapSaved.Location = new System.Drawing.Point(108, 143);
        }

        // Drawing Controls
        private System.Windows.Forms.PictureBox pbRightPanel;
        private System.Windows.Forms.PictureBox pbMap;

        // Standard Controls
        private Label lblMapName;
        private TextBox tbxMapName;
        private Label lblCurrentMap;
        private ComboBox cboCurrentMap;
        private Label lblCurrentLevel;
        private NumericUpDown nudCurrentLevel;
        private Button btnAddMap;
        private Button btnRemoveMap;
        private Button btnAddLevel;
        private Button btnRemoveLevel;
        private Button btnSave;
        private Label lblIsMapSaved;
    }
}
