using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Maze.Classes;
using System.Windows.Forms;


namespace Maze.Forms
{
    public partial class MapEditor : MazeForm
    {
        private Player VirtualPlayer;
        private Timer SystemTimer;
        private BlockEdit BlockEditForm;

        public MapEditor()
        {
            InitializeComponent();
            CustomInitialize();

            SystemTimer = new Timer();
            SystemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS;
            SystemTimer.Tick += new EventHandler(SystemTimerTick);
            SystemTimer.Start();

            VirtualPlayer = new Player();

            RebuildFormMap();
        }

        public void SystemTimerTick(object sender, EventArgs e)
        {
            byte MoveType = 0;
            SetBit(ref MoveType, (byte)Directions.None);
            for (int counter = 0; counter < KeyMgr.KeysDownCount; ++counter)
                switch (KeyMgr.KeyDown(counter))
                {
                    case Keys.W: SetBit(ref MoveType, (byte)Directions.Up); break;
                    case Keys.A: SetBit(ref MoveType, (byte)Directions.Left); break;
                    case Keys.S: SetBit(ref MoveType, (byte)Directions.Down); break;
                    case Keys.D: SetBit(ref MoveType, (byte)Directions.Right); break;
                }
            MovementAction(MoveType);

            switch (KeyMgr.ExtractKeyPressed())
            {
                case Keys.Space: RebuildFormMap(); break;
            }
        }
        private void MovementAction(byte MoveType)
        {

            if (!HasBit(MoveType, (byte)Directions.Up) && !HasBit(MoveType, (byte)Directions.Down) &&
                !HasBit(MoveType, (byte)Directions.Right) && !HasBit(MoveType, (byte)Directions.Left))
                return;


            GridGPS OldPosition = new GridGPS();
            OldPosition = VirtualPlayer.CopyGridGPS();
            VirtualPlayer.MovementAction(MoveType);

            if (OldPosition.Equals(VirtualPlayer.Position))
                return;
            RebuildFormMap();
        }

        public void RebuildFormMap()
        {
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();
            // GridMap
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    int x, y;
                    x = i + (i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (VirtualPlayer.Position.X - GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2);
                    y = j + (j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (VirtualPlayer.Position.Y - GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2);
                    PBLocation.X = VirtualPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = VirtualPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = 0;
                    PBLocation.Map = 0;
                    Block = GetWorldMap().GetGridMapByGPS(PBLocation);
                    Block.Location = PBLocation;

                    this.GridMapArray[i, j].Tag = Block;
                    this.GridMapArray[i, j].Image = GetWorldMap().GetPictureByType(Block.Type);
                    this.GridMapArray[i, j].Location = new System.Drawing.Point(x, y);
                    
                    // Draw Start Block
                    if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.IsStart))
                        StartPB.Location = new Point(x + 5, y + 5);
                    // Draw Finish Block
                    if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.IsFinish))
                        FinishPB.Location = new Point(x + 5, y + 5);
                }
        }

        void BlockClick(object sender, System.EventArgs e)
        {
            //this.BackColor = Color.Red;
            GridMap Block = (GridMap)((PictureBox)sender).Tag;

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

        private void MapEditor_Load(object sender, EventArgs e)
        {
            SetNextAction(WorldNextAction.ApplicationQuit);
        }

        void MapEditorFormClosing(object sender, FormClosingEventArgs e)
        {
            GetWorldMap().CloseCurrentMap();
        }
    }
}
