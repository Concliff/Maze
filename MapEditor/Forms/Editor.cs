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
        struct VritualPlayer
        {
            public GPS Position;
        };

        private VritualPlayer oPlayer;
        private Timer SystemTimer;
        private BlockEdit BlockEditForm;

        public Editor()
        {
            InitializeComponent();
            CustomInitialize();

            SystemTimer = new Timer();
            SystemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS;
            SystemTimer.Tick += new EventHandler(SystemTimerTick);
            SystemTimer.Start();

            oPlayer = new VritualPlayer();
            oPlayer.Position.Location = Map.Instance.GetStartPoint();
            oPlayer.Position.X = 25;
            oPlayer.Position.Y = 25;

            levelUpDown.Value = 0;

            RebuildGraphMap();
            this.Focus();
        }

        public void SystemTimerTick(object sender, EventArgs e)
        {
            uint MoveType = (uint)Directions.None;
            for (int counter = 0; counter < KeyMgr.KeysDownCount; ++counter)
                switch (KeyMgr.KeyDown(counter))
                {
                    case Keys.W: MoveType += (uint)Directions.Up; break;
                    case Keys.A: MoveType += (uint)Directions.Left; break;
                    case Keys.S: MoveType += (uint)Directions.Down; break;
                    case Keys.D: MoveType += (uint)Directions.Right; break;
                }
            MovementAction(MoveType);

            // Get last pressed Key
            switch (KeyMgr.ExtractKeyPressed())
            {
                case Keys.PageUp:
                    int nextLevel = Map.Instance.CurrentLevel;
                    ++nextLevel;
                    Map.Instance.SetMap(Map.Instance.GetMap(), nextLevel);
                    oPlayer.Position.Location.Level = nextLevel;
                    RebuildGraphMap();
                    break;
                case Keys.PageDown:
                    int previousLevel = Map.Instance.CurrentLevel;
                    --previousLevel;
                    if (previousLevel < 0)
                        previousLevel = 0;
                    Map.Instance.SetMap(Map.Instance.GetMap(), previousLevel);
                    oPlayer.Position.Location.Level = previousLevel;
                    RebuildGraphMap();
                    break;
            }

        }
        private void MovementAction(uint MoveType)
        {
            if ((MoveType & (uint)Directions.Right) != 0)
                oPlayer.Position.X += GlobalConstants.MOVEMENT_STEP_PX * 2;
            if ((MoveType & (uint)Directions.Left) != 0)
                oPlayer.Position.X -= GlobalConstants.MOVEMENT_STEP_PX * 2;
            if ((MoveType & (uint)Directions.Up) != 0)
                oPlayer.Position.Y -= GlobalConstants.MOVEMENT_STEP_PX * 2;
            if ((MoveType & (uint)Directions.Down) != 0)
                oPlayer.Position.Y += GlobalConstants.MOVEMENT_STEP_PX * 2;

            if (oPlayer.Position.X > GlobalConstants.CELL_WIDTH)
            {
                ++oPlayer.Position.Location.X;
                oPlayer.Position.X -= GlobalConstants.CELL_WIDTH;
            }

            if (oPlayer.Position.X < 0)
            {
                --oPlayer.Position.Location.X;
                oPlayer.Position.X = GlobalConstants.CELL_WIDTH + oPlayer.Position.X;
            }

            if (oPlayer.Position.Y > GlobalConstants.CELL_HEIGHT)
            {
                ++oPlayer.Position.Location.Y;
                oPlayer.Position.Y -= GlobalConstants.CELL_HEIGHT;
            }

            if (oPlayer.Position.Y < 0)
            {
                --oPlayer.Position.Location.Y;
                oPlayer.Position.Y = GlobalConstants.CELL_HEIGHT + oPlayer.Position.Y;
            }

            RebuildGraphMap();
        }

        public void RebuildGraphMap()
        {
            Graphics gGraphic;
            gGraphic = this.CreateGraphics();
            this.SuspendLayout();
            GridLocation PBLocation = new GridLocation();
            Cell Block = new Cell();
            // CellGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    int x, y;
                    x = (i - 1) * GlobalConstants.CELL_WIDTH - (oPlayer.Position.X - GlobalConstants.CELL_WIDTH / 2) - this.pbRightPanel.Size.Width / 2;
                    y = (j - 1) * GlobalConstants.CELL_HEIGHT - (oPlayer.Position.Y - GlobalConstants.CELL_HEIGHT / 2) + FormTitleBarSize;
                    PBLocation.X = oPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = oPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = oPlayer.Position.Location.Z;
                    PBLocation.Level = oPlayer.Position.Location.Level;
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
            gGraphic.Dispose();
            this.ResumeLayout();
        }

        void BlockClick(object sender, System.EventArgs e)
        {
            GridLocation CursorGPS = oPlayer.Position.Location;

            // Calculate GPS of mouse click location by distance beetween player postion, 
            // form center point and MouseClick point
            CursorGPS.X += (int)Math.Floor((oPlayer.Position.X + (Cursor.Position.X - this.Location.X -
                (this.PlayerPB.Location.X + this.PlayerPB.Size.Width / 2 + FormBorderBarSize))) /
                (double)GlobalConstants.CELL_WIDTH);

            CursorGPS.Y += (int)Math.Floor((oPlayer.Position.Y + (Cursor.Position.Y - this.Location.Y -
                (this.PlayerPB.Location.Y + this.PlayerPB.Size.Height / 2 + FormTitleBarSize))) /
                (double)GlobalConstants.CELL_HEIGHT);

            Cell Block = Map.Instance.GetCell(CursorGPS);

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

        void MapEditorFormClosing(object sender, FormClosingEventArgs e)
        {
            Map.Instance.Dispose();
        }
    }
}