namespace EffectEditor
{
    partial class EffectEditor
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnReload = new System.Windows.Forms.Button();
            this.ptbAura = new System.Windows.Forms.PictureBox();
            this.ptbMap = new System.Windows.Forms.PictureBox();
            this.txtEffectInfo = new System.Windows.Forms.TextBox();
            this.cmbInfoID = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnClean = new System.Windows.Forms.Button();
            this.chbOneTact = new System.Windows.Forms.CheckBox();
            this.txtAttributes = new System.Windows.Forms.TextBox();
            this.lblAura = new System.Windows.Forms.Label();
            this.lablMap = new System.Windows.Forms.Label();
            this.ptbUploadedAura = new System.Windows.Forms.PictureBox();
            this.ptbUploadedMap = new System.Windows.Forms.PictureBox();
            this.btnSelectAura = new System.Windows.Forms.Button();
            this.txtAura = new System.Windows.Forms.TextBox();
            this.btnSelectMap = new System.Windows.Forms.Button();
            this.txtMap = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnCreateString = new System.Windows.Forms.Button();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.lblEffectName = new System.Windows.Forms.Label();
            this.txtEffectName = new System.Windows.Forms.TextBox();
            this.lblND4 = new System.Windows.Forms.Label();
            this.lblND3 = new System.Windows.Forms.Label();
            this.lblND2 = new System.Windows.Forms.Label();
            this.lbl = new System.Windows.Forms.Label();
            this.lblRange = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblTargets = new System.Windows.Forms.Label();
            this.lblAttributes = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblEffectID = new System.Windows.Forms.Label();
            this.txtND4 = new System.Windows.Forms.TextBox();
            this.txtND3 = new System.Windows.Forms.TextBox();
            this.txtND2 = new System.Windows.Forms.TextBox();
            this.txtND1 = new System.Windows.Forms.TextBox();
            this.txtRange = new System.Windows.Forms.TextBox();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.cmbTargets = new System.Windows.Forms.ComboBox();
            this.btnAddAttributes = new System.Windows.Forms.Button();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtEffectID = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbAura)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMap)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbUploadedAura)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbUploadedMap)).BeginInit();
            this.SuspendLayout();
            //
            // tabControl1
            //
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 26);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(688, 378);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            //
            // tabPage1
            //
            this.tabPage1.Controls.Add(this.btnReload);
            this.tabPage1.Controls.Add(this.ptbAura);
            this.tabPage1.Controls.Add(this.ptbMap);
            this.tabPage1.Controls.Add(this.txtEffectInfo);
            this.tabPage1.Controls.Add(this.cmbInfoID);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(680, 352);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Info";
            this.tabPage1.UseVisualStyleBackColor = true;
            //
            // btnReload
            //
            this.btnReload.Location = new System.Drawing.Point(499, 46);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 5;
            this.btnReload.TabStop = false;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // ptbAura
            //
            this.ptbAura.Location = new System.Drawing.Point(596, 159);
            this.ptbAura.Name = "ptbAura";
            this.ptbAura.Size = new System.Drawing.Size(60, 60);
            this.ptbAura.TabIndex = 0;
            this.ptbAura.TabStop = false;
            //
            // ptbMap
            //
            this.ptbMap.Location = new System.Drawing.Point(488, 159);
            this.ptbMap.Name = "ptbMap";
            this.ptbMap.Size = new System.Drawing.Size(60, 60);
            this.ptbMap.TabIndex = 1;
            this.ptbMap.TabStop = false;
            //
            // txtEffectInfo
            //
            this.txtEffectInfo.Location = new System.Drawing.Point(21, 51);
            this.txtEffectInfo.Multiline = true;
            this.txtEffectInfo.Name = "txtEffectInfo";
            this.txtEffectInfo.Size = new System.Drawing.Size(440, 282);
            this.txtEffectInfo.TabIndex = 4;
            //
            // cmbInfoID
            //
            this.cmbInfoID.Location = new System.Drawing.Point(328, 19);
            this.cmbInfoID.Name = "cmbInfoID";
            this.cmbInfoID.Size = new System.Drawing.Size(133, 21);
            this.cmbInfoID.TabIndex = 3;
            this.cmbInfoID.SelectedIndexChanged += new System.EventHandler(this.cmbInfoID_SelectedIndexChanged);
            //
            // tabPage2
            //
            this.tabPage2.Controls.Add(this.btnClean);
            this.tabPage2.Controls.Add(this.chbOneTact);
            this.tabPage2.Controls.Add(this.txtAttributes);
            this.tabPage2.Controls.Add(this.lblAura);
            this.tabPage2.Controls.Add(this.lablMap);
            this.tabPage2.Controls.Add(this.ptbUploadedAura);
            this.tabPage2.Controls.Add(this.ptbUploadedMap);
            this.tabPage2.Controls.Add(this.btnSelectAura);
            this.tabPage2.Controls.Add(this.txtAura);
            this.tabPage2.Controls.Add(this.btnSelectMap);
            this.tabPage2.Controls.Add(this.txtMap);
            this.tabPage2.Controls.Add(this.btnCopy);
            this.tabPage2.Controls.Add(this.btnCreateString);
            this.tabPage2.Controls.Add(this.txtSummary);
            this.tabPage2.Controls.Add(this.lblEffectName);
            this.tabPage2.Controls.Add(this.txtEffectName);
            this.tabPage2.Controls.Add(this.lblND4);
            this.tabPage2.Controls.Add(this.lblND3);
            this.tabPage2.Controls.Add(this.lblND2);
            this.tabPage2.Controls.Add(this.lbl);
            this.tabPage2.Controls.Add(this.lblRange);
            this.tabPage2.Controls.Add(this.lblDuration);
            this.tabPage2.Controls.Add(this.lblValue);
            this.tabPage2.Controls.Add(this.lblType);
            this.tabPage2.Controls.Add(this.lblTargets);
            this.tabPage2.Controls.Add(this.lblAttributes);
            this.tabPage2.Controls.Add(this.lblDescription);
            this.tabPage2.Controls.Add(this.lblEffectID);
            this.tabPage2.Controls.Add(this.txtND4);
            this.tabPage2.Controls.Add(this.txtND3);
            this.tabPage2.Controls.Add(this.txtND2);
            this.tabPage2.Controls.Add(this.txtND1);
            this.tabPage2.Controls.Add(this.txtRange);
            this.tabPage2.Controls.Add(this.txtDuration);
            this.tabPage2.Controls.Add(this.txtValue);
            this.tabPage2.Controls.Add(this.cmbType);
            this.tabPage2.Controls.Add(this.cmbTargets);
            this.tabPage2.Controls.Add(this.btnAddAttributes);
            this.tabPage2.Controls.Add(this.txtDescription);
            this.tabPage2.Controls.Add(this.txtEffectID);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(680, 352);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "New effect";
            this.tabPage2.UseVisualStyleBackColor = true;
            //
            // btnClean
            //
            this.btnClean.Location = new System.Drawing.Point(237, 307);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(75, 26);
            this.btnClean.TabIndex = 17;
            this.btnClean.Text = "Clean";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            //
            // chbOneTact
            //
            this.chbOneTact.AutoSize = true;
            this.chbOneTact.Location = new System.Drawing.Point(165, 240);
            this.chbOneTact.Name = "chbOneTact";
            this.chbOneTact.Size = new System.Drawing.Size(67, 17);
            this.chbOneTact.TabIndex = 4;
            this.chbOneTact.Text = "One-tact";
            this.chbOneTact.UseVisualStyleBackColor = true;
            this.chbOneTact.CheckedChanged += new System.EventHandler(this.chbOneTact_CheckedChanged);
            //
            // txtAttributes
            //
            this.txtAttributes.Enabled = false;
            this.txtAttributes.Location = new System.Drawing.Point(76, 132);
            this.txtAttributes.Name = "txtAttributes";
            this.txtAttributes.Size = new System.Drawing.Size(144, 20);
            this.txtAttributes.TabIndex = 18;
            //
            // lblAura
            //
            this.lblAura.AutoSize = true;
            this.lblAura.Location = new System.Drawing.Point(277, 223);
            this.lblAura.Name = "lblAura";
            this.lblAura.Size = new System.Drawing.Size(29, 13);
            this.lblAura.TabIndex = 19;
            this.lblAura.Text = "Aura";
            //
            // lablMap
            //
            this.lablMap.AutoSize = true;
            this.lablMap.Location = new System.Drawing.Point(277, 166);
            this.lablMap.Name = "lablMap";
            this.lablMap.Size = new System.Drawing.Size(28, 13);
            this.lablMap.TabIndex = 20;
            this.lablMap.Text = "Map";
            //
            // ptbUploadedAura
            //
            this.ptbUploadedAura.Location = new System.Drawing.Point(595, 201);
            this.ptbUploadedAura.Name = "ptbUploadedAura";
            this.ptbUploadedAura.Size = new System.Drawing.Size(50, 50);
            this.ptbUploadedAura.TabIndex = 21;
            this.ptbUploadedAura.TabStop = false;
            //
            // ptbUploadedMap
            //
            this.ptbUploadedMap.Location = new System.Drawing.Point(595, 146);
            this.ptbUploadedMap.Name = "ptbUploadedMap";
            this.ptbUploadedMap.Size = new System.Drawing.Size(40, 40);
            this.ptbUploadedMap.TabIndex = 22;
            this.ptbUploadedMap.TabStop = false;
            //
            // btnSelectAura
            //
            this.btnSelectAura.Location = new System.Drawing.Point(458, 220);
            this.btnSelectAura.Name = "btnSelectAura";
            this.btnSelectAura.Size = new System.Drawing.Size(89, 23);
            this.btnSelectAura.TabIndex = 13;
            this.btnSelectAura.Text = "Upload";
            this.btnSelectAura.UseVisualStyleBackColor = true;
            this.btnSelectAura.Click += new System.EventHandler(this.btnSelectAura_Click);
            //
            // txtAura
            //
            this.txtAura.Location = new System.Drawing.Point(312, 223);
            this.txtAura.Name = "txtAura";
            this.txtAura.Size = new System.Drawing.Size(140, 20);
            this.txtAura.TabIndex = 12;
            //
            // btnSelectMap
            //
            this.btnSelectMap.Location = new System.Drawing.Point(458, 163);
            this.btnSelectMap.Name = "btnSelectMap";
            this.btnSelectMap.Size = new System.Drawing.Size(89, 23);
            this.btnSelectMap.TabIndex = 11;
            this.btnSelectMap.Text = "Upload";
            this.btnSelectMap.UseVisualStyleBackColor = true;
            this.btnSelectMap.Click += new System.EventHandler(this.btnSelectMap_Click);
            //
            // txtMap
            //
            this.txtMap.Location = new System.Drawing.Point(312, 166);
            this.txtMap.Name = "txtMap";
            this.txtMap.Size = new System.Drawing.Size(140, 20);
            this.txtMap.TabIndex = 10;
            //
            // btnCopy
            //
            this.btnCopy.Location = new System.Drawing.Point(585, 259);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(89, 23);
            this.btnCopy.TabIndex = 16;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            //
            // btnCreateString
            //
            this.btnCreateString.Location = new System.Drawing.Point(482, 259);
            this.btnCreateString.Name = "btnCreateString";
            this.btnCreateString.Size = new System.Drawing.Size(89, 23);
            this.btnCreateString.TabIndex = 14;
            this.btnCreateString.Text = "Create string";
            this.btnCreateString.UseVisualStyleBackColor = true;
            this.btnCreateString.Click += new System.EventHandler(this.btnCreateString_Click);
            //
            // txtSummary
            //
            this.txtSummary.Location = new System.Drawing.Point(354, 288);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.Size = new System.Drawing.Size(320, 57);
            this.txtSummary.TabIndex = 15;
            //
            // lblEffectName
            //
            this.lblEffectName.AutoSize = true;
            this.lblEffectName.Location = new System.Drawing.Point(10, 39);
            this.lblEffectName.Name = "lblEffectName";
            this.lblEffectName.Size = new System.Drawing.Size(64, 13);
            this.lblEffectName.TabIndex = 23;
            this.lblEffectName.Text = "Effect name";
            //
            // txtEffectName
            //
            this.txtEffectName.Location = new System.Drawing.Point(76, 39);
            this.txtEffectName.Name = "txtEffectName";
            this.txtEffectName.Size = new System.Drawing.Size(144, 20);
            this.txtEffectName.TabIndex = 1;
            //
            // lblND4
            //
            this.lblND4.AutoSize = true;
            this.lblND4.Location = new System.Drawing.Point(557, 88);
            this.lblND4.Name = "lblND4";
            this.lblND4.Size = new System.Drawing.Size(29, 13);
            this.lblND4.TabIndex = 24;
            this.lblND4.Text = "ND4";
            //
            // lblND3
            //
            this.lblND3.AutoSize = true;
            this.lblND3.Location = new System.Drawing.Point(557, 62);
            this.lblND3.Name = "lblND3";
            this.lblND3.Size = new System.Drawing.Size(29, 13);
            this.lblND3.TabIndex = 25;
            this.lblND3.Text = "ND3";
            //
            // lblND2
            //
            this.lblND2.AutoSize = true;
            this.lblND2.Location = new System.Drawing.Point(557, 36);
            this.lblND2.Name = "lblND2";
            this.lblND2.Size = new System.Drawing.Size(29, 13);
            this.lblND2.TabIndex = 26;
            this.lblND2.Text = "ND2";
            //
            // lbl
            //
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(557, 10);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(29, 13);
            this.lbl.TabIndex = 27;
            this.lbl.Text = "ND1";
            //
            // lblRange
            //
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(10, 271);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(39, 13);
            this.lblRange.TabIndex = 28;
            this.lblRange.Text = "Range";
            //
            // lblDuration
            //
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(10, 238);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(47, 13);
            this.lblDuration.TabIndex = 29;
            this.lblDuration.Text = "Duration";
            //
            // lblValue
            //
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(10, 212);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(34, 13);
            this.lblValue.TabIndex = 30;
            this.lblValue.Text = "Value";
            //
            // lblType
            //
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(10, 185);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 13);
            this.lblType.TabIndex = 31;
            this.lblType.Text = "Type";
            //
            // lblTargets
            //
            this.lblTargets.AutoSize = true;
            this.lblTargets.Location = new System.Drawing.Point(10, 158);
            this.lblTargets.Name = "lblTargets";
            this.lblTargets.Size = new System.Drawing.Size(43, 13);
            this.lblTargets.TabIndex = 32;
            this.lblTargets.Text = "Targets";
            //
            // lblAttributes
            //
            this.lblAttributes.AutoSize = true;
            this.lblAttributes.Location = new System.Drawing.Point(10, 134);
            this.lblAttributes.Name = "lblAttributes";
            this.lblAttributes.Size = new System.Drawing.Size(51, 13);
            this.lblAttributes.TabIndex = 33;
            this.lblAttributes.Text = "Attributes";
            //
            // lblDescription
            //
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(10, 65);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 34;
            this.lblDescription.Text = "Description";
            //
            // lblEffectID
            //
            this.lblEffectID.AutoSize = true;
            this.lblEffectID.Location = new System.Drawing.Point(10, 20);
            this.lblEffectID.Name = "lblEffectID";
            this.lblEffectID.Size = new System.Drawing.Size(18, 13);
            this.lblEffectID.TabIndex = 35;
            this.lblEffectID.Text = "ID";
            //
            // txtND4
            //
            this.txtND4.Location = new System.Drawing.Point(623, 88);
            this.txtND4.Name = "txtND4";
            this.txtND4.Size = new System.Drawing.Size(51, 20);
            this.txtND4.TabIndex = 9;
            //
            // txtND3
            //
            this.txtND3.Location = new System.Drawing.Point(623, 62);
            this.txtND3.Name = "txtND3";
            this.txtND3.Size = new System.Drawing.Size(51, 20);
            this.txtND3.TabIndex = 8;
            //
            // txtND2
            //
            this.txtND2.Location = new System.Drawing.Point(623, 36);
            this.txtND2.Name = "txtND2";
            this.txtND2.Size = new System.Drawing.Size(51, 20);
            this.txtND2.TabIndex = 7;
            //
            // txtND1
            //
            this.txtND1.Location = new System.Drawing.Point(623, 10);
            this.txtND1.Name = "txtND1";
            this.txtND1.Size = new System.Drawing.Size(51, 20);
            this.txtND1.TabIndex = 6;
            //
            // txtRange
            //
            this.txtRange.Location = new System.Drawing.Point(76, 264);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(144, 20);
            this.txtRange.TabIndex = 5;
            //
            // txtDuration
            //
            this.txtDuration.Location = new System.Drawing.Point(76, 238);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.Size = new System.Drawing.Size(68, 20);
            this.txtDuration.TabIndex = 4;
            //
            // txtValue
            //
            this.txtValue.Location = new System.Drawing.Point(76, 212);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(144, 20);
            this.txtValue.TabIndex = 3;
            //
            // cmbType
            //
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(76, 185);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(144, 21);
            this.cmbType.TabIndex = 36;
            //
            // cmbTargets
            //
            this.cmbTargets.FormattingEnabled = true;
            this.cmbTargets.Location = new System.Drawing.Point(76, 158);
            this.cmbTargets.Name = "cmbTargets";
            this.cmbTargets.Size = new System.Drawing.Size(144, 21);
            this.cmbTargets.TabIndex = 37;
            //
            // btnAddAttributes
            //
            this.btnAddAttributes.Location = new System.Drawing.Point(237, 131);
            this.btnAddAttributes.Name = "btnAddAttributes";
            this.btnAddAttributes.Size = new System.Drawing.Size(68, 23);
            this.btnAddAttributes.TabIndex = 38;
            this.btnAddAttributes.Text = "Edit";
            this.btnAddAttributes.UseVisualStyleBackColor = true;
            this.btnAddAttributes.Click += new System.EventHandler(this.btnAddAttributes_Click);
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(76, 65);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(187, 60);
            this.txtDescription.TabIndex = 2;
            //
            // txtEffectID
            //
            this.txtEffectID.Location = new System.Drawing.Point(76, 13);
            this.txtEffectID.Name = "txtEffectID";
            this.txtEffectID.Size = new System.Drawing.Size(82, 20);
            this.txtEffectID.TabIndex = 0;
            //
            // folderBrowserDialog1
            //
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            //
            // EffectEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 416);
            this.Controls.Add(this.tabControl1);
            this.Name = "EffectEditor";
            this.Text = "EffectEditor1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbAura)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMap)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbUploadedAura)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbUploadedMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox cmbInfoID;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtEffectInfo;
        private System.Windows.Forms.PictureBox ptbAura;
        private System.Windows.Forms.PictureBox ptbMap;
        private System.Windows.Forms.Button btnAddAttributes;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtEffectID;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnCreateString;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.Label lblEffectName;
        private System.Windows.Forms.TextBox txtEffectName;
        private System.Windows.Forms.Label lblND4;
        private System.Windows.Forms.Label lblND3;
        private System.Windows.Forms.Label lblND2;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblTargets;
        private System.Windows.Forms.Label lblAttributes;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblEffectID;
        private System.Windows.Forms.TextBox txtND4;
        private System.Windows.Forms.TextBox txtND3;
        private System.Windows.Forms.TextBox txtND2;
        private System.Windows.Forms.TextBox txtND1;
        private System.Windows.Forms.TextBox txtRange;
        private System.Windows.Forms.TextBox txtDuration;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.ComboBox cmbTargets;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.PictureBox ptbUploadedAura;
        private System.Windows.Forms.PictureBox ptbUploadedMap;
        private System.Windows.Forms.Button btnSelectAura;
        private System.Windows.Forms.TextBox txtAura;
        private System.Windows.Forms.Button btnSelectMap;
        private System.Windows.Forms.TextBox txtMap;
        private System.Windows.Forms.Label lblAura;
        private System.Windows.Forms.Label lablMap;

        private System.Windows.Forms.TextBox txtAttributes;
        private System.Windows.Forms.CheckBox chbOneTact;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.Button btnReload;
    }
}