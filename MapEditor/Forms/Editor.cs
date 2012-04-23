﻿using System;
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
            public GridGPS Position;
        };

        private VritualPlayer oPlayer;
        private Timer SystemTimer;
        private BlockEdit BlockEditForm;

        private PictureManager PictureMgr;

        public Editor()
        {
            PictureMgr = new PictureManager();

            InitializeComponent();
            CustomInitialize();

            SystemTimer = new Timer();
            SystemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS;
            SystemTimer.Tick += new EventHandler(SystemTimerTick);
            SystemTimer.Start();

            oPlayer = new VritualPlayer();

            RebuildGraphMap();
            this.Focus();
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

        }
        private void MovementAction(byte MoveType)
        {
            if (HasBit(MoveType, (byte)Directions.Right))
                oPlayer.Position.X += GlobalConstants.MOVEMENT_STEP_PX * 2;
            if (HasBit(MoveType, (byte)Directions.Left))
                oPlayer.Position.X -= GlobalConstants.MOVEMENT_STEP_PX * 2;
            if (HasBit(MoveType, (byte)Directions.Up))
                oPlayer.Position.Y -= GlobalConstants.MOVEMENT_STEP_PX * 2;
            if (HasBit(MoveType, (byte)Directions.Down))
                oPlayer.Position.Y += GlobalConstants.MOVEMENT_STEP_PX * 2;

            if (oPlayer.Position.X > GlobalConstants.GRIDMAP_BLOCK_WIDTH)
            {
                ++oPlayer.Position.Location.X;
                oPlayer.Position.X -= GlobalConstants.GRIDMAP_BLOCK_WIDTH;
            }

            if (oPlayer.Position.X < 0)
            {
                --oPlayer.Position.Location.X;
                oPlayer.Position.X = GlobalConstants.GRIDMAP_BLOCK_WIDTH + oPlayer.Position.X;
            }

            if (oPlayer.Position.Y > GlobalConstants.GRIDMAP_BLOCK_HEIGHT)
            {
                ++oPlayer.Position.Location.Y;
                oPlayer.Position.Y -= GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
            }

            if (oPlayer.Position.Y < 0)
            {
                --oPlayer.Position.Location.Y;
                oPlayer.Position.Y = GlobalConstants.GRIDMAP_BLOCK_HEIGHT + oPlayer.Position.Y;
            }

            RebuildGraphMap();
        }

        public void RebuildGraphMap()
        {
            this.SuspendLayout();
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();
            // GridMapGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    int x, y;
                    x = (i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (oPlayer.Position.X - GlobalConstants.GRIDMAP_BLOCK_WIDTH / 2);
                    y = (j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (oPlayer.Position.Y - GlobalConstants.GRIDMAP_BLOCK_HEIGHT / 2);
                    PBLocation.X = oPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = oPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = 0;
                    PBLocation.Level = 0;
                    Block = Program.WorldMap.GetGridMap(PBLocation);

                    this.GridMapGraphic[i, j].Block = Block;
                    this.GridMapGraphic[i, j].Graphic = Graphics.FromImage(PictureMgr.GetPictureByType(Block.Type));
                    this.GridMapGraphic[i, j].Graphic.Dispose();
                    this.GridMapGraphic[i, j].Graphic = this.CreateGraphics();

                    this.GridMapGraphic[i, j].Graphic.DrawImage(PictureMgr.GetPictureByType(Block.Type), x, y, GlobalConstants.GRIDMAP_BLOCK_WIDTH, GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
                    // Draw Start Block
                    if (HasBit(Block.Attribute, (byte)Attributes.IsStart))
                    {
                        //StartPB.Location = new Point(x + 5, y + 5);
                        Graphics g = Graphics.FromImage(PictureMgr.StartImage);
                        g = this.CreateGraphics();
                        g.DrawImage(PictureMgr.StartImage, x + 5, y + 5, 40, 40);
                        g.Dispose();
                    }
                    // Draw Finish Block
                    if (HasBit(Block.Attribute, (byte)Attributes.IsFinish))
                    {
                        //FinishPB.Location = new Point(x + 5, y + 5);
                        Graphics g = Graphics.FromImage(PictureMgr.FinishImage);// Non indexed image
                        g = this.CreateGraphics();
                        g.DrawImage(PictureMgr.FinishImage, x + 5, y + 5, 40, 40);
                        g.Dispose();
                    }
                    // Draw Coin
                    if (HasBit(Block.Attribute, (byte)Attributes.HasCoin))
                    {
                        Graphics g = Graphics.FromImage(PictureMgr.CoinImage);
                        g = this.CreateGraphics();
                        g.DrawImage(PictureMgr.CoinImage, x + 15, y + 10, 20, 30);
                        g.Dispose();
                    }

                }
            this.ResumeLayout();
        }

        void BlockClick(object sender, System.EventArgs e)
        {
            GPS CursorGPS = oPlayer.Position.Location;

            // Calculate GPS of mouse click location by distance beetween player postion, 
            // form center point and MouseClick point
            CursorGPS.X += (int)Math.Floor((oPlayer.Position.X + (Cursor.Position.X - this.Location.X -
                (this.PlayerPB.Location.X + this.PlayerPB.Size.Width / 2 + FormBorderBarSize))) /
                (double)GlobalConstants.GRIDMAP_BLOCK_WIDTH);

            CursorGPS.Y += (int)Math.Floor((oPlayer.Position.Y + (Cursor.Position.Y - this.Location.Y -
                (this.PlayerPB.Location.Y + this.PlayerPB.Size.Height / 2 + FormTitleBarSize))) /
                (double)GlobalConstants.GRIDMAP_BLOCK_HEIGHT);

            GridMap Block = Program.WorldMap.GetGridMap(CursorGPS);

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
            Program.WorldMap.CloseCurrentMap();
        }
    }
}