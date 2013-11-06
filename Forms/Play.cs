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
    public partial class Play : Form
    {
        /// <summary>
        /// Allowed the count of spell on the bottom Spell Panel
        /// </summary>
        public static int MAX_SPELLS_COUNT = 5;

        public KeyManager KeyMgr;

        private enum GameStates
        {
            MainMenu,
            Game,
            Paused,
            SelectingMap,
            RandomGame,
            HighScores,
            Quit,
        }

        /// <summary>
        /// Indicates whether the game was paused
        /// </summary>
        private bool gamePaused;

        /// <summary>
        /// Indicates whether the Game (selected or random) was started.
        /// </summary>
        private bool playStarted;

        /// <summary>
        /// Diagnostic tool for tracking the total Play time
        /// </summary>
        private Stopwatch gameTime;

        /// <summary>
        /// The time of the last update tick (in milliseconds).
        /// </summary>
        private long lastTickTime;

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

            KeyMgr = new KeyManager();

            Initialize();
            this.pbGridMap.BackColor = Color.Gray;
            this.systemTimer.Interval = GlobalConstants.TIMER_TICK_IN_MS;
            
            //RebuildGraphMap();
            //CurrentInterface = FormInterface.MainMenu;
            //SetInterface(FormInterface.MainMenu);
            this.gameState = GameStates.MainMenu;

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

        #region Menu / Interface Actions

        private GameStates pr_gameState;
        /// <summary>
        /// Gets or sets the current state of the game.
        /// </summary>
        private GameStates gameState
        {
            get
            {
                return this.pr_gameState;
            }
            set
            {
                switch (value)
                {
                    case GameStates.MainMenu:
                        // If "Main Menu" was selected from Pause menu
                        if (this.pr_gameState == GameStates.Paused)
                        {
                            this.playStarted = false;        // Stop the game
                            this.systemTimer.Stop();

                            // Hide Pause Menu
                            this.pbPause.Hide();
                            this.pbPauseResume.Hide();
                            this.pbPauseMainMenu.Hide();

                            // Clean Form Controls
                            this.pbGridMap.Invalidate();
                            this.pbRightPanel.Invalidate();
                            this.pbLeftPanel.Invalidate();

                            ClearPlayerAurasAndSpells();
                        }
                        ShowMainMenu("New Game", "Random Map", "High Scores", "Quit");
                        break;

                    case GameStates.SelectingMap:
                        ShowMainMenu(Map.Instance.GetMapNames());
                        break;

                    case GameStates.Game:
                        // If continued from Pause
                        if (this.pr_gameState == GameStates.Paused)
                        {
                            this.pbPause.Hide();
                            this.pbPauseResume.Hide();
                            this.pbPauseMainMenu.Hide();
                            this.gamePaused = false;
                            this.gameTime.Start();
                            break;
                        }

                        // We are selecting Map
                        // Nothing to do
                        // StartNewGame is called when the Map has beed selected
                        else if (this.pr_gameState == GameStates.SelectingMap)
                            break;

                        StartNewGame(0);
                        break;

                    case GameStates.Paused:
                        this.gameTime.Stop();
                        this.gamePaused = true;
                        this.pbPause.Show();
                        this.pbPauseResume.Show();
                        this.pbPauseMainMenu.Show();

                        Graphics g;
                        g = this.pbPauseResume.CreateGraphics();
                        g.DrawString("Resume", this.fontMenu, this.brushMenuUnselected, 0, 0);

                        g = this.pbPauseMainMenu.CreateGraphics();
                        g.DrawString("Main Menu", this.fontMenu, this.brushMenuUnselected, 0, 0);
                        break;

                    case GameStates.RandomGame:
                        // RandomGame is just a menu title
                        // The actual state is Game
                        value = GameStates.Game;

                        StartNewGame(-1);
                        break;

                    case GameStates.Quit:
                        Application.Exit();
                        break;

                    default:
                        return;
                }

                this.pr_gameState = value;
            }
        }

        /// <summary>
        /// Draws Main Menu items with specified titles.
        /// </summary>
        /// <param name="titles">Array of the menu titles that will be displayed on the form</param>
        private void ShowMainMenu(params string[] titles)
        {
            if (titles.Length == 0)
                return;
            ClearMainMenu();
            this.pbMenuItems = new PictureBox[titles.Length];
            for (int i = 0; i < titles.Length; ++i)
            {
                this.pbMenuItems[i] = new PictureBox();
                this.pbMenuItems[i].Name = titles[i];
                this.pbMenuItems[i].Tag = i;
                this.pbMenuItems[i].Size = new Size(150, 30);
                this.pbMenuItems[i].Location = new Point(this.pbGridMap.Size.Width / 2 - 75, 40 * (i + 1));
                this.pbMenuItems[i].Paint += pbMenuItems_Paint;
                this.pbMenuItems[i].MouseEnter += pbMenuItems_MouseEnter;
                this.pbMenuItems[i].MouseLeave += pbMenuItems_MouseLeave;
                this.pbMenuItems[i].Click += MenuItem_Click;
                this.pbGridMap.Controls.Add(this.pbMenuItems[i]);
            }
        }

        /// <summary>
        /// Removes Main Menu title, unsubsribing from all events.
        /// </summary>
        private void ClearMainMenu()
        {
            if (this.pbMenuItems == null)
                return;

            foreach (PictureBox pb in this.pbMenuItems)
            {
                pb.Paint -= pbMenuItems_Paint;
                pb.MouseEnter -= pbMenuItems_MouseEnter;
                pb.MouseLeave -= pbMenuItems_MouseLeave;
                pb.Click -= MenuItem_Click;
                this.pbGridMap.Controls.Remove(pb);
            }

            this.pbMenuItems = null;
        }

        private void MenuItem_Click(object sender, System.EventArgs e)
        {
            PictureBox pbSender = (PictureBox)sender;
            if (pbSender == null)
                return;

            // Pause Menu
            if (this.gameState == GameStates.Paused)
            {
                if (pbSender == this.pbPauseResume)
                    this.gameState = GameStates.Game;
                else if (pbSender == this.pbPauseMainMenu)
                    this.gameState = GameStates.MainMenu;
                return;
            }

            // Index Selecting Menus

            if (pbSender.Tag == null || !(pbSender.Tag is int))
                return;

            int index = (int)pbSender.Tag;

            if (this.gameState == GameStates.MainMenu)
            {
                switch (index)
                {
                    case 0:     // New Game
                        this.gameState = GameStates.SelectingMap;
                        break;
                    case 1:     // Random Map
                        this.gameState = GameStates.RandomGame;
                        break;
                    case 2:     // HighScrores
                        this.gameState = GameStates.HighScores;
                        break;
                    case 3:     // Quit
                        this.gameState = GameStates.Quit;
                        break;
                }
            }
            else if (this.gameState == GameStates.SelectingMap)
            {
                this.gameState = GameStates.Game;
                StartNewGame(index);
            }

        }

        private void ClearPlayerAurasAndSpells()
        {
            for (int i = 0; i < this.pbAuraIcons.Count(); ++i)
                this.pbAuraIcons[i].Hide();

            aurasCount = 0;

            for (int i = 0; i < this.pbSpellBars.Count(); ++i)
            {
                RemoveSpell(i);
            }
        }

        #endregion

        private void StartNewGame(int mapIndex)
        {
            // Load selected map.
            Map.Instance.CurrentMap = mapIndex;

            Player = new Slug();
            Player.Create();
            // Events
            Player.LocationChanged += new Maze.Classes.Object.PositionHandler(Player_OnLocationChanged);

            // Load first level
            Map.Instance.CurrentLevel = 0;

            ClearMainMenu();

            this.gameTime.Start();
            this.playStarted = true;
            this.gamePaused = false;
            this.lastTickTime = 0;

            this.systemTimer.Start();
        }

        public void OnEffectApplied(object sender, EffectEventArgs e)
        {
            EffectHolder effectHolder = e.Holder;

            if(effectHolder.EffectInfo.HasAttribute(EffectAttributes.HiddenAura))
                return;

            this.pbAuraIcons[aurasCount].Tag = effectHolder;
            this.pbAuraIcons[aurasCount].Image = PictureManager.EffectImages[effectHolder.EffectInfo.ID].Aura;
            this.toolTipAuras.SetToolTip(this.pbAuraIcons[aurasCount], effectHolder.EffectInfo.EffectName + "\n"
                + effectHolder.EffectInfo.Description);
            this.pbAuraIcons[aurasCount].Show();

            ++aurasCount;
        }

        public void OnEffectRemoved(object sender, EffectEventArgs e)
        {
            EffectHolder effectHolder = e.Holder;

            for (int i = 0; i < aurasCount; ++i)
            {
                if (this.pbAuraIcons[i].Tag == effectHolder)
                {
                    for (int j = i; j < aurasCount; ++j)
                    {
                        this.pbAuraIcons[j].Tag = this.pbAuraIcons[j + 1].Tag;
                        this.pbAuraIcons[j].Image = this.pbAuraIcons[j + 1].Image;
                        this.toolTipAuras.SetToolTip(this.pbAuraIcons[j], this.toolTipAuras.GetToolTip(this.pbAuraIcons[j + 1]));
                    }
                    --aurasCount;
                    this.pbAuraIcons[aurasCount].Hide();
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
                    if (firstFree == -1 && this.pbSpellBars[i].RelatedEffect.ID == 0)
                        firstFree = i;

                    // Can not exist two Permanent/Disposable spells at the same time
                    if (effectEntry.ID == this.pbSpellBars[i].RelatedEffect.ID && isPermanent == this.pbSpellBars[i].IsPermanentSpell)
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
                this.pbSpellBars[spellSlot].RelatedEffect = effectEntry;
                this.pbSpellBars[spellSlot].Image = PictureManager.EffectImages[effectEntry.ID].Aura;
                this.pbSpellBars[spellSlot].IsPermanentSpell = isPermanent;
                this.pbSpellBars[spellSlot].Show();
                this.toolTipAuras.SetToolTip(this.pbSpellBars[spellSlot], effectEntry.EffectName + "\n"
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
            if (this.pbSpellBars[spellSlot].RelatedEffect.ID == 0)
                return false;

            // Reset & Hide the PictureBox
            this.pbSpellBars[spellSlot].RelatedEffect = new EffectEntry();
            this.pbSpellBars[spellSlot].IsPermanentSpell = false;
            this.pbSpellBars[spellSlot].Hide();

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
            this.gameState = GameStates.MainMenu;
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
                    if (this.gameState == GameStates.Game)
                    {
                        this.gameState = GameStates.Paused;
                    }
                    else if (this.gameState == GameStates.Paused)
                    {
                        this.gameState = GameStates.Game;
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
            if (Player.IsAlive && usedSpellNumber > 0)
                UseSpell(usedSpellNumber);

            if (this.gamePaused)
                return;

            // Calculate actual past time since last timer tick
            long currentTime = this.gameTime.ElapsedMilliseconds;
            int tickTime = (int)(currentTime - this.lastTickTime);
            // Update every object on map
            ObjectContainer.Instance.UpdateState(tickTime);

            this.lastTickTime = currentTime;

            // Bonuses are generated after the 5th level on Classic Map
            // and on every level on Random Map
            if (Map.Instance.CurrentLevel >= 3 || Map.Instance.IsRandom)
            {
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
                        if (Map.Instance.GetCell(bonusPosition.Location).ID == -1)
                            continue;

                        // else generate Bonus object
                        bonusPosition.X = Random.Int(30) + 10;
                        bonusPosition.Y = Random.Int(30) + 10;

                        // Get Random effect
                        BonusEffect effect = bonusEffects[Random.Int(bonusEffects.Count())];

                        Bonus newBonus = new Bonus();
                        newBonus.Create(bonusPosition, effect);

                        // leave cycle
                        break;
                    }

                    bonusGenerateTimer = Random.Int(3000, 5000);
                }
                else
                    bonusGenerateTimer -= tickTime;
            }

            // GRAPHIC PART
            // Repaint display elements of Play form
            // in a particular order

            // Redraw Game stats panel
            this.pbRightPanel.Invalidate();

            this.pbLeftPanel.Invalidate();

            // Redraw auras PB
            for (int i = 0; i < aurasCount; ++i)
                this.pbAuraIcons[i].Invalidate();

            // Redraw Form Map
            this.pbGridMap.Invalidate();
        }

        private void UseSpell(int spellNumber)
        {
            EffectEntry effectEntry = this.pbSpellBars[spellNumber - 1].RelatedEffect;
            if (effectEntry.ID == 0)
                return;

            Player.CastEffect(effectEntry.ID, Player);

            if (!this.pbSpellBars[spellNumber - 1].IsPermanentSpell)
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
            if (Map.Instance.DropsRemain == 0 &&
                Map.Instance.GetCell(Player.Position).HasAttribute(CellAttributes.IsFinish))
            {
                Player.AddPoints(30);
                ++Map.Instance.CurrentLevel;

                ClearPlayerAurasAndSpells();
            }
        }
    }
}
