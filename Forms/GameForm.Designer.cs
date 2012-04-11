namespace Maze.Forms
{
    partial class GameForm
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
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 200);
            this.Name = "GameForm";
            this.Text = "GameForm";
            this.ResumeLayout(false);

        }

        private void CustomInitialize()
        {
            MapEditorButton = new System.Windows.Forms.Button();
            GameButton = new System.Windows.Forms.Button();
            OpenMapButton = new System.Windows.Forms.Button();
            CreateMapButton = new System.Windows.Forms.Button();
            BackButton = new System.Windows.Forms.Button();
            PlayerNameTextBox = new System.Windows.Forms.TextBox();
            PlayerNameLabel = new System.Windows.Forms.Label();
            CurrentMapLabel = new System.Windows.Forms.Label();
            MapComboBox = new System.Windows.Forms.ComboBox();
            PlayButton = new System.Windows.Forms.Button();
            OpenSelectedMapButton = new System.Windows.Forms.Button();
            CreateNewMapButton = new System.Windows.Forms.Button();
            MapNameTextBox = new System.Windows.Forms.TextBox();
            MapNameLabel = new System.Windows.Forms.Label();

            // Back Button
            BackButton.Size = new System.Drawing.Size(30, 30);
            BackButton.Location = new System.Drawing.Point(20, 20);
            BackButton.Text = "<<";
            BackButton.Click +=new System.EventHandler(ButtonClick);

            //Initial
            //
            MapEditorButton.Size = new System.Drawing.Size(80, 30);
            GameButton.Size = new System.Drawing.Size(80, 30);

            MapEditorButton.Location = new System.Drawing.Point(160, 80);
            GameButton.Location = new System.Drawing.Point(160, 120);

            MapEditorButton.Text = "Map Editor";
            GameButton.Text = "Game";

            MapEditorButton.Click += new System.EventHandler(ButtonClick);
            GameButton.Click += new System.EventHandler(ButtonClick);

            //MapEditor
            //
            OpenMapButton.Size = new System.Drawing.Size(80, 30);
            CreateMapButton.Size = new System.Drawing.Size(80, 30);

            OpenMapButton.Location = new System.Drawing.Point(160, 80);
            CreateMapButton.Location = new System.Drawing.Point(160, 120);

            OpenMapButton.Text = "Open Map";
            CreateMapButton.Text = "Create New";

            OpenMapButton.Click += new System.EventHandler(ButtonClick);
            CreateMapButton.Click += new System.EventHandler(ButtonClick);

            // CreateNewMap
            //
            OpenSelectedMapButton.Size = new System.Drawing.Size(80, 30);
            OpenSelectedMapButton.Location = new System.Drawing.Point(160, 150);
            OpenSelectedMapButton.Click += new System.EventHandler(OpenSelectedMapButtonClick);
            OpenSelectedMapButton.Text = "Open";


            // OpenMap
            //
            CreateNewMapButton.Size = new System.Drawing.Size(80, 30);
            CreateNewMapButton.Location = new System.Drawing.Point(160, 150);
            CreateNewMapButton.Click += new System.EventHandler(CreateNewMapButtonClick);
            CreateNewMapButton.Text = "Create";

            MapNameTextBox.Size = new System.Drawing.Size(80, 35);
            MapNameTextBox.Location = new System.Drawing.Point(200, 100);
            MapNameTextBox.Text = "NewMap";

            MapNameLabel.Size = new System.Drawing.Size(80, 20);
            MapNameLabel.Location = new System.Drawing.Point(100, 100);
            MapNameLabel.Text = "Map Name";
            MapNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // PlayGame
            //
            PlayerNameTextBox.Size = new System.Drawing.Size(80, 35);
            PlayerNameTextBox.Location = new System.Drawing.Point(200, 70);
            PlayerNameTextBox.Text = "Player";

            PlayerNameLabel.Size = new System.Drawing.Size(80, 20);
            PlayerNameLabel.Location = new System.Drawing.Point(100, 70);
            PlayerNameLabel.Text = "Nickname";
            PlayerNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            CurrentMapLabel.Size = new System.Drawing.Size(80, 20);
            CurrentMapLabel.Location = new System.Drawing.Point(100, 110);
            CurrentMapLabel.Text = "Current Map";
            CurrentMapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            MapComboBox.Size = new System.Drawing.Size(80, 25);
            MapComboBox.Location = new System.Drawing.Point(200, 110);

            PlayButton.Size = new System.Drawing.Size(80, 30);
            PlayButton.Location = new System.Drawing.Point(160, 150);
            PlayButton.Click += new System.EventHandler(PlayButtonClick);
            PlayButton.Text = "Play";




            this.Controls.Add(BackButton);
            this.Controls.Add(MapEditorButton);
            this.Controls.Add(GameButton);
            this.Controls.Add(OpenMapButton);
            this.Controls.Add(CreateMapButton);
            this.Controls.Add(PlayerNameTextBox);
            this.Controls.Add(PlayerNameLabel);
            this.Controls.Add(CurrentMapLabel);
            this.Controls.Add(MapComboBox);
            this.Controls.Add(PlayButton);
            this.Controls.Add(OpenSelectedMapButton);
            this.Controls.Add(CreateNewMapButton);
            this.Controls.Add(MapNameLabel);
            this.Controls.Add(MapNameTextBox);
        }

        void PlayButton_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }


        #endregion

        private System.Windows.Forms.Button MapEditorButton;
        private System.Windows.Forms.Button GameButton;
        private System.Windows.Forms.Button OpenMapButton;
        private System.Windows.Forms.Button CreateMapButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.TextBox PlayerNameTextBox;
        private System.Windows.Forms.Label PlayerNameLabel;
        private System.Windows.Forms.Label CurrentMapLabel;
        private System.Windows.Forms.ComboBox MapComboBox;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button OpenSelectedMapButton;
        private System.Windows.Forms.Button CreateNewMapButton;
        private System.Windows.Forms.TextBox MapNameTextBox;
        private System.Windows.Forms.Label MapNameLabel;


    }
}