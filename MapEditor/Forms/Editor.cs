using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Maze.Classes;
using Maze.Forms;
using System.Windows.Forms;


namespace MapEditor.Forms
{
    public partial class Editor : MazeForm
    {
        /// <summary>
        /// Central position of the displayed map.
        /// </summary>
        private GPS centralGPS;
        private BlockEdit BlockEditForm;

        private Point capturedMousePoint;
        private bool isCapturingMove;

        public Editor()
        {
            InitializeComponent();
            CustomInitialize();

            this.centralGPS.Location = Map.Instance.GetStartPoint();
            this.centralGPS.X = 25;
            this.centralGPS.Y = 25;

            //this.centralGPS = new GPS(new GridLocation(25, 25, 0, 0), 25, 25);

            this.nudCurrentLevel.Value = 0;

            Invalidate();
            this.Focus();
        }

        void pbMap_MouseClick(object sender, MouseEventArgs e)
        {
            // Only Left Mouse Button
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;

            GridLocation cursorLocation = new GridLocation();

            // Calculate Mouse absolute position
            // i.e How far it is from the cetral position
            cursorLocation.X = this.centralGPS.Absolute.X - (this.pbMap.Size.Width / 2 - e.Location.X);
            cursorLocation.Y = this.centralGPS.Absolute.Y - (this.pbMap.Size.Height / 2 - e.Location.Y);

            GPS cursorGPS = new GPS();
            cursorGPS.Absolute = cursorLocation;

            Cell Block = Map.Instance.GetCell(cursorGPS.Location);

            if (BlockEditForm == null)
                BlockEditForm = new BlockEdit(Block);
            else
            {
                BlockEditForm.Close();
                BlockEditForm = new BlockEdit(Block);
            }
            BlockEditForm.Show();
            BlockEditForm.Focus();
        }

        void pbMap_Paint(object sender, PaintEventArgs e)
        {
            Graphics gGraphic = e.Graphics;

            GridLocation PBLocation = new GridLocation();
            Cell Block = new Cell();
            // CellGraph
            int cellsCountWidth = (int)Math.Ceiling(this.pbMap.Size.Width / 2d / GlobalConstants.CELL_WIDTH) * 2 + 1;
            int cellsCountHeight = (int)Math.Ceiling(this.pbMap.Size.Height / 2d / GlobalConstants.CELL_HEIGHT) * 2 + 1;

            // HACK: Correction values because the width and height of drawing region are not a multiple of CELL_WIDTH and CELL_HEIGHT
            int xCorrection = ((int)Math.Ceiling(this.pbMap.Size.Height * 1d / GlobalConstants.CELL_HEIGHT) * GlobalConstants.CELL_HEIGHT - this.pbMap.Size.Height) / 2;
            int yCorrection = ((int)Math.Ceiling(this.pbMap.Size.Width * 1d / GlobalConstants.CELL_WIDTH) * GlobalConstants.CELL_WIDTH - this.pbMap.Size.Width) / 2;

            for (int i = 0; i < cellsCountWidth; ++i)
                for (int j = 0; j < cellsCountHeight; ++j)
                {
                    int x, y;
                    x = (i - 1) * GlobalConstants.CELL_WIDTH - this.centralGPS.X + xCorrection + 25;
                    y = (j - 1) * GlobalConstants.CELL_HEIGHT - this.centralGPS.Y + yCorrection + 25;
                    PBLocation.X = centralGPS.Location.X + i - cellsCountWidth / 2;
                    PBLocation.Y = centralGPS.Location.Y + j - cellsCountHeight / 2;
                    PBLocation.Z = centralGPS.Location.Z;
                    PBLocation.Level = centralGPS.Location.Level;
                    Block = Map.Instance.GetCell(PBLocation);

                    gGraphic.DrawImage(PictureManager.GetPictureByType(Block.Type), x, y, GlobalConstants.CELL_WIDTH, GlobalConstants.CELL_HEIGHT);

                    // Draw Start Block
                    if (Block.HasAttribute(CellAttributes.IsStart))
                    {
                        gGraphic.DrawImage(PictureManager.StartImage, x + 5, y + 5, 40, 40);
                    }

                    // Draw Finish Block
                    if (Block.HasAttribute(CellAttributes.IsFinish))
                    {
                        gGraphic.DrawImage(PictureManager.FinishImage, x + 5, y + 5, PictureManager.FinishImage.Width, PictureManager.FinishImage.Height);
                    }

                    // Draw Ooze Drop
                    if (Block.HasAttribute(CellAttributes.HasDrop))
                    {
                        gGraphic.DrawImage(PictureManager.DropImage, x + 15, y + 10, 20, 30);
                    }

                    // Portal
                    if (Block.HasOption(CellOptions.Portal))
                    {
                        Image image = PictureManager.PortalImage;
                        gGraphic.DrawImage(image,
                            x + (GlobalConstants.CELL_WIDTH - image.Width) / 2,
                            y + (GlobalConstants.CELL_HEIGHT - image.Height) / 2,
                            PictureManager.PortalImage.Width, PictureManager.PortalImage.Height);
                    }
                }
        }

        void nudCurrentLevel_ValueChanged(object sender, System.EventArgs e)
        {
            if (this.nudCurrentLevel.Value < 0)
                this.nudCurrentLevel.Value = 0;
            else if (this.nudCurrentLevel.Value > 100)
                this.nudCurrentLevel.Value = 100;

            Map.Instance.SetMap(Map.Instance.GetMap(), (int)this.nudCurrentLevel.Value);
            centralGPS.Location.Level = (int)this.nudCurrentLevel.Value;
            this.pbMap.Refresh();
            this.Focus();
        }

        void cboCurrentMap_SelectedValueChanged(object sender, System.EventArgs e)
        {
            // TODO: Load selected map from the file.
        }

        private void btnAddMap_Click(object sender, System.EventArgs e)
        {
            // TODO: Add new map
        }

        private void btnRemoveMap_Click(object sender, System.EventArgs e)
        {
            // TODO: Remove current map
        }

        private void btnAddLevel_Click(object sender, System.EventArgs e)
        {
            // TODO: Add new level
        }

        private void btnRemoveLevel_Click(object sender, System.EventArgs e)
        {
            // TODO: remove current level
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            // TODO: Save current map into the file.
        }

        private void pbMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            this.capturedMousePoint = Cursor.Position;
            this.isCapturingMove = true;
            Cursor = Cursors.Hand;
        }

        private void pbMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            this.isCapturingMove = false;
            Cursor = DefaultCursor;
        }

        private void pbMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isCapturingMove)
                return;
            // Find out where the mouse were moved
            // and these changes apply to the new Map center
            GridLocation prevLocation = this.centralGPS.Absolute;
            prevLocation.X += this.capturedMousePoint.X - Cursor.Position.X;
            prevLocation.Y += this.capturedMousePoint.Y - Cursor.Position.Y;
            this.centralGPS.Absolute = prevLocation;

            //Cursor.Position = this.capturedMousePoint;
            this.capturedMousePoint = Cursor.Position;
            this.pbMap.Refresh();
        }

        private void MapEditorFormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: Ask a confirmation if the map was changed
        }
    }
}
