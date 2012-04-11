namespace Maze.Forms
{
    partial class BlockEdit
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
            // BlockEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 350);
            this.Name = "BlockEdit";
            this.Text = "Block Edit";
            this.ResumeLayout(false);

        }

        #endregion

        private void CustomInitialize()
        {
            this.BlockIDLabel = new System.Windows.Forms.Label();
            this.BlockIDTextBox = new System.Windows.Forms.TextBox();
            this.LocationLabel = new System.Windows.Forms.Label();
            this.LocationXLabel = new System.Windows.Forms.Label();
            this.LocationYLabel = new System.Windows.Forms.Label();
            this.LocationZLabel = new System.Windows.Forms.Label();
            this.LocationMapLabel = new System.Windows.Forms.Label();
            this.LocationXTextBox = new System.Windows.Forms.TextBox();
            this.LocationYTextBox = new System.Windows.Forms.TextBox();
            this.LocationZTextBox = new System.Windows.Forms.TextBox();
            this.LocationMapTextBox = new System.Windows.Forms.TextBox();

            this.MovementLabel = new System.Windows.Forms.Label();
            this.UpCheckBox = new System.Windows.Forms.CheckBox();
            this.LeftCheckBox = new System.Windows.Forms.CheckBox();
            this.DownCheckBox = new System.Windows.Forms.CheckBox();
            this.RightCheckBox = new System.Windows.Forms.CheckBox();
            this.StartCheckBox = new System.Windows.Forms.CheckBox();
            this.FinishCheckBox = new System.Windows.Forms.CheckBox();

            this.CancelButton = new System.Windows.Forms.Button();
            this.ConfirmButton = new System.Windows.Forms.Button();

            BlockIDLabel.Size = new System.Drawing.Size(80, 20);
            LocationLabel.Size = new System.Drawing.Size(80, 20);
            LocationXLabel.Size = new System.Drawing.Size(80, 20);
            LocationYLabel.Size = new System.Drawing.Size(80, 20);
            LocationZLabel.Size = new System.Drawing.Size(80, 20);
            LocationMapLabel.Size = new System.Drawing.Size(80, 20);
            BlockIDTextBox.Size = new System.Drawing.Size(50, 20);
            LocationXTextBox.Size = new System.Drawing.Size(50, 20);
            LocationYTextBox.Size = new System.Drawing.Size(50, 20);
            LocationZTextBox.Size = new System.Drawing.Size(50, 20);
            LocationMapTextBox.Size = new System.Drawing.Size(50, 20);
            MovementLabel.Size = new System.Drawing.Size(80, 20);
            UpCheckBox.AutoSize = true;
            LeftCheckBox.AutoSize = true;
            DownCheckBox.AutoSize = true;
            RightCheckBox.AutoSize = true;
            ConfirmButton.Size = new System.Drawing.Size(80, 30);
            CancelButton.Size = new System.Drawing.Size(80, 30);

            BlockIDLabel.Location = new System.Drawing.Point(20, 30);
            BlockIDTextBox.Location = new System.Drawing.Point(120, 30);

            LocationLabel.Location = new System.Drawing.Point(20, 60);
            LocationXLabel.Location = new System.Drawing.Point(20, 80);
            LocationYLabel.Location = new System.Drawing.Point(20, 100);
            LocationZLabel.Location = new System.Drawing.Point(20, 120);
            LocationMapLabel.Location = new System.Drawing.Point(20, 140);
            LocationXTextBox.Location = new System.Drawing.Point(120, 80);
            LocationYTextBox.Location = new System.Drawing.Point(120, 100);
            LocationZTextBox.Location = new System.Drawing.Point(120, 120);
            LocationMapTextBox.Location = new System.Drawing.Point(120, 140);

            MovementLabel.Location = new System.Drawing.Point(20, 170);
            UpCheckBox.Location = new System.Drawing.Point(70, 190);
            LeftCheckBox.Location = new System.Drawing.Point(20, 210);
            DownCheckBox.Location = new System.Drawing.Point(70, 230);
            RightCheckBox.Location = new System.Drawing.Point(120, 210);
            StartCheckBox.Location = new System.Drawing.Point(200, 190);
            FinishCheckBox.Location = new System.Drawing.Point(200, 220);

            ConfirmButton.Location = new System.Drawing.Point(50, 300);
            CancelButton.Location = new System.Drawing.Point(150, 300);

            LocationXLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LocationYLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LocationZLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LocationMapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BlockIDTextBox.Enabled = false;
            LocationXTextBox.Enabled = false;
            LocationYTextBox.Enabled = false;
            LocationZTextBox.Enabled = false;
            LocationMapTextBox.Enabled = false;

            ConfirmButton.Click += new System.EventHandler(AcceptButtonClick);
            CancelButton.Click +=new System.EventHandler(CancelButtonClick);

            BlockIDLabel.Text = "ID";
            LocationLabel.Text = "Location:";
            LocationXLabel.Text = "X";
            LocationYLabel.Text = "Y";
            LocationZLabel.Text = "Z";
            LocationMapLabel.Text = "Map";
            MovementLabel.Text = "Allowed Move:";
            UpCheckBox.Text = "Up";
            LeftCheckBox.Text = "Left";
            DownCheckBox.Text = "Down";
            RightCheckBox.Text = "Right";
            StartCheckBox.Text = "Start";
            FinishCheckBox.Text = "Finish";
            ConfirmButton.Text = "Replace";
            CancelButton.Text = "Cancel";

            this.Controls.Add(this.BlockIDLabel);
            this.Controls.Add(this.BlockIDTextBox);

            this.Controls.Add(this.LocationLabel);
            this.Controls.Add(this.LocationXLabel);
            this.Controls.Add(this.LocationYLabel);
            this.Controls.Add(this.LocationZLabel);
            this.Controls.Add(this.LocationMapLabel);
            this.Controls.Add(this.LocationXTextBox);
            this.Controls.Add(this.LocationYTextBox);
            this.Controls.Add(this.LocationZTextBox);
            this.Controls.Add(this.LocationMapTextBox);

            this.Controls.Add(this.MovementLabel);
            this.Controls.Add(this.UpCheckBox);
            this.Controls.Add(this.LeftCheckBox);
            this.Controls.Add(this.DownCheckBox);
            this.Controls.Add(this.RightCheckBox);
            this.Controls.Add(this.StartCheckBox);
            this.Controls.Add(this.FinishCheckBox);

            this.Controls.Add(this.ConfirmButton);
            this.Controls.Add(this.CancelButton);

            //PictureIDTextBox.Focus();
        }

        private System.Windows.Forms.Label BlockIDLabel;
        private System.Windows.Forms.TextBox BlockIDTextBox;
        private System.Windows.Forms.Label LocationLabel;
        private System.Windows.Forms.Label LocationXLabel;
        private System.Windows.Forms.Label LocationYLabel;
        private System.Windows.Forms.Label LocationZLabel;
        private System.Windows.Forms.Label LocationMapLabel;
        private System.Windows.Forms.TextBox LocationXTextBox;
        private System.Windows.Forms.TextBox LocationYTextBox;
        private System.Windows.Forms.TextBox LocationZTextBox;
        private System.Windows.Forms.TextBox LocationMapTextBox;
        private System.Windows.Forms.Label MovementLabel;
        private System.Windows.Forms.CheckBox UpCheckBox;
        private System.Windows.Forms.CheckBox LeftCheckBox;
        private System.Windows.Forms.CheckBox DownCheckBox;
        private System.Windows.Forms.CheckBox RightCheckBox;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.CheckBox StartCheckBox;
        private System.Windows.Forms.CheckBox FinishCheckBox;

    }
}