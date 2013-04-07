using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Classes;
using System.Diagnostics;

namespace Maze.Forms
{
    public partial class Play : MazeForm
    {
        /// <summary>
        /// Allowed the count of spell on the bottom Spell Panel
        /// </summary>
        public static int MAX_SPELLS_COUNT = 5;

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

        private bool gamePaused;
        private bool playStarted;

        /// <summary>
        /// Diagnostic tool for tracking the total Play time
        /// </summary>
        private Stopwatch gameTime;

        /// <summary>
        /// The time of the last update tick (in milliseconds).
        /// </summary>
        private long lastTickTime;

        private FormInterface CurrentInterface;

        private int bonusGenerateTimer;
        private BonusEffect[] bonusEffects;

        private byte aurasCount;

        public Slug Player
        {
            get;
            private set;
        }

        public Play()
        {
            // Visual is 11x7
            // Player grid is 7,5 (central)

            // |------------------------>
            // |                        X
            // |
            // |        Blocks 50x50 px
            // |
            // |
            // | Y
            //

            this.gameTime = new Stopwatch();

            this.gamePaused = false;
            this.playStarted = false;

            InitializeComponent();
            CustomInitialize();
            AddControlsOrder();
            GridMapPB.BackColor = Color.Gray;
            this.systemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS;
            
            //RebuildGraphMap();
            CurrentInterface = FormInterface.MainMenu;
            SetInterface(FormInterface.MainMenu);

            this.bonusGenerateTimer = 5000;

            // Define bonus effects
            this.bonusEffects = new BonusEffect[]
            {
                // Open Bonuses
                new BonusEffect(2 , true),      // Sprint
                new BonusEffect(3 , true),      // Icy Wind
                new BonusEffect(5 , true),      // Thickener
                new BonusEffect(6 , true),      // A Cap of Invisibility
                new BonusEffect(10 , true),     // Replenishment

                // Hidden Bonuses
                new BonusEffect(2 , false),      // Sprint
                new BonusEffect(5 , false),      // Thickener
                new BonusEffect(6 , false),      // A Cap of Invisibility
                new BonusEffect(7 , false),      // Mind Inverter
                new BonusEffect(8 , false),      // Slimy Clone
                new BonusEffect(12 , false),     // Smoke Bomb
                new BonusEffect(14 , false),     // Magical Leap
                new BonusEffect(11 , false),      // Shield
            };

            aurasCount = 0;
        }

        public void OnEffectApplied(object sender, EffectEventArgs e)
        {
            EffectHolder effectHolder = e.holder;

            if(effectHolder.EffectInfo.HasAttribute(EffectAttributes.HiddenAura))
                return;

            AuraIconPB[aurasCount].Tag = effectHolder;
            AuraIconPB[aurasCount].Image = PictureManager.EffectImages[effectHolder.EffectInfo.ID].Aura;
            AurasToolTip.SetToolTip(AuraIconPB[aurasCount], effectHolder.EffectInfo.EffectName + "\n"
                + effectHolder.EffectInfo.Description);
            AuraIconPB[aurasCount].Show();

            ++aurasCount;
        }

        public void OnEffectRemoved(object sender, EffectEventArgs e)
        {
            EffectHolder effectHolder = e.holder;

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

        /// <summary>
        /// Add new Spell into spell bar. If Spell Slot value is set and that slot is occupied, Spell is replaced.
        /// </summary>
        /// <param name="effectEntry">related EffectEntry of the spell</param>
        /// <param name="isPermanent">Is Spell Permanent or Disposable</param>
        /// <param name="spellSlotNumber">Spell slot on spell bar</param>
        public void AddSpell(EffectEntry effectEntry, bool isPermanent = false, int spellSlotNumber = -1)
        {
            // Check the validity of the input parameters
            if (spellSlotNumber < -1 || spellSlotNumber >= MAX_SPELLS_COUNT)
                return;

            int spellSlot = spellSlotNumber;
            bool isExist = false;

            // Find first free slot
            if (spellSlot == -1)
            {
                int firstFree = -1;
                for (int i = 0; i < MAX_SPELLS_COUNT; ++i)
                {
                    if (firstFree == -1 && SpellBarPB[i].RelatedEffect.ID == 0)
                        firstFree = i;

                    // Can not exist two Permanent/Disposable spells at the same time
                    if (effectEntry.ID == SpellBarPB[i].RelatedEffect.ID && isPermanent == SpellBarPB[i].IsPermanentSpell)
                    {
                        isExist = true;
                    }
                }

                spellSlot = firstFree;
            }

            // when free slot is found
            // and the played do not have this spell already
            if (!isExist && spellSlot != -1)
            {
                SpellBarPB[spellSlot].RelatedEffect = effectEntry;
                SpellBarPB[spellSlot].Image = PictureManager.EffectImages[effectEntry.ID].Aura;
                SpellBarPB[spellSlot].IsPermanentSpell = isPermanent;
                SpellBarPB[spellSlot].Show();
                AurasToolTip.SetToolTip(SpellBarPB[spellSlot], effectEntry.EffectName + "\n"
                    + effectEntry.Description);
            }
        }

        /// <summary>
        /// Clear the spell slot.
        /// </summary>
        /// <param name="spellSlot">Specific Slot</param>
        /// <returns></returns>
        private bool RemoveSpell(int spellSlot)
        {
            // Valid spell slot
            if (spellSlot < 0 || spellSlot >= MAX_SPELLS_COUNT)
                return false;

            // Effect exists
            if (SpellBarPB[spellSlot].RelatedEffect.ID == 0)
                return false;

            // Reset & Hide the PictureBox
            SpellBarPB[spellSlot].RelatedEffect = new EffectEntry();
            SpellBarPB[spellSlot].IsPermanentSpell = false;
            SpellBarPB[spellSlot].Hide();

            return true;
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
                        this.gameTime.Reset();
                        if (Show)
                        {
                            if (this.playStarted)    // When paused
                            {
                                this.playStarted = false;        // Stop the game
                                this.systemTimer.Stop();

                                // Clean Form Controls
                                GridMapPB.Invalidate();
                                RightPanelPB.Invalidate();
                                LeftPanelPB.Invalidate();

                                ClearPlayerAurasAndSpells();
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
                            this.gameTime.Start();
                            // Create Map and units
                            worldMap.Reset();
                            worldMap.SetMap(0);

                            SetInterface(FormInterface.Play);
                        }

                        break;
                    }
                case FormInterface.Random:
                    {
                        if (Show)
                        {
                            this.gameTime.Start();

                            // Create Map and units

                            worldMap.GenerateRandomMap();

                            SetInterface(FormInterface.Play);
                        }
                        break;
                    }
                case FormInterface.Play:
                    {
                        if (!Show)
                            break;
                        gameTime.Start();
                        if (!this.playStarted)                   // Start New Game
                        {
                            this.playStarted = true;
                            this.lastTickTime = 0;

                            this.objectContainer.ClearEnvironment(true); // Remove all old objects and units
                            Player = new Slug();    // Create new Slug
                            Player.HookEvents();
                            this.worldMap.FillMapWithUnits(); // Add units to map
                            this.worldMap.FillMapWithObjects(); // Add objects
                            this.objectContainer.StartMotion();

                            // Events
                            Player.LocationChanged += new Maze.Classes.Object.PositionHandler(Player_OnLocationChanged);

                            this.systemTimer.Start();
                        }
                        if (this.playStarted && this.gamePaused)  //Continue Game
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
                            this.gameTime.Stop();

                            this.gamePaused = true;
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
                            this.gamePaused = false;
                            PausePB.Hide();
                            PauseResumePB.Hide();
                            PauseMainMenuPB.Hide();
                        }
                        break;
                    }
            }
            MenuRandomGamePB.Invalidate();
        }

        private void ClearPlayerAurasAndSpells()
        {
            for (int i = 0; i < AuraIconPB.Count(); ++i)
                AuraIconPB[i].Hide();

            aurasCount = 0;

            for (int i = 0; i < SpellBarPB.Count(); ++i)
            {
                RemoveSpell(i);
            }
        }

        #endregion

        /// <summary>
        /// Handle game update ticks.
        /// </summary>
        public void SystemTimerTick(object sender, EventArgs e)
        {
            // Do nothing when game is not started
            if (!this.playStarted)
                return;

            int usedSpellNumber = 0;
            // Get last pressed Key
            switch (KeyMgr.ExtractKeyPressed())
            {
                // Switch Play/Pause game mode
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

                // Digit Keys (Spell using)
                case Keys.D1:
                    usedSpellNumber = 1;
                    break;
                case Keys.D2:
                    usedSpellNumber = 2;
                    break;
                case Keys.D3:
                    usedSpellNumber = 3;
                    break;
                case Keys.D4:
                    usedSpellNumber = 4;
                    break;
                case Keys.D5:
                    usedSpellNumber = 5;
                    Player.CastEffect(8, Player);
                    break;
            }
            if (Player.IsAlive() && usedSpellNumber > 0)
                UseSpell(usedSpellNumber);

            if (this.gamePaused)
                return;

            // Calculate actual past time since last timer tick
            long currentTime = this.gameTime.ElapsedMilliseconds;
            int tickTime = (int)(currentTime - this.lastTickTime);
            // Update every object on map
            this.objectContainer.UpdateState(tickTime);

            this.lastTickTime = currentTime;

            // Generate Bonus
            if (bonusGenerateTimer <= 0)
            {
                // Define location of the bonus
                // in 3 Blocks radius range
                GPS bonusPosition;
                // Try 10 times to find appropriate point
                for (int i = 0; i < 10; ++i)
                {
                    bonusPosition = Player.Position;
                    short xDiff = (short)Random.Int(6);
                    short yDiff = (short)Random.Int(6);
                    bonusPosition.Location.X += xDiff - 3;
                    bonusPosition.Location.Y += yDiff - 3;

                    // If current GPS doesn't have a map block
                    if (worldMap.GetCell(bonusPosition.Location).ID == -1)
                        continue;

                    // else generate Bonus object
                    bonusPosition.X = Random.Int(30) + 10;
                    bonusPosition.Y = Random.Int(30) + 10;

                    Bonus newBonus = new Bonus(bonusPosition);

                    // Get Random effect
                    BonusEffect eff = bonusEffects[Random.Int(bonusEffects.Count())];
                    newBonus.SetEffect(eff);

                    // leave cycle
                    break;
                }

                bonusGenerateTimer = Random.Int(3000, 5000);
            }
            else
                bonusGenerateTimer -= tickTime;

            // GRAPHIC PART
            // Repaint display elements of Play form
            // in a particular order

            // Redraw Game stats panel
            this.RightPanelPB.Invalidate();

            this.LeftPanelPB.Invalidate();

            // Redraw auras PB
            for (int i = 0; i < aurasCount; ++i)
                AuraIconPB[i].Invalidate();

            // Redraw Form Map
            GridMapPB.Invalidate();
        }

        private void UseSpell(int spellNumber)
        {
            EffectEntry effectEntry = SpellBarPB[spellNumber - 1].RelatedEffect;
            if (effectEntry.ID == 0)
                return;

            Player.CastEffect(effectEntry.ID, Player);

            if (!SpellBarPB[spellNumber - 1].IsPermanentSpell)
                RemoveSpell(spellNumber - 1);
        }

        private void SpellBarPB_MouseClick(object sender, MouseEventArgs e)
        {
            SpellBarPictureBox clickedSpellPB = (SpellBarPictureBox)sender;

            // No effect is linked
            if (clickedSpellPB.RelatedEffect.ID == 0)
                return;

            UseSpell(clickedSpellPB.SpellNumber);
        }

        private void Player_OnLocationChanged(object sender, PositionEventArgs e)
        {
            // Check finish point
            if (worldMap.DropsRemain == 0 &&
                worldMap.GetCell(Player.Position.BlockID).HasAttribute(CellAttributes.IsFinish))
            {
                // Random Map: regenerate level and create objects
                if (worldMap.IsRandom())
                {
                    // Regenerate Map and Objects
                    worldMap.GenerateRandomMap();

                    objectContainer.ClearEnvironment(false);

                    worldMap.FillMapWithUnits();
                    worldMap.FillMapWithObjects();
                    objectContainer.StartMotion();
                }
                // Normal Game: Set next Map level
                else
                {
                    // TODO: clear objects for past levels
                    int currentMap = worldMap.GetMap();
                    int currentLevel = worldMap.CurrentLevel;

                    if (++currentLevel < worldMap.LevelCount)
                        worldMap.SetMap(currentMap, currentLevel);
                }

                Player.LevelChanged();
                Player.AddPoints(30);
                ClearPlayerAurasAndSpells();
            }
        }
    }
}
