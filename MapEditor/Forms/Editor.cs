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
using System.IO;


namespace MapEditor.Forms
{
    public partial class Editor : MazeForm
    {
        /// <summary>
        /// Central position of the displayed map.
        /// </summary>
        private GPS centralGPS;
        private CellEdit cellEditForm;

        private Point capturedMousePoint;
        private bool isCapturingMove;

        private string[] mapNames;
        private int currentMapIndex;
        private int levelsCount;
        private bool isNewMap;

        private Dictionary<GridLocation, Cell> mapCells;
        private Dictionary<int, GridLocation> startLocations;
        private Dictionary<int, GridLocation> finishLocations;

        private int maxCellId;

        public Editor()
        {
            InitializeComponent();
            CustomInitialize();

            this.mapCells = new Dictionary<GridLocation, Cell>();
            this.startLocations = new Dictionary<int, GridLocation>();
            this.finishLocations = new Dictionary<int, GridLocation>();

            this.nudCurrentLevel.Value = 0;
            this.currentMapIndex = -1;

            this.pr_isMapSaved = true;

            LoadMapNames();
            if (this.mapNames.Length > 0)
                LoadMap(0);

            Invalidate();
            this.Focus();
        }

        private int pr_currentLevel;
        /// <summary>
        /// Gets or sets the current level of the map.
        /// </summary>
        private int currentLevel
        {
            get
            {
                return this.pr_currentLevel;
            }
            set
            {
                this.pr_currentLevel = value;
                // Return center to the Start point
                if (this.startLocations.ContainsKey(this.pr_currentLevel))
                    this.centralGPS = new GPS(this.startLocations[this.pr_currentLevel], 25, 25);
                else
                    this.centralGPS = new GPS(new GridLocation(0, 0, 0, this.pr_currentLevel), 25, 25);
                this.pbMap.Refresh();
            }
        }

        public int NewCellID
        {
            get
            {
                return this.maxCellId + 1;
            }
        }

        private bool pr_isMapSaved;
        private bool isMapSaved
        {
            get
            {
                return this.pr_isMapSaved;
            }
            set
            {
                if (this.pr_isMapSaved == value)
                    return;

                if (value)
                {
                    this.lblIsMapSaved.Text = "Saved";
                    this.btnSave.Enabled = false;
                }
                else
                {
                    this.lblIsMapSaved.Text = "";
                    this.btnSave.Enabled = true;
                }

                this.pr_isMapSaved = value;
            }

        }

        private void LoadMapNames()
        {
            DirectoryInfo mapDirectory = new DirectoryInfo(GlobalConstants.MAPS_PATH);
            FileInfo[] mapFiles = mapDirectory.GetFiles();
            List<string> mapNames = new List<string>();

            foreach (FileInfo fi in mapFiles)
            {
                if (fi.Extension == ".map")
                    mapNames.Add(Path.GetFileNameWithoutExtension(fi.FullName));
            }

            this.mapNames = mapNames.ToArray();
            this.cboCurrentMap.Items.AddRange(this.mapNames);
            this.cboCurrentMap.SelectedIndex = 0;
        }

        private void LoadMap(int mapIndex)
        {
            // An attempt to reload the current map.
            if (this.currentMapIndex == mapIndex)
                return;

            string mapName = null;
            if (mapIndex != -1)
            {
                try
                {
                    mapName = this.mapNames[mapIndex];
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("Map with index '" + mapIndex.ToString() + "' doesn't exist");
                    return;
                }
            }

            this.mapCells = new Dictionary<GridLocation, Cell>();
            this.startLocations = new Dictionary<int, GridLocation>();
            this.finishLocations = new Dictionary<int, GridLocation>();
            this.currentMapIndex = mapIndex;
            this.levelsCount = 1;
            this.maxCellId = 0;

            if (mapIndex != -1)
            {
                StreamReader reader = File.OpenText(GlobalConstants.MAPS_PATH + this.mapNames[this.currentMapIndex] + ".map");
                string currentString;
                while ((currentString = reader.ReadLine()) != null)
                {
                    string[] stringStruct = new string[10];
                    stringStruct = currentString.Split(' ');
                    Cell cell;

                    cell.ID = Convert.ToInt32(stringStruct[0]);
                    cell.Location.X = Convert.ToInt32(stringStruct[1]);
                    cell.Location.Y = Convert.ToInt32(stringStruct[2]);
                    cell.Location.Z = Convert.ToInt32(stringStruct[3]);
                    cell.Location.Level = Convert.ToInt32(stringStruct[4]);
                    cell.Type = Convert.ToUInt32(stringStruct[5]);
                    cell.Attribute = Convert.ToUInt32(stringStruct[6]);
                    cell.Option = Convert.ToUInt32(stringStruct[7]);
                    cell.OptionValue = Convert.ToInt32(stringStruct[8]);
                    cell.ND4 = Convert.ToInt32(stringStruct[9]);

                    AddCell(cell);
                }
                reader.Close();

                this.isNewMap = false;
                this.tbxMapName.Text = mapName;
                this.isMapSaved = true;
            }
            else
            {
                this.isNewMap = true;
                this.tbxMapName.Text = "Enter_name";
                this.tbxMapName.Focus();
            }

            // Set Level to 0
            this.currentLevel = 0;
        }

        private void SaveMap()
        {
            string mapName = this.tbxMapName.Text;
            if (string.IsNullOrEmpty(mapName))
            {
                MessageBox.Show("Enter Map Name", "Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tbxMapName.Focus();
                return;
            }
            for (int i = 0; i < this.mapNames.Length; ++i)
            {
                if (i == this.currentMapIndex)
                    continue;
                if (this.mapNames[i] == mapName)
                {
                    MessageBox.Show("Map \"" + mapName + "\" already exists.", "Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.tbxMapName.Focus();
                    return;
                }
            }

            StreamWriter writer = new StreamWriter(GlobalConstants.MAPS_PATH + mapName + ".map", false);
            string cellString;

            foreach (KeyValuePair<GridLocation, Cell> cell in this.mapCells)
            {
                cellString = cell.Value.ID.ToString() + " "
                    + cell.Value.Location.X.ToString() + " "
                    + cell.Value.Location.Y.ToString() + " "
                    + cell.Value.Location.Z.ToString() + " "
                    + cell.Value.Location.Level.ToString() + " "
                    + cell.Value.Type.ToString() + " "
                    + cell.Value.Attribute.ToString() + " "
                    + cell.Value.Option.ToString() + " "
                    + cell.Value.OptionValue.ToString() + " "
                    + cell.Value.ND4.ToString();
                writer.WriteLine(cellString);
            }
            writer.Close();

            if (this.isNewMap)
            {
                Array.Resize<string>(ref this.mapNames, this.mapNames.Length + 1);
                this.currentMapIndex = this.mapNames.Length - 1;
                this.mapNames[this.currentMapIndex] = mapName;
                this.cboCurrentMap.Items.Clear();
                this.cboCurrentMap.Items.AddRange(this.mapNames);
                this.cboCurrentMap.SelectedIndex = this.currentMapIndex;
                this.isNewMap = false;
            }
            else
            {
                // Change displaying Map Name if it has been changed
                if (mapName != this.mapNames[this.currentMapIndex])
                {
                    // Drop the previous map file
                    File.Delete(GlobalConstants.MAPS_PATH + this.mapNames[this.currentMapIndex] + ".map");
                    this.cboCurrentMap.Items[this.currentMapIndex] = mapName;
                    this.mapNames[this.currentMapIndex] = mapName;
                }
            }

            // Mark that map is at newer verstion
            this.isMapSaved = true;
        }

        public void AddCell(Cell cell)
        {
            if (this.mapCells.ContainsKey(cell.Location))
            {
                ReplaceCell(cell);
                return;
            }

            this.mapCells.Add(cell.Location, cell);

            if (Convert.ToInt32(cell.Location.Level) >= this.levelsCount)
                ++this.levelsCount;

            if (cell.HasAttribute(CellAttributes.IsStart))
                this.startLocations[cell.Location.Level] = cell.Location;
            if (cell.HasAttribute(CellAttributes.IsFinish))
                this.finishLocations[cell.Location.Level] = cell.Location;

            if (cell.ID > this.maxCellId)
                this.maxCellId = cell.ID;

            this.isMapSaved = false;
        }

        public void RemoveCell(Cell cell)
        {
            this.mapCells.Remove(cell.Location);

            if (this.maxCellId == cell.ID)
                --this.maxCellId;

            if (cell.HasAttribute(CellAttributes.IsStart))
                this.startLocations.Remove(cell.Location.Level);
            if (cell.HasAttribute(CellAttributes.IsFinish))
                this.startLocations.Remove(cell.Location.Level);

            this.isMapSaved = false;
        }

        public void ReplaceCell(Cell cell)
        {
            RemoveCell(cell);
            AddCell(cell);
        }

        public Cell GetCell(GridLocation location)
        {
            Cell cell;
            if (!this.mapCells.TryGetValue(location, out cell))
            {
                // default cell
                cell.Initialize();
                // but specified location
                cell.Location = location;
            }

            return cell;
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
            cursorLocation.Z = this.centralGPS.Absolute.Z;
            cursorLocation.Level = this.centralGPS.Location.Level;

            GPS cursorGPS = new GPS();
            cursorGPS.Absolute = cursorLocation;

            Cell cell = GetCell(cursorGPS.Location);

            // Prevent double BlockEdit window openning
            if (this.cellEditForm == null)
                this.cellEditForm = new CellEdit(cell);
            else
            {
                this.cellEditForm.Close();
                this.cellEditForm = new CellEdit(cell);
            }
            this.cellEditForm.FormClosing += (sender_, e_) => { this.pbMap.Refresh(); };
            this.cellEditForm.ShowDialog();
        }

        void pbMap_Paint(object sender, PaintEventArgs e)
        {
            Graphics gGraphic = e.Graphics;

            GridLocation cellLocation = new GridLocation();
            Cell cell = new Cell();
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
                    cellLocation.X = centralGPS.Location.X + i - cellsCountWidth / 2;
                    cellLocation.Y = centralGPS.Location.Y + j - cellsCountHeight / 2;
                    cellLocation.Z = centralGPS.Location.Z;
                    cellLocation.Level = centralGPS.Location.Level;
                    cell = GetCell(cellLocation);

                    gGraphic.DrawImage(PictureManager.GetPictureByType(cell.Type), x, y, GlobalConstants.CELL_WIDTH, GlobalConstants.CELL_HEIGHT);

                    // Draw Start Block
                    if (cell.HasAttribute(CellAttributes.IsStart))
                    {
                        gGraphic.DrawImage(PictureManager.StartImage, x + 5, y + 5, 40, 40);
                    }

                    // Draw Finish Block
                    if (cell.HasAttribute(CellAttributes.IsFinish))
                    {
                        gGraphic.DrawImage(PictureManager.FinishImage, x + 5, y + 5, PictureManager.FinishImage.Width, PictureManager.FinishImage.Height);
                    }

                    // Draw Ooze Drop
                    if (cell.HasAttribute(CellAttributes.HasDrop))
                    {
                        gGraphic.DrawImage(PictureManager.DropImage, x + 15, y + 10, 20, 30);
                    }

                    // Portal
                    if (cell.HasOption(CellOptions.Portal))
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
            else if (this.nudCurrentLevel.Value >= this.levelsCount)
                this.nudCurrentLevel.Value = this.levelsCount - 1;

            this.currentLevel = (int)this.nudCurrentLevel.Value;
        }

        void cboCurrentMap_SelectedValueChanged(object sender, System.EventArgs e)
        {
            if (!this.isMapSaved && this.currentMapIndex != this.cboCurrentMap.SelectedIndex)
            {
                System.Windows.Forms.DialogResult result = MessageBox.Show("Would you like to save the current map?", "Map Saving", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                    SaveMap();
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                    this.cboCurrentMap.SelectedIndex = this.currentMapIndex;

            }
            LoadMap(this.cboCurrentMap.SelectedIndex);
        }

        private void btnAddMap_Click(object sender, System.EventArgs e)
        {
            this.cboCurrentMap.SelectedIndex = -1;
        }

        private void btnRemoveMap_Click(object sender, System.EventArgs e)
        {
            // TODO: Remove current map
        }

        private void btnAddLevel_Click(object sender, System.EventArgs e)
        {
            ++this.levelsCount;
            this.nudCurrentLevel.Value = (int)(this.levelsCount - 1);
        }

        private void btnRemoveLevel_Click(object sender, System.EventArgs e)
        {
            // TODO: remove current level
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            SaveMap();
        }

        void tbxMapName_TextChanged(object sender, System.EventArgs e)
        {
            this.isMapSaved = false;
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

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.isMapSaved)
            {
                System.Windows.Forms.DialogResult result = MessageBox.Show("Would you like to save the current map?", "Map Saving", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                    SaveMap();
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;
            }
        }


        private void pbMap_MouseLeave(object sender, System.EventArgs e)
        {
            this.tbxMapName.Focus();
        }

        private void pbMap_MouseEnter(object sender, System.EventArgs e)
        {
            this.pbMap.Focus();
        }

        private void pbMap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.W && e.KeyCode != Keys.A && e.KeyCode != Keys.S && e.KeyCode != Keys.D && e.KeyCode != Keys.Z && e.KeyCode != Keys.R)
                return;

            // Compute the cursor GPS
            Point cursorPoint = this.pbMap.PointToClient(Cursor.Position);
            //Point cursorLocation = new Point(Cursor.Position.X - pbMapPoint.X, Cursor.Position.Y - pbMapPoint.Y);

            GridLocation cursorLocation = new GridLocation();

            // Calculate Mouse absolute position
            // i.e How far it is from the cetral position
            cursorLocation.X = this.centralGPS.Absolute.X - (this.pbMap.Size.Width / 2 - cursorPoint.X);
            cursorLocation.Y = this.centralGPS.Absolute.Y - (this.pbMap.Size.Height / 2 - cursorPoint.Y);
            cursorLocation.Z = this.centralGPS.Absolute.Z;
            cursorLocation.Level = this.centralGPS.Location.Level;

            GPS cursorGPS = new GPS();
            cursorGPS.Absolute = cursorLocation;

            Cell cell = GetCell(cursorGPS.Location);

            if (cell.Type == (uint)Directions.None)
                cell.Type = 0;

            byte modifiedDirection = 0;

            switch (e.KeyCode)
            {
                case Keys.W:
                    modifiedDirection += (byte)Directions.Up;
                    break;
                case Keys.A:
                    modifiedDirection += (byte)Directions.Left;
                    break;
                case Keys.S:
                    modifiedDirection += (byte)Directions.Down;
                    break;
                case Keys.D:
                    modifiedDirection += (byte)Directions.Right;
                    break;
                case Keys.Z:
                    modifiedDirection += (byte)Directions.Up + (byte)Directions.Left + (byte)Directions.Down + (byte)Directions.Right;
                    break;
                case Keys.R:
                    // Mark the cell with every direction to change
                    // then change the current cell Type to these direction
                    // that allows to change all the neighbours and
                    // remove the cell because it will be marked as 'no way to go'.
                    modifiedDirection += (byte)Directions.Up + (byte)Directions.Left + (byte)Directions.Down + (byte)Directions.Right;
                    cell.Type = modifiedDirection;
                    break;
            }

            if ((modifiedDirection & (byte)Directions.Up) != 0)
            {
                cell.Type ^= (uint)Directions.Up;
                // Neighbour Cell
                Cell neighbourCell = GetCell(new GridLocation(cell.Location.X, cell.Location.Y - 1, cell.Location.Z, cell.Location.Level));
                if (neighbourCell.ID == -1)
                {
                    neighbourCell.ID = NewCellID;
                    neighbourCell.Type = 0;
                }
                if ((cell.Type & (uint)Directions.Up) != (neighbourCell.Type & (uint)Directions.Down))
                    neighbourCell.Type ^= (uint)Directions.Down;
                if (neighbourCell.Type == 0)
                    RemoveCell(neighbourCell);
                else
                    AddCell(neighbourCell);
            }

            if ((modifiedDirection & (byte)Directions.Down) != 0)
            {
                cell.Type ^= (uint)Directions.Down;
                // Neighbour Cell
                Cell neighbourCell = GetCell(new GridLocation(cell.Location.X, cell.Location.Y + 1, cell.Location.Z, cell.Location.Level));
                if (neighbourCell.ID == -1)
                {
                    neighbourCell.ID = NewCellID;
                    neighbourCell.Type = 0;
                }
                if ((cell.Type & (uint)Directions.Down) != (neighbourCell.Type & (uint)Directions.Up))
                    neighbourCell.Type ^= (uint)Directions.Up;
                if (neighbourCell.Type == 0)
                    RemoveCell(neighbourCell);
                else
                    AddCell(neighbourCell);
            }

            if ((modifiedDirection & (byte)Directions.Left) != 0)
            {
                cell.Type ^= (uint)Directions.Left;
                // Neighbour Cell
                Cell neighbourCell = GetCell(new GridLocation(cell.Location.X - 1, cell.Location.Y, cell.Location.Z, cell.Location.Level));
                if (neighbourCell.ID == -1)
                {
                    neighbourCell.ID = NewCellID;
                    neighbourCell.Type = 0;
                }
                if ((cell.Type & (uint)Directions.Left) != (neighbourCell.Type & (uint)Directions.Right))
                    neighbourCell.Type ^= (uint)Directions.Right;
                if (neighbourCell.Type == 0)
                    RemoveCell(neighbourCell);
                else
                    AddCell(neighbourCell);
            }

            if ((modifiedDirection & (byte)Directions.Right) != 0)
            {
                cell.Type ^= (uint)Directions.Right;
                // Neighbour Cell
                Cell neighbourCell = GetCell(new GridLocation(cell.Location.X + 1, cell.Location.Y, cell.Location.Z, cell.Location.Level));
                if (neighbourCell.ID == -1)
                {
                    neighbourCell.ID = NewCellID;
                    neighbourCell.Type = 0;
                }
                if ((cell.Type & (uint)Directions.Right) != (neighbourCell.Type & (uint)Directions.Left))
                    neighbourCell.Type ^= (uint)Directions.Left;
                if (neighbourCell.Type == 0)
                    RemoveCell(neighbourCell);
                else
                    AddCell(neighbourCell);
            }


            if (cell.ID == -1)
                cell.ID = NewCellID;
            if (cell.Type == 0)
                RemoveCell(cell);
            else
                AddCell(cell);
            this.pbMap.Refresh();
        }
    }
}
