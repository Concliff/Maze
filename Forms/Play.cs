using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class Play : MazeForm
    {
        Player oPlayer;
        //int GridMapWidth = GlobalConstants.GRIDMAP_WIDTH;
        //int GridMapHeight = GlobalConstants.GRIDMAP_HEIGHT;
        int tempCount;
        TimeControl SysTimer;
        BlockEdit newForm;
        //public Map FormMap;

        public Play()
        {
            //FormMap = new Map();
            tempCount = 0;
            InitializeComponent();
            CustomInitialize();
            AddControlsOrder();
            GridMapPB.BackColor = Color.Gray;
            SystemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS; // 50 ms
            SystemTimer.Start();
            label1.Text = "0";
            
            // Visual is 11x7
            // Player grid is 7,5 (central)

            oPlayer = new Player();
            SysTimer = new TimeControl(this);

            //RebuildFormMap();
            RebuildGraphMap();

        }

        ~Play()
        {
            SystemTimer.Stop();
        }

        private void Play_Load(object sender, EventArgs e)
        {
            SetNextAction(WorldNextAction.ApplicationQuit);
            RebuildGraphMap();
        }

        void Play_Shown(object sender, System.EventArgs e)
        {
            RebuildGraphMap();
        }

        public void SystemTimerTick(object sender, EventArgs e)
        {
            //this.SuspendLayout();
            // |------------------------>
            // |                        X
            // |
            // |        Blocks 50x50 px
            // |
            // |
            // | Y
            //
            // Moving
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
            if (!oPlayer.IsFinished())
                MovementAction(MoveType);

            switch (KeyMgr.ExtractKeyPressed())
            {
                case Keys.M:
                    if (KeyMgr.Control())
                    {
                        SetNextAction(WorldNextAction.MapEdit);
                        this.Close();
                    }
                    break;
            }

            label1.Text = tempCount.ToString();
        }

        private void Play_Paint(object sender, PaintEventArgs e)
        {
            RebuildGraphMap();
            ++tempCount;
        }

        private void MovementAction(byte MoveType)
        {
            if (!HasBit(MoveType, (byte)Directions.Up) && !HasBit(MoveType, (byte)Directions.Down) &&
                !HasBit(MoveType, (byte)Directions.Right) && !HasBit(MoveType, (byte)Directions.Left))
                return;
            
            GridGPS OldPosition = new GridGPS();
            OldPosition = oPlayer.CopyGridGPS();
            oPlayer.MovementAction(MoveType);
            
            if (OldPosition.Equals(oPlayer.Position))
                return;
            //World.GetPlayForm().label1.Text = oPlayer.Position.Location.X.ToString() + "\n" + oPlayer.Position.Location.Y.ToString() +
            //    "\n" + oPlayer.Position.X.ToString() + "\n" + oPlayer.Position.Y.ToString();// "move";
            //RebuildFormMap();
            RebuildGraphMap();

            if (oPlayer.IsFinished())
                MessageBox.Show("FINISH");
        }

        private void RebuildFormMap()
        {
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();
            // GridMap
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    int x, y;
                    x = GridMapPB.Location.X + (i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (oPlayer.Position.X - 25);
                    y = GridMapPB.Location.Y + (j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (oPlayer.Position.Y - 25);
                    PBLocation.X = oPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = oPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = 0;
                    PBLocation.Map = 0;
                    Block = GetWorldMap().GetGridMapByGPS(PBLocation);

                    this.GridMapArray[i, j].Tag = Block;
                    this.GridMapArray[i, j].Image = GetWorldMap().GetPictureByType(Block.Type);
                    this.GridMapArray[i, j].Location = new System.Drawing.Point(x, y);
                    
                    // Draw Start Block
                    if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.IsStart))
                    {
                        StartPB.Location = new Point(x + 5, y + 5);
                        //Graphics g = Graphics.FromImage(GetWorldMap().StartImage);
                        //g = this.CreateGraphics();
                        //g.DrawImage(GetWorldMap().StartImage, x + 5, y + 5, 40, 40);
                        //g.Dispose();
                    }
                    // Draw Finish Block
                    if (BinaryOperations.IsBit(Block.Attribute, (byte)Attributes.IsFinish))
                        FinishPB.Location = new Point(x + 5, y + 5);

                }
        }

        private void RebuildGraphMap()
        {
            this.SuspendLayout();
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();
            // GridMapGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    int x, y;
                    x = GridMapPB.Location.X + (i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (oPlayer.Position.X - 25);
                    y = GridMapPB.Location.Y + (j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (oPlayer.Position.Y - 25);
                    PBLocation.X = oPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = oPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = 0;
                    PBLocation.Map = 0;
                    Block = GetWorldMap().GetGridMapByGPS(PBLocation);

                    this.GridMapGraphic[i, j].Block = Block;
                    this.GridMapGraphic[i, j].Graphic = Graphics.FromImage(GetWorldMap().GetPictureByType(Block.Type));
                    this.GridMapGraphic[i, j].Graphic.Dispose();
                    this.GridMapGraphic[i, j].Graphic = this.CreateGraphics();
                    
                    this.GridMapGraphic[i, j].Graphic.DrawImage(GetWorldMap().GetPictureByType(Block.Type), x, y, GlobalConstants.GRIDMAP_BLOCK_WIDTH, GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
                    // Draw Start Block
                    if (HasBit(Block.Attribute, (byte)Attributes.IsStart))
                    {
                        //StartPB.Location = new Point(x + 5, y + 5);
                        Graphics g = Graphics.FromImage(GetWorldMap().StartImage);
                        g = this.CreateGraphics();
                        g.DrawImage(GetWorldMap().StartImage, x + 5, y + 5, 40, 40);
                        g.Dispose();
                    }
                    // Draw Finish Block
                    if (HasBit(Block.Attribute, (byte)Attributes.IsFinish))
                    {
                        //FinishPB.Location = new Point(x + 5, y + 5);
                        Graphics g = Graphics.FromImage(GetWorldMap().FinishImage);// Non indexed image
                        g = this.CreateGraphics();
                        g.DrawImage(GetWorldMap().FinishImage, x + 5, y + 5, 40, 40);
                        g.Dispose();
                    }
                }
            this.ResumeLayout();
        }

    }
}
