using Maze.Classes;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Maze.Forms
{
    partial class Play
    {
        void RightPanelPB_Paint(object sender, PaintEventArgs e)
        {
            if (!PlayStarted)
                return;

            e.Graphics.DrawString("Time: " + (ProgramTime.Seconds + ProgramTime.Minutes * 60).ToString(), new Font("Arial", 14), new SolidBrush(Color.White), 10, 30);
            e.Graphics.DrawString("Coins x " + GetWorldMap().CoinsRemain().ToString(),
                new Font("Arial", 14), new SolidBrush(Color.White), 10, 50);
            e.Graphics.DrawString("Total scores: " + player.GetScore(), new Font("Arial", 12), new SolidBrush(Color.White), 10, 70);
        }

        void GridMapPB_Paint(object sender, PaintEventArgs e)
        {
            // Only when game is started
            if (PlayStarted)
                RebuildGraphMap(e.Graphics);
        }

        private void Play_Paint(object sender, PaintEventArgs e)
        {
            //RebuildGraphMap();
            ++tempCount;
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

        /// <summary>
        /// RePaint PlayForm map pictures.
        /// Include images of the player, blocks and objects on a block
        /// </summary>
        private void RebuildGraphMap(Graphics gGridMapBP)
        {
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();

            List<Maze.Classes.Object> objectsOnMap = new List<Maze.Classes.Object>();

            // GridMapGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    // Calculated location point for every block
                    int x, y;
                    x = /*GridMapPB.Location.X*/ +(i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (player.Position.X - 25);
                    y = /*GridMapPB.Location.Y*/ +(j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (player.Position.Y - 25);
                    PBLocation.X = player.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = player.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = player.Position.Location.Z;
                    PBLocation.Level = player.Position.Location.Level;
                    Block = GetWorldMap().GetGridMap(PBLocation);

                    this.GridMapGraphic[i, j].Block = Block;
                    gGridMapBP.DrawImage(PictureMgr.GetPictureByType(Block.Type), x, y, GlobalConstants.GRIDMAP_BLOCK_WIDTH, GlobalConstants.GRIDMAP_BLOCK_HEIGHT);


                    // Draw Start Block
                    if (Block.HasAttribute(GridMapAttributes.IsStart))
                    {
                        gGridMapBP.DrawImage(PictureMgr.StartImage, x + 5, y + 5, 40, 40);
                    }

                    // Draw Finish Block
                    if (Block.HasAttribute(GridMapAttributes.IsFinish))
                    {
                        gGridMapBP.DrawImage(PictureMgr.FinishImage, x + 5, y + 5, 40, 40);
                    }

                    // Include all objects in this grid
                    objectsOnMap.AddRange(GetObjectContainer().GetAllObjectsByGPS(Block.Location));

                }

            // Draw Visible Objects
            for (int i = 0; i < objectsOnMap.Count; ++i)
            {
                Image objectImage;
                objectImage = PictureMgr.SoulImage; // Default

                if (objectsOnMap[i].GetType() == ObjectType.Slug)
                {
                    if (player.IsAlive())
                        objectImage = PictureMgr.SlugImage;
                    else
                        objectImage = PictureMgr.SoulImage;
                }
                else if (objectsOnMap[i].GetType() == ObjectType.Unit)
                {
                    Unit unit = (Unit)objectsOnMap[i];
                    switch (unit.GetUnitType())
                    {
                        case UnitTypes.Deimos: objectImage = PictureMgr.DeimosImage; break;
                        case UnitTypes.Phobos: objectImage = PictureMgr.PhobosImage; break;
                        default: objectImage = PictureMgr.DeimosImage; break;
                    }
                }
                else if (objectsOnMap[i].GetType() == ObjectType.GridObject)
                {
                    GridObject gridObject = (GridObject)objectsOnMap[i];
                    if (!gridObject.IsActive())
                        continue;

                    switch (gridObject.GetGridObjectType())
                    {
                        case GridObjectType.Coin: objectImage = PictureMgr.CoinImage; break;
                        case GridObjectType.Portal: objectImage = PictureMgr.PortalImage; break;
                        default: objectImage = PictureMgr.CoinImage; break;
                    }
                }

                int xCoord = GridMapPB.Size.Width / 2 - ((player.Position.Location.X - objectsOnMap[i].Position.Location.X) *
                        GlobalConstants.GRIDMAP_BLOCK_WIDTH + player.Position.X - objectsOnMap[i].Position.X) - objectImage.Size.Width / 2;
                int yCoord = GridMapPB.Size.Height / 2 - ((player.Position.Location.Y - objectsOnMap[i].Position.Location.Y) *
                        GlobalConstants.GRIDMAP_BLOCK_HEIGHT + player.Position.Y - objectsOnMap[i].Position.Y) - objectImage.Size.Height / 2;

                gGridMapBP.DrawImage(objectImage, xCoord, yCoord,
                    objectImage.Size.Width, objectImage.Size.Height);
            }
        }

    }
}
