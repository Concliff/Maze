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
        private enum FormInterface
        {
            MainMenu,
            NewGame,
            Play,
            Pause,
            Quit,
        };

        bool GamePaused;
        bool PlayStarted;
        Player oPlayer;
        int tempCount;

        DateTime ProgramStartDateTime;  // Contains Time when game was started
        TimeSpan ProgramTime;           // Contains game run-time

        TimeControl SysTimer;
        Graphics TimeGraph;

        PictureManager PictureMgr;
        FormInterface CurrentInterface;

        DateTime LastTickTime;
        //public Map FormMap;

        public Play()
        {
            //FormMap = new Map();
            tempCount = 0;
            ProgramStartDateTime = DateTime.Now;
            LastTickTime = DateTime.Now;
            PictureMgr = new PictureManager();

            GamePaused = false;
            PlayStarted = false;

            InitializeComponent();
            CustomInitialize();
            AddControlsOrder();
            GridMapPB.BackColor = Color.Gray;
            SystemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS; // 50 ms
            SystemTimer.Start();
            //
            label1.Text = "0";
            TimeGraph = this.RightPanelPB.CreateGraphics();
            
            // Visual is 11x7
            // Player grid is 7,5 (central)

            oPlayer = new Player();
            PlayerPB.Hide();
            label1.Hide();

            // Test-created monsters
            new Phobos();
            SysTimer = new TimeControl(this);

            //RebuildGraphMap();
            CurrentInterface = FormInterface.MainMenu;
            SetInterface(FormInterface.MainMenu);
        }

        ~Play()
        {
            SystemTimer.Stop();
        }

        #region Play Form Events

        private void Play_Load(object sender, EventArgs e)
        {
            SetNextAction(WorldNextAction.ApplicationQuit);
            //RebuildGraphMap();
        }

        private void Play_Shown(object sender, System.EventArgs e)
        {
            this.Invalidate();
            //RebuildGraphMap();
        }

        void GridMapPB_Paint(object sender, PaintEventArgs e)
        {

            //SetInterface(CurrentInterface);
            //ChangeInterface(CurrentInterface, true);
        }

        private void Play_Paint(object sender, PaintEventArgs e)
        {
            //RebuildGraphMap();
            ++tempCount;
        }

        void Play_VisibleChanged(object sender, System.EventArgs e)
        {
            SetInterface(FormInterface.MainMenu);
        }

        void RightPanelPBPaint(object sender, PaintEventArgs e)
        {
            if (!PlayStarted)
                return;

            e.Graphics.DrawString("Time: " + (ProgramTime.Seconds + ProgramTime.Minutes*60).ToString(), new Font("Arial", 14), new SolidBrush(Color.White), 50, 30);
            e.Graphics.DrawString("Coins x " + (GetWorldMap().GetCoinsCount() - GetWorldMap().GetCollectedCoinsCount()).ToString(), 
                new Font("Arial", 14), new SolidBrush(Color.White), 50, 50);
        }

        #endregion

        #region Menu / Interface Actions

        void MenuItemClick(object sender, System.EventArgs e)
        {
            //PictureBox SenderPB = (PictureBox)sender;
            if (sender == MenuNewGamePB || sender == PauseResumePB || sender == MenuContinueGamePB)
                SetInterface(FormInterface.Play);
        }


        private void SetInterface(FormInterface NewInterface)
        {
            ChangeInterface(CurrentInterface, false);
            ChangeInterface(NewInterface, true);
            CurrentInterface = NewInterface;
        }

        private void ChangeInterface(FormInterface Interface, bool Show)
        {
            switch (Interface)
            {
                case FormInterface.MainMenu:
                    {
                        if (Show)
                        {
                            MenuNewGamePB.Show();
                            MenuContinueGamePB.Show();
                            MenuQuitPB.Show();
                            Graphics g;
                            /*g = this.MenuNewGamePB.CreateGraphics();
                            g.DrawString("New Game", MenuFont, MenuUnselectedBrush, 0, 0);

                            g = this.MenuContinueGamePB.CreateGraphics();
                            g.DrawString("Continue", MenuFont, MenuUnselectedBrush, 0, 0);

                            g = this.MenuQuitPB.CreateGraphics();
                            g.DrawString("Quit", MenuFont, MenuUnselectedBrush, 0, 0);
                             * */
                        }
                        else
                        {
                            MenuNewGamePB.Visible = false;
                            MenuContinueGamePB.Hide();
                            MenuQuitPB.Hide();
                        }
                        break;
                    }
                case FormInterface.NewGame:
                    {
                        if (Show)
                        {
                            SetInterface(FormInterface.Play);
                        }

                        break;
                    }
                case FormInterface.Play:
                    {
                        if (!Show)
                            break;

                        if (!PlayStarted)               // Start New Game
                        {
                            //ChangeInterface(FormInterface.MainMenu, false);
                            PlayStarted = true;
                            SystemTimer.Start();
                            GetUnitContainer().StartMotion();
                        }
                        if (PlayStarted && GamePaused)  //Continue Game
                        {
                            // Implemented in FormInterface.Pause !show
                            //GamePaused = false;
                        }

                        PlayerPB.Show();
                        break;
                    }
                case FormInterface.Pause:
                    {
                        if (Show)
                        {
                            PlayerPB.Hide();
                            GamePaused = true;
                            PausePB.Show();
                            PauseResumePB.Show();
                            PauseMainMenuPB.Show();

                            Graphics g;
                            g = this.PauseResumePB.CreateGraphics();
                            g.DrawString("Resume", MenuFont, MenuUnselectedBrush, 0, 0);

                            g = this.PauseMainMenuPB.CreateGraphics();
                            g.DrawString("Main Menu", MenuFont, MenuUnselectedBrush, 0, 0);
                        }
                        else
                        {
                            GamePaused = false;
                            PausePB.Hide();
                            PauseResumePB.Hide();
                            PauseMainMenuPB.Hide();
                        }
                        break;
                    }
            }
            MenuContinueGamePB.Invalidate();
        }

        void MenuItemMouseEnter(object sender, System.EventArgs e)
        {
            PictureBox SenderPB = (PictureBox)sender;
            Graphics g;
            g = SenderPB.CreateGraphics();
            g.DrawString(SenderPB.Name, MenuFont, MenuSelectedBrush, 0, 0);
        }

        void MenuItemMouseLeave(object sender, System.EventArgs e)
        {
            PictureBox SenderPB = (PictureBox)sender;
            Graphics g;
            g = SenderPB.CreateGraphics();
            g.DrawString(SenderPB.Name, MenuFont, MenuUnselectedBrush, 0, 0);
        }

        void MenuItemPaint(object sender, PaintEventArgs e)
        {
            PictureBox senderPB = (PictureBox)sender;
            e.Graphics.DrawString(senderPB.Name, MenuFont, MenuUnselectedBrush, 0, 0);
        }

        #endregion

        public void SystemTimerTick(object sender, EventArgs e)
        {
            if (!PlayStarted)
                return;

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
                case Keys.Escape:
                    SetInterface(FormInterface.Pause);
                    break;
            }

            if (GamePaused)
                return;

            // return if a player reached finish block
            if (oPlayer.IsFinished())
                return;

            // Refresh game run-time
            ProgramTime = DateTime.Now.Subtract(ProgramStartDateTime);

            // Call for Update every Unit
            GetUnitContainer().UpdateState(DateTime.Now.Subtract(LastTickTime).Milliseconds);
            LastTickTime = DateTime.Now;

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

            RebuildGraphMap();
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

            if (!oPlayer.IsAlive())
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
                    x = /*GridMapPB.Location.X*/ + (i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (oPlayer.Position.X - 25);
                    y = /*GridMapPB.Location.Y*/ + (j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (oPlayer.Position.Y - 25);
                    PBLocation.X = oPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = oPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = 0;
                    PBLocation.Map = 0;
                    Block = GetWorldMap().GetGridMapByGPS(PBLocation);

                    this.GridMapGraphic[i, j].Block = Block;
                    this.GridMapGraphic[i, j].Graphic = Graphics.FromImage(PictureMgr.GetPictureByType(Block.Type));
                    this.GridMapGraphic[i, j].Graphic.Dispose();
                    this.GridMapGraphic[i, j].Graphic = this.GridMapPB.CreateGraphics();
                    
                    this.GridMapGraphic[i, j].Graphic.DrawImage(PictureMgr.GetPictureByType(Block.Type), x, y, GlobalConstants.GRIDMAP_BLOCK_WIDTH, GlobalConstants.GRIDMAP_BLOCK_HEIGHT);
                    // Draw Start Block
                    if (HasBit(Block.Attribute, (byte)Attributes.IsStart))
                    {
                        //StartPB.Location = new Point(x + 5, y + 5);
                        Graphics g = Graphics.FromImage(PictureMgr.StartImage);
                        g = this.GridMapPB.CreateGraphics();
                        g.DrawImage(PictureMgr.StartImage, x + 5, y + 5, 40, 40);
                        g.Dispose();
                    }
                    // Draw Finish Block
                    if (HasBit(Block.Attribute, (byte)Attributes.IsFinish))
                    {
                        //FinishPB.Location = new Point(x + 5, y + 5);
                        Graphics g = Graphics.FromImage(PictureMgr.FinishImage);// Non indexed image
                        g = this.GridMapPB.CreateGraphics();
                        g.DrawImage(PictureMgr.FinishImage, x + 5, y + 5, 40, 40);
                        g.Dispose();
                    }
                    // Draw Coin if not collected
                    if (HasBit(Block.Attribute, (byte) Attributes.HasCoin) &&
                        !GetWorldMap().IsCoinCollected(Block))
                    {
                        Graphics g = Graphics.FromImage(PictureMgr.CoinImage);
                        g = this.GridMapPB.CreateGraphics();
                        g.DrawImage(PictureMgr.CoinImage, x + 15, y + 10, 20, 30);
                        g.Dispose();
                    }

                    // Draw Visible Units
                    List<Unit> Units = GetUnitContainer().GetAllUnitsByGPS(Block.Location);
                    for (int d = 0; d < Units.Count; ++d)
                    {
                        if (Units[d].GetUnitType() == UnitTypes.Player)
                            continue;

                        Image UnitImage;
                        switch (Units[d].GetUnitType())
                        {
                            case UnitTypes.Deimos: UnitImage = PictureMgr.DeimosImage; break;
                            case UnitTypes.Phobos: UnitImage = PictureMgr.PhobosImage; break;
                            default: UnitImage = PictureMgr.DeimosImage; break;
                        }

                        Graphics g = Graphics.FromImage(UnitImage);
                        g = this.GridMapPB.CreateGraphics();
                        g.DrawImage(UnitImage,
                            x + Units[d].Position.X - PictureMgr.DeimosImage.Size.Width / 2,
                            y + Units[d].Position.Y - PictureMgr.DeimosImage.Size.Height / 2,
                            UnitImage.Size.Width, UnitImage.Size.Height);
                        g.Dispose();
                    }

                    // Draw Player
                    if (oPlayer.IsAlive() && !PlayerPB.Visible)
                        PlayerPB.Show();
                    if (!oPlayer.IsAlive() && PlayerPB.Visible)
                        PlayerPB.Hide();
                }
            this.ResumeLayout();
        }
    }
}
