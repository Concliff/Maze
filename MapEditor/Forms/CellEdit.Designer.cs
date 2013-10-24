namespace MapEditor.Forms
{
    partial class CellEdit
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
            // CellEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 350);
            this.Name = "CellEdit";
            this.Text = "Cell Edit";
            this.ResumeLayout(false);

        }

        #endregion

        private void CustomInitialize()
        {
            this.KeyPreview = true;
            this.KeyDown += CellEdit_KeyDown;
            //this.Shown += CellEdit_Shown;

            //
            // LABELS
            //
            this.lblBlockId = new System.Windows.Forms.Label();
            this.lblBlockId.Text = "ID";
            this.lblBlockId.Size = new System.Drawing.Size(80, 20);
            this.lblBlockId.Location = new System.Drawing.Point(20, 30);

            this.lblLocation = new System.Windows.Forms.Label();
            this.lblLocation.Text = "Location:";
            this.lblLocation.Size = new System.Drawing.Size(80, 20);
            this.lblLocation.Location = new System.Drawing.Point(20, 60);

            this.lblLocationX = new System.Windows.Forms.Label();
            this.lblLocationX.Text = "X";
            this.lblLocationX.Size = new System.Drawing.Size(80, 20);
            this.lblLocationX.Location = new System.Drawing.Point(20, 80);
            this.lblLocationX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblLocationY = new System.Windows.Forms.Label();
            this.lblLocationY.Text = "Y";
            this.lblLocationY.Size = new System.Drawing.Size(80, 20);
            this.lblLocationY.Location = new System.Drawing.Point(20, 100);
            this.lblLocationY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblLocationZ = new System.Windows.Forms.Label();
            this.lblLocationZ.Text = "Z";
            this.lblLocationZ.Size = new System.Drawing.Size(80, 20);
            this.lblLocationZ.Location = new System.Drawing.Point(20, 120);
            this.lblLocationZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblLocationLevel = new System.Windows.Forms.Label();
            this.lblLocationLevel.Text = "Level";
            this.lblLocationLevel.Size = new System.Drawing.Size(80, 20);
            this.lblLocationLevel.Location = new System.Drawing.Point(20, 140);
            this.lblLocationLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblMovement = new System.Windows.Forms.Label();
            this.lblMovement.Text = "Allowed Move:";
            this.lblMovement.Size = new System.Drawing.Size(80, 20);
            this.lblMovement.Location = new System.Drawing.Point(20, 170);

            this.lblOptionValue = new System.Windows.Forms.Label();
            this.lblOptionValue.Text = "Option Value";
            this.lblOptionValue.Location = new System.Drawing.Point(300, 30);
            this.lblOptionValue.AutoSize = true;

            //
            // TEXTBOXES
            //
            this.tbxBlockId = new System.Windows.Forms.TextBox();
            this.tbxBlockId.Size = new System.Drawing.Size(50, 20);
            this.tbxBlockId.Location = new System.Drawing.Point(120, 30);
            this.tbxBlockId.Enabled = false;

            this.tbxLocationX = new System.Windows.Forms.TextBox();
            this.tbxLocationX.Size = new System.Drawing.Size(50, 20);
            this.tbxLocationX.Location = new System.Drawing.Point(120, 80);
            this.tbxLocationX.Enabled = false;

            this.tbxLocationY = new System.Windows.Forms.TextBox();
            this.tbxLocationY.Size = new System.Drawing.Size(50, 20);
            this.tbxLocationY.Location = new System.Drawing.Point(120, 100);
            this.tbxLocationY.Enabled = false;

            this.tbxLocationZ = new System.Windows.Forms.TextBox();
            this.tbxLocationZ.Size = new System.Drawing.Size(50, 20);
            this.tbxLocationZ.Location = new System.Drawing.Point(120, 120);
            this.tbxLocationZ.Enabled = false;

            this.tbxLocationLevel = new System.Windows.Forms.TextBox();
            this.tbxLocationLevel.Size = new System.Drawing.Size(50, 20);
            this.tbxLocationLevel.Location = new System.Drawing.Point(120, 140);
            this.tbxLocationLevel.Enabled = false;

            this.tbxOptionValue = new System.Windows.Forms.TextBox();
            this.tbxOptionValue.Location = new System.Drawing.Point(380, 30);
            this.tbxOptionValue.Size = new System.Drawing.Size(40, 20);

            //
            // CHECKBOXES
            //
            this.chkUp = new System.Windows.Forms.CheckBox();
            this.chkUp.Text = "Up";
            this.chkUp.AutoSize = true;
            this.chkUp.Location = new System.Drawing.Point(70, 190);
            this.chkUp.TabStop = false;
            this.chkUp.CheckedChanged += chkDirection_CheckedChanged;

            this.chkLeft = new System.Windows.Forms.CheckBox();
            this.chkLeft.Text = "Left";
            this.chkLeft.AutoSize = true;
            this.chkLeft.Location = new System.Drawing.Point(20, 210);
            this.chkLeft.CheckedChanged += chkDirection_CheckedChanged;

            this.chkDown = new System.Windows.Forms.CheckBox();
            this.chkDown.Text = "Down";
            this.chkDown.AutoSize = true;
            this.chkDown.Location = new System.Drawing.Point(70, 230);
            this.chkDown.CheckedChanged += chkDirection_CheckedChanged;

            this.chkRight = new System.Windows.Forms.CheckBox();
            this.chkRight.Text = "Right";
            this.chkRight.AutoSize = true;
            this.chkRight.Location = new System.Drawing.Point(120, 210);
            this.chkRight.CheckedChanged += chkDirection_CheckedChanged;

            this.chkAll = new System.Windows.Forms.CheckBox();
            this.chkAll.Text = "All";
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(70, 210);
            this.chkAll.CheckedChanged += chkAll_CheckedChanged;

            this.chkStart = new System.Windows.Forms.CheckBox();
            this.chkStart.Text = "Start";
            this.chkStart.Location = new System.Drawing.Point(200, 190);

            this.chkFinish = new System.Windows.Forms.CheckBox();
            this.chkFinish.Text = "Finish";
            this.chkFinish.Location = new System.Drawing.Point(200, 220);

            this.chkDrop = new System.Windows.Forms.CheckBox();
            this.chkDrop.Text = "Drop";
            this.chkDrop.Location = new System.Drawing.Point(200, 250);

            this.chkPortal = new System.Windows.Forms.CheckBox();
            this.chkPortal.Text = "Portal";
            this.chkPortal.Location = new System.Drawing.Point(300, 60);

            //
            // BUTTONS
            //
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.Location = new System.Drawing.Point(150, 300);
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);

            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnConfirm.Text = "Replace";
            this.btnConfirm.Size = new System.Drawing.Size(80, 30);
            this.btnConfirm.Location = new System.Drawing.Point(50, 300);
            this.btnConfirm.Click += new System.EventHandler(btnAccept_Click);

            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDelete.Text = "Delete";
            this.btnDelete.Size = new System.Drawing.Size(70, 30);
            this.btnDelete.Location = new System.Drawing.Point(200, 25);
            this.btnDelete.Click += new System.EventHandler(btnDelete_Click);

            // Add Controls to the Form
            this.Controls.Add(this.lblBlockId);
            this.Controls.Add(this.tbxBlockId);

            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblLocationX);
            this.Controls.Add(this.lblLocationY);
            this.Controls.Add(this.lblLocationZ);
            this.Controls.Add(this.lblLocationLevel);
            this.Controls.Add(this.tbxLocationX);
            this.Controls.Add(this.tbxLocationY);
            this.Controls.Add(this.tbxLocationZ);
            this.Controls.Add(this.tbxLocationLevel);

            this.Controls.Add(this.lblMovement);
            this.Controls.Add(this.chkUp);
            this.Controls.Add(this.chkLeft);
            this.Controls.Add(this.chkDown);
            this.Controls.Add(this.chkRight);
            this.Controls.Add(this.chkStart);
            this.Controls.Add(this.chkFinish);
            this.Controls.Add(this.chkDrop);
            this.Controls.Add(this.chkAll);

            this.Controls.Add(this.lblOptionValue);
            this.Controls.Add(this.tbxOptionValue);
            this.Controls.Add(this.chkPortal);

            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDelete);
        }

        private System.Windows.Forms.Label lblBlockId;
        private System.Windows.Forms.TextBox tbxBlockId;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblLocationX;
        private System.Windows.Forms.Label lblLocationY;
        private System.Windows.Forms.Label lblLocationZ;
        private System.Windows.Forms.Label lblLocationLevel;
        private System.Windows.Forms.TextBox tbxLocationX;
        private System.Windows.Forms.TextBox tbxLocationY;
        private System.Windows.Forms.TextBox tbxLocationZ;
        private System.Windows.Forms.TextBox tbxLocationLevel;
        private System.Windows.Forms.Label lblMovement;
        private System.Windows.Forms.CheckBox chkUp;
        private System.Windows.Forms.CheckBox chkLeft;
        private System.Windows.Forms.CheckBox chkDown;
        private System.Windows.Forms.CheckBox chkRight;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnDelete;

        // Attributes
        private System.Windows.Forms.CheckBox chkStart;
        private System.Windows.Forms.CheckBox chkFinish;
        private System.Windows.Forms.CheckBox chkDrop;

        // Options
        private System.Windows.Forms.Label lblOptionValue;
        private System.Windows.Forms.TextBox tbxOptionValue;
        private System.Windows.Forms.CheckBox chkPortal;

    }
}