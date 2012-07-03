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
            Random,
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

        FormInterface CurrentInterface;

        DateTime LastTickTime;

        private int bonusGenerateTimer;
        private BonusEffect[] bonusEffects;

        private byte aurasCount;

        public Play()
        {
            tempCount = 0;
            ProgramStartDateTime = DateTime.Now;
            LastTickTime = DateTime.Now;

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

            bonusGenerateTimer = 5000;
            // Define bonus effects
            bonusEffects = new BonusEffect[]
            {
                new BonusEffect(2 , true),      // Sprint
                new BonusEffect(3 , true),      // Icy Wind
                new BonusEffect(5 , true),      // Thickener
                new BonusEffect(6 , true),      // A Cap of Invisibility
            };

            aurasCount = 0;
        }

        public void OnEffectApplied(EffectHolder effectHolder)
        {
            AuraIconPB[aurasCount].Tag = effectHolder;
            AuraIconPB[aurasCount].Image = PictureManager.EffectImages[effectHolder.EffectInfo.ID].Aura;
            AurasToolTip.SetToolTip(AuraIconPB[aurasCount], effectHolder.EffectInfo.EffectName + "\n"
                + effectHolder.EffectInfo.Description);
            AuraIconPB[aurasCount].Show();

            ++aurasCount;
        }

        public void OnEffectRemoved(EffectHolder effectHolder)
        {
            for (int i = 0; i < aurasCount; ++i)
            {
                if (AuraIconPB[i].Tag == effectHolder)
                {
                    for (int j = i; j < aurasCount; ++j)
                    {
                        AuraIconPB[j].Tag = AuraIconPB[j + 1].Tag;
                        AuraIconPB[j].Image = AuraIconPB[j + 1].Image;
                        AurasToolTip.SetToolTip(AuraIconPB[j], AurasToolTip.GetToolTip(AuraIconPB[j + 1]));
                    }
                    --aurasCount;
                    AuraIconPB[aurasCount].Hide();
                    break;
                }
            }
        }

        ~Play()
        {
            SystemTimer.Stop();
        }

        #region Play Form Events

        private void Play_Load(object sender, EventArgs e)
        {
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
            if (sender == MenuNewGamePB)
            {
                SetInterface(FormInterface.NewGame);
            }
            else if (sender == MenuRandomGamePB)
            {
                SetInterface(FormInterface.Random);
            }
            else if (sender == PauseResumePB)
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
            CurrentInterface = NewInterface;
            ChangeInterface(NewInterface, true);
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
                                LeftPanelPB.Invalidate();
                                for (int i = 0; i < AuraIconPB.Count(); ++i)
                                {
                                    AuraIconPB[i].Hide();
                                    AuraIconPB.Count();
                                }
                             }

                            MenuNewGamePB.Show();
                            MenuRandomGamePB.Show();
                            MenuHighScoresPB.Show();
                            MenuQuitPB.Show();

                        }
                        else
                        {
                            MenuNewGamePB.Visible = false;
                            MenuRandomGamePB.Hide();
                            MenuHighScoresPB.Hide();
                            MenuQuitPB.Hide();
                        }
                        break;
                    }
                case FormInterface.NewGame:
                    {
                        if (Show)
                        {
                            // Create Map and units
                            CreateWorldMap();               // Create Map
                            GetWorldMap().SetMap(0);

                            SetInterface(FormInterface.Play);
                        }

                        break;
                    }
                case FormInterface.Random:
                    {
                        if (Show)
                        {
                            // Create Map and units
                            CreateWorldMap();               // Create Map
                            GetWorldMap().GenerateRandomMap();

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

                            CreateObjectContainer();

                            player = new Slug();

                            GetWorldMap().FillMapWithUnits(); // Add units to map
                            GetWorldMap().FillMapWithObjects();// Add objects
                            GetObjectContainer().StartMotion();

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
            MenuRandomGamePB.Invalidate();
        }

        #endregion

        public void SystemTimerTick(object sender, EventArgs e)
        {
            if (!PlayStarted)
                return;

            // Get last pressed Key
            switch (KeyMgr.ExtractKeyPressed())
            {
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
            int tickTime = DateTime.Now.Subtract(LastTickTime).Milliseconds;
            GetObjectContainer().UpdateState(tickTime);
            LastTickTime = DateTime.Now;

            // Repaint Game stats panel
            this.RightPanelPB.Invalidate();

            this.LeftPanelPB.Invalidate();

            //Repaing auras PB
            for (int i = 0; i < aurasCount; ++i)
                AuraIconPB[i].Invalidate();

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

            // Generate Bonus
            if (bonusGenerateTimer <= 0)
            {
                // Define location of the bonus
                // in 3 Blocks radius range
                GridGPS bonusGridGPS;
                // Try 10 times to find appropriate point
                for (int i = 0; i < 10; ++i)
                {
                    bonusGridGPS = player.Position;
                    short xDiff = (short)Random.Int(6);
                    short yDiff = (short)Random.Int(6);
                     bonusGridGPS.Location.X += xDiff - 3;
                    bonusGridGPS.Location.Y += yDiff - 3;

                    // If current GPS doesn't have a map block
                    if (GetWorldMap().GetGridMap(bonusGridGPS.Location).ID == -1)
                        continue;
                    // else
                    bonusGridGPS.X = Random.Int(30) + 10;
                    bonusGridGPS.Y = Random.Int(30) + 10;

                    Bonus newBonus = new Bonus(bonusGridGPS);

                    // Get Random effect
                    BonusEffect eff = bonusEffects[Random.Int(bonusEffects.Count())];
                    newBonus.SetEffect(eff);
                    bonusGenerateTimer = Random.Int(3000, 5000);
                    // leave cycle
                    break;
                }
            }
            else
                bonusGenerateTimer -= tickTime;

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
                if (GetWorldMap().IsRandom())
                {
                    // Regenerate Map and Objects
                    CreateWorldMap();               // Create New Map
                    GetWorldMap().GenerateRandomMap();

                    GetObjectContainer().ClearEnvironment();

                    GetWorldMap().FillMapWithUnits();
                    GetWorldMap().FillMapWithObjects();
                    GetObjectContainer().StartMotion();
                }
                else
                {
                    // TODO: clear objects for past levels
                    int currentMap = GetWorldMap().GetMap();
                    int currentLevel = GetWorldMap().GetLevel();

                    if (++currentLevel < GetWorldMap().GetLevelCount())
                        GetWorldMap().SetMap(currentMap, currentLevel);
                }

                player.LevelChanged();
                player.AddPoints(30);
            }
        }

        public Slug GetPlayer()
        {
            return player;
        }
    }
}
