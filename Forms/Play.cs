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
        Deimos oDeimos;
        int tempCount;

        DateTime ProgramStartDateTime;  // Contains Time when game was started
        TimeSpan ProgramTime;           // Contains game run-time

        TimeControl SysTimer;
        Graphics TimeGraph;
        //public Map FormMap;

        public Play()
        {
            //FormMap = new Map();
            tempCount = 0;
            ProgramStartDateTime = DateTime.Now;
            InitializeComponent();
            CustomInitialize();
            AddControlsOrder();
            GridMapPB.BackColor = Color.Gray;
            SystemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS; // 50 ms
            SystemTimer.Start();
            label1.Text = "0";
            TimeGraph = this.RightPanelPB.CreateGraphics();
            
            // Visual is 11x7
            // Player grid is 7,5 (central)

            oPlayer = new Player();
            oDeimos = new Deimos();
            oDeimos.StartMotion();
            SysTimer = new TimeControl(this);

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

        void RightPanelPBPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("Time: " + (ProgramTime.Seconds + ProgramTime.Minutes*60).ToString(), new Font("Arial", 14), new SolidBrush(Color.White), 50, 30);
            e.Graphics.DrawString("Coins x " + (GetWorldMap().GetCoinsCount() - GetWorldMap().GetCollectedCoinsCount()).ToString(), 
                new Font("Arial", 14), new SolidBrush(Color.White), 50, 50);
        }

        public void SystemTimerTick(object sender, EventArgs e)
        {
            // return if a player reached finish block
            if (oPlayer.IsFinished())
                return;

            oDeimos.MovementAction();

            // Refresh game run-time
            ProgramTime = DateTime.Now.Subtract(ProgramStartDateTime);

            // Repaint Game stats panel
            this.RightPanelPB.Invalidate();

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

            // Review all the keys are currently down
            for (int counter = 0; counter < KeyMgr.KeysDownCount; ++counter)
                switch (KeyMgr.KeyDown(counter))
                {
                    // Catch moving keys
                    case Keys.W: SetBit(ref MoveType, (byte)Directions.Up); break;
                    case Keys.A: SetBit(ref MoveType, (byte)Directions.Left); break;
                    case Keys.S: SetBit(ref MoveType, (byte)Directions.Down); break;
                    case Keys.D: SetBit(ref MoveType, (byte)Directions.Right); break;
                }

            MovementAction(MoveType);

            // Get last pressed Key
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

            RebuildGraphMap();

            //World.GetPlayForm().label1.Text = oPlayer.Position.Location.X.ToString() + "\n" + oPlayer.Position.Location.Y.ToString() +
            //    "\n" + oPlayer.Position.X.ToString() + "\n" + oPlayer.Position.Y.ToString();// "move";
            //label1.Text = tempCount.ToString();
        }

        private void Play_Paint(object sender, PaintEventArgs e)
        {
            RebuildGraphMap();
            ++tempCount;
        }

        /// <summary>
        /// PlayForm Moving Handler
        /// </summary>
        /// <param name="MoveType">Flags of direction</param>
        private void MovementAction(byte MoveType)
        {
            // Check if moving occurs
            if (!HasBit(MoveType, (byte)Directions.Up) && !HasBit(MoveType, (byte)Directions.Down) &&
                !HasBit(MoveType, (byte)Directions.Right) && !HasBit(MoveType, (byte)Directions.Left))
                return;

            oPlayer.MovementAction(MoveType);

            if (oPlayer.IsFinished())
                MessageBox.Show("FINISH");
        }
        /// <summary>
        /// RePaint PlayForm map pictures.
        /// Include images of the player, blocks and objects on a block
        /// </summary>
        private void RebuildGraphMap()
        {
            this.SuspendLayout();
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();
            // GridMapGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    // Calculated location point for every block
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
                    // Draw Coin if not collected
                    if (HasBit(Block.Attribute, (byte) Attributes.HasCoin) &&
                        !GetWorldMap().IsCoinCollected(Block))
                    {
                        Graphics g = Graphics.FromImage(GetWorldMap().CoinImage);
                        g = this.CreateGraphics();
                        g.DrawImage(GetWorldMap().CoinImage, x + 15, y + 10, 20, 30);
                        g.Dispose();
                    }
                    // Draw Deimos
                    if (oDeimos.Position.Location.Equals(Block.Location))
                    {
                        Graphics g = Graphics.FromImage(GetWorldMap().DeimosImage);
                        g = this.CreateGraphics();
                        g.DrawImage(GetWorldMap().DeimosImage,
                            x + oDeimos.Position.X - GetWorldMap().DeimosImage.Size.Width /2,
                            y + oDeimos.Position.Y - GetWorldMap().DeimosImage.Size.Height /2,
                            GetWorldMap().DeimosImage.Size.Width, GetWorldMap().DeimosImage.Size.Height);
                        g.Dispose();
                    }
                }
            this.ResumeLayout();
        }

    }
}
