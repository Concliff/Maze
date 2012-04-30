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
            HighScores,
            Play,
            Pause,
            Quit,
        };

        bool GamePaused;
        bool PlayStarted;
        Slug player;
        int tempCount;

        DateTime ProgramStartDateTime;  // Contains Time when game was started
        TimeSpan ProgramTime;           // Contains game run-time

        //TimeControl SysTimer;         // Not implemented yet

        PictureManager PictureMgr;
        FormInterface CurrentInterface;

        DateTime LastTickTime;

        public Play()
        {
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

            label1.Text = "0";
            label1.Hide();
            
            // Visual is 11x7
            // Player grid is 7,5 (central)

            //SysTimer = new TimeControl(this);

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

        void Play_VisibleChanged(object sender, System.EventArgs e)
        {
            SetInterface(FormInterface.MainMenu);
        }

        #endregion

        #region Menu / Interface Actions

        void MenuItemClick(object sender, System.EventArgs e)
        {
            //PictureBox SenderPB = (PictureBox)sender;
            if (sender == MenuNewGamePB || sender == PauseResumePB || sender == MenuContinueGamePB)
            {
                SetInterface(FormInterface.Play);
            }
            else if (sender == PauseMainMenuPB)
            {
                SetInterface(FormInterface.MainMenu);
            }
            else if (sender == MenuQuitPB)
            {
                Application.Exit();
            }
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
                            if (PlayStarted)    // When paused
                            {
                                PlayStarted = false;        // Stop the game
                                SystemTimer.Stop();

                                // Clean Form Controls
                                GridMapPB.Invalidate();
                                RightPanelPB.Invalidate();
                            }

                            MenuNewGamePB.Show();
                            MenuContinueGamePB.Show();
                            MenuHighScoresPB.Show();
                            MenuQuitPB.Show();

                        }
                        else
                        {
                            MenuNewGamePB.Visible = false;
                            MenuContinueGamePB.Hide();
                            MenuHighScoresPB.Hide();
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

                        if (!PlayStarted)                   // Start New Game
                        {
                            PlayStarted = true;

                            CreateWorldMap();               // Create Map
                            GetWorldMap().SetMap(0);
                            CreateObjectContainer();
                            GetWorldMap().FillMapWithUnits(); // Add units to map
                            GetWorldMap().FillMapWithObjects();// Add objects
                            GetObjectContainer().StartMotion();

                            player = new Slug();

                            SystemTimer.Start();
                        }
                        if (PlayStarted && GamePaused)  //Continue Game
                        {
                            // Implemented in FormInterface.Pause !show
                            //GamePaused = false;
                        }
                        break;
                    }
                case FormInterface.Pause:
                    {
                        if (Show)
                        {
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
                    if (CurrentInterface == FormInterface.Play)
                    {
                        SetInterface(FormInterface.Pause);
                    }
                    else if (CurrentInterface == FormInterface.Pause)
                    {
                        SetInterface(FormInterface.Play);
                    }
                    break;
            }

            if (GamePaused)
                return;

            // return if a player reached finish block
            if (player.IsFinished())
                return;

            // Refresh game run-time
            ProgramTime = DateTime.Now.Subtract(ProgramStartDateTime);

            // Call for Update every Unit
            GetObjectContainer().UpdateState(DateTime.Now.Subtract(LastTickTime).Milliseconds);
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
            uint MoveType = (uint)Directions.None;

            // Review all the keys are currently down
            for (int counter = 0; counter < KeyMgr.KeysDownCount; ++counter)
                switch (KeyMgr.KeyDown(counter))
                {
                    // Catch moving keys
                    case Keys.W: MoveType += (uint)Directions.Up; break;
                    case Keys.A: MoveType += (uint)Directions.Left; break;
                    case Keys.S: MoveType += (uint)Directions.Down; break;
                    case Keys.D: MoveType += (uint)Directions.Right; break;
                }

            MovementAction(MoveType);

            // Redraw Form Map
            GridMapPB.Invalidate();
        }


        /// <summary>
        /// PlayForm Moving Handler
        /// </summary>
        /// <param name="MoveType">Flags of direction</param>
        private void MovementAction(uint MoveType)
        {
            // Check if moving occurs
            if ((MoveType & (uint)Directions.Up) == 0 && (MoveType & (uint)Directions.Down) == 0 &&
                (MoveType & (uint)Directions.Right) == 0 && (MoveType & (uint)Directions.Left) == 0)
                return;

            if (!player.IsAlive())
                return;

            player.MovementAction(MoveType);

            if (player.IsFinished())
            {
                int currentMap = GetWorldMap().GetMap();
                int currentLevel = GetWorldMap().GetLevel();
                
                if (++currentLevel < GetWorldMap().GetLevelCount())
                {
                    GetWorldMap().SetMap(currentMap, currentLevel);
                    player.LevelChanged();
                    player.AddPoints(30);
                }
            }
        }
    }
}
