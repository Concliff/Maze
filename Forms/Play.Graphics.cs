﻿using Maze.Classes;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Maze.Forms
{
    partial class Play
    {
        void RightPanelPB_Paint(object sender, PaintEventArgs e)
        {
            if (!this.playStarted)
                return;
            // Total game time
            e.Graphics.DrawString("Time: " + (this.gameTime.ElapsedMilliseconds / 1000).ToString(), new Font("Arial", 14), new SolidBrush(Color.White), 10, 30);
            // Ooze drops count to collect.
            e.Graphics.DrawString("Drops x " + Map.Instance.DropsRemain.ToString(),
                new Font("Arial", 14), new SolidBrush(Color.White), 10, 50);
            // Game Scores.
            e.Graphics.DrawString("Total scores: " + Player.Score, new Font("Arial", 12), new SolidBrush(Color.White), 10, 70);
        }

        void LeftPanelPB_Paint(object sender, PaintEventArgs e)
        {
            if (!this.playStarted)
                return;

            // Calcs Ooze energy percentage
            int playerOozePercent = Player.OozeEnergy * 100 / Slug.MaxOozeEnergy;
            Brush emptyBrush = new SolidBrush(Color.Black);
            Brush filledBrush = new SolidBrush(Color.Green);
            Rectangle oozeBar = new Rectangle(20, 100, 100, 30);
            Rectangle oozeAmount = new Rectangle(20, 100, 100 * playerOozePercent / 100, 30);

            // Draw green and black rectanges to show current Energy level.
            e.Graphics.FillRectangle(emptyBrush, oozeBar);
            e.Graphics.FillRectangle(filledBrush, oozeAmount);
            e.Graphics.DrawString(Player.OozeEnergy.ToString(), new Font("Arial", 14), new SolidBrush(Color.White), 50, 105);
        }

        void CellPB_Paint(object sender, PaintEventArgs e)
        {
            // Only when game is started
            if (this.playStarted)
                RebuildGraphMap(e.Graphics);
        }

        private void pbAuraIcons_Paint(object sender, PaintEventArgs e)
        {
            EffectHolder holder = (EffectHolder)((PictureBox)sender).Tag;
            int durationInSeconds = holder.Duration / 1000;

            // Draw Aura Timer
            // Skip if duration == -1 (One-tact effect)
            if (holder.EffectInfo.Duration == -1)
                return;
            // Alignment centre
            int x;
            if ((durationInSeconds) / 10 > 0)
                x = 9;
            else
                x = 15;

            e.Graphics.DrawString(durationInSeconds.ToString(), new Font("Arial", 16), new SolidBrush(Color.White), x, 50); 
        }

        private void SpellBarPB_Paint(object sender, PaintEventArgs e)
        {
            SpellBarPictureBox paintedSpellPB = (SpellBarPictureBox)sender;
            // Draw spell keyboard digits
            // Draw bigger white digits and smaller red one simulating text shadow effect.
            e.Graphics.DrawString(paintedSpellPB.SpellNumber.ToString(), new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.White), 35, -2);
            e.Graphics.DrawString(paintedSpellPB.SpellNumber.ToString(), new Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Red), 36, 0);
        }

        private void Play_Paint(object sender, PaintEventArgs e)
        {
            //RebuildGraphMap();
        }

        private void pbMenuItems_MouseEnter(object sender, System.EventArgs e)
        {
            PictureBox SenderPB = (PictureBox)sender;
            Graphics g;
            g = SenderPB.CreateGraphics();
            g.DrawString(SenderPB.Name, this.fontMenu, this.brushMenuSelected, 0, 0);
        }

        private void pbMenuItems_MouseLeave(object sender, System.EventArgs e)
        {
            PictureBox SenderPB = (PictureBox)sender;
            Graphics g;
            g = SenderPB.CreateGraphics();
            g.DrawString(SenderPB.Name, this.fontMenu, this.brushMenuUnselected, 0, 0);
        }

        private void pbMenuItems_Paint(object sender, PaintEventArgs e)
        {
            PictureBox senderPB = (PictureBox)sender;
            e.Graphics.DrawString(senderPB.Name, this.fontMenu, this.brushMenuUnselected, 0, 0);
        }

        /// <summary>
        /// RePaint PlayForm map pictures.
        /// Include images of the player, blocks and objects on a block
        /// </summary>
        private void RebuildGraphMap(Graphics gCellBP)
        {
            GridLocation PBLocation = new GridLocation();
            Cell Block = new Cell();

            List<Maze.Classes.Object> objectsOnMap = new List<Maze.Classes.Object>();

            // CellGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    // Calculated location point for every block
                    int x, y;
                    x = /*CellPB.Location.X*/ +(i - 1) * GlobalConstants.CELL_WIDTH - (Player.Position.X - 25);
                    y = /*CellPB.Location.Y*/ +(j - 1) * GlobalConstants.CELL_HEIGHT - (Player.Position.Y - 25);
                    PBLocation.X = Player.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = Player.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = Player.Position.Location.Z;
                    PBLocation.Level = Player.Position.Location.Level;
                    Block = Map.Instance.GetCell(PBLocation);

                    // Draw Grid Cell image
                    gCellBP.DrawImage(PictureManager.GetPictureByType(Block.Type), x, y, GlobalConstants.CELL_WIDTH, GlobalConstants.CELL_HEIGHT);


                    // Draw Start Block
                    if (Block.HasAttribute(CellAttributes.IsStart))
                    {
                        gCellBP.DrawImage(PictureManager.StartImage, x + 5, y + 5, 40, 40);
                    }

                    // Draw Finish Block
                    if (Block.HasAttribute(CellAttributes.IsFinish))
                    {
                        gCellBP.DrawImage(PictureManager.FinishImage, x + 5, y + 5, 40, 40);
                    }

                    // Include all objects in this grid
                    objectsOnMap.AddRange(ObjectContainer.Instance.GetObjects(Block.Location));

                }

            /* Draw Visible Objects
             * Order:
             * 1. Slime
             * 2. GridObjects
             * 3. Units
             * 4. Slug
             */
            Image objectImage;

            for (int j = 0; j < 4; ++j)
                for (int i = 0; i < objectsOnMap.Count; ++i)
                {
                    // Default NULL
                    objectImage = null;
                    switch (j)
                    {
                        case 0:
                            // Slime
                            if (objectsOnMap[i].ObjectType == ObjectTypes.GridObject &&
                                ((GridObject)objectsOnMap[i]).GridObjectType == GridObjectTypes.Slime)
                            {
                                objectImage = PictureManager.SlimeImage;
                            }
                            else
                                continue;
                            break;
                        case 1:
                            // GridObjects
                            if (objectsOnMap[i].ObjectType == ObjectTypes.GridObject)
                            {
                                GridObject gridObject = (GridObject)objectsOnMap[i];
                                if (gridObject.GridObjectType == GridObjectTypes.Slime)
                                    continue;

                                objectImage = PictureManager.GetGridObjectImage(gridObject);
                            }
                            else
                                continue;
                            break;
                        case 2:
                            // Units
                            if (objectsOnMap[i].ObjectType == ObjectTypes.Unit)
                            {
                                Unit unit = (Unit)objectsOnMap[i];

                                // Check Smoke Cloud
                                // Draw objects with 'Smoke Cloud' effect if Slug also has it and vice versa
                                if (!(Player.HasEffectType(EffectTypes.SmokeCloud) ^ unit.HasEffectType(EffectTypes.SmokeCloud)))
                                    objectImage = PictureManager.GetUnitImage(unit);
                            }
                            else
                                continue;
                            break;
                        case 3:
                            // Slug
                            if (objectsOnMap[i].ObjectType == ObjectTypes.Slug)
                                objectImage = PictureManager.GetSlugImage((Slug)objectsOnMap[i]);
                            else
                                continue;
                            break;
                    }

                    if (objectImage == null)
                        continue;

                    // Determine object image coords
                    // relative to the object position on Cell and Player (as a center of the GridMap)
                    int xCoord = this.pbGridMap.Size.Width / 2 - ((Player.Position.Location.X - objectsOnMap[i].Position.Location.X) *
                            GlobalConstants.CELL_WIDTH + Player.Position.X - objectsOnMap[i].Position.X) - objectImage.Size.Width / 2;
                    int yCoord = this.pbGridMap.Size.Height / 2 - ((Player.Position.Location.Y - objectsOnMap[i].Position.Location.Y) *
                            GlobalConstants.CELL_HEIGHT + Player.Position.Y - objectsOnMap[i].Position.Y) - objectImage.Size.Height / 2;

                    gCellBP.DrawImage(objectImage, xCoord, yCoord,
                        objectImage.Size.Width, objectImage.Size.Height);
                }
        }

    }
}
