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
            e.Graphics.DrawString("Total scores: " + oPlayer.GetScore(), new Font("Arial", 12), new SolidBrush(Color.White), 10, 70);
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
        private void RebuildGraphMap()
        {
            this.SuspendLayout();
            GPS PBLocation = new GPS();
            GridMap Block = new GridMap();

            List<Maze.Classes.Object> objectsOnMap = new List<Maze.Classes.Object>();
            
            // GridMapGraph
            for (int i = 0; i < GlobalConstants.GRIDMAP_WIDTH; ++i)
                for (int j = 0; j < GlobalConstants.GRIDMAP_HEIGHT; ++j)
                {
                    // Calculated location point for every block
                    int x, y;
                    x = /*GridMapPB.Location.X*/ +(i - 1) * GlobalConstants.GRIDMAP_BLOCK_WIDTH - (oPlayer.Position.X - 25);
                    y = /*GridMapPB.Location.Y*/ +(j - 1) * GlobalConstants.GRIDMAP_BLOCK_HEIGHT - (oPlayer.Position.Y - 25);
                    PBLocation.X = oPlayer.Position.Location.X + i - GlobalConstants.GRIDMAP_WIDTH / 2;
                    PBLocation.Y = oPlayer.Position.Location.Y + j - GlobalConstants.GRIDMAP_HEIGHT / 2;
                    PBLocation.Z = oPlayer.Position.Location.Z;
                    PBLocation.Level = oPlayer.Position.Location.Level;
                    Block = GetWorldMap().GetGridMap(PBLocation);

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

                    /*
                    // Draw Coin if not collected
                    if (HasBit(Block.Attribute, (byte) Attributes.HasCoin) &&
                        !GetWorldMap().IsCoinCollected(Block))
                    {
                        Graphics g = Graphics.FromImage(PictureMgr.CoinImage);
                        g = this.GridMapPB.CreateGraphics();
                        g.DrawImage(PictureMgr.CoinImage, x + 15, y + 10, 20, 30);
                        g.Dispose();
                    }
                    */

                    // Draw Visible Units
                    objectsOnMap.AddRange(GetObjectContainer().GetAllObjectsByGPS(Block.Location));
                }

            for (int i = 0; i < objectsOnMap.Count; ++i)
            {
                if (objectsOnMap[i].GetType() == ObjectType.Player)
                    continue;
                
                Image objectImage;
                objectImage = PictureMgr.SoulImage; // Default

                if (objectsOnMap[i].GetType() == ObjectType.Unit)
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
                        return;

                    switch (gridObject.GetObjectType())
                    {
                        case GridObjectType.Coin: objectImage = PictureMgr.CoinImage; break;
                        default: objectImage = PictureMgr.CoinImage; break;
                    }
                }

                int xCoord = GridMapPB.Size.Width / 2 - ((oPlayer.Position.Location.X - objectsOnMap[i].Position.Location.X) *
                        GlobalConstants.GRIDMAP_BLOCK_WIDTH + oPlayer.Position.X - objectsOnMap[i].Position.X) - objectImage.Size.Width / 2;
                int yCoord = GridMapPB.Size.Height / 2 - ((oPlayer.Position.Location.Y - objectsOnMap[i].Position.Location.Y) *
                        GlobalConstants.GRIDMAP_BLOCK_HEIGHT + oPlayer.Position.Y - objectsOnMap[i].Position.Y) - objectImage.Size.Height / 2;


                Graphics g = Graphics.FromImage(objectImage);
                g = this.GridMapPB.CreateGraphics();
                g.DrawImage(objectImage, xCoord, yCoord,
                    objectImage.Size.Width, objectImage.Size.Height);
                g.Dispose();
            }

            // Draw Player
            Image PlayerImage;
            if (oPlayer.IsAlive())
                PlayerImage = PictureMgr.PlayerImage;
            else
                PlayerImage = PictureMgr.SoulImage;

            Graphics gPlayer = Graphics.FromImage(PlayerImage);
            gPlayer = this.GridMapPB.CreateGraphics();
            gPlayer.DrawImage(PlayerImage,
                (this.GridMapPB.Size.Width - PlayerImage.Width) / 2,
                (this.GridMapPB.Size.Height - PlayerImage.Height) / 2,
                PlayerImage.Width, PlayerImage.Height);
            gPlayer.Dispose();

            this.ResumeLayout();
        }

    }
}
