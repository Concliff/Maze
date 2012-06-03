using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace Maze.Classes
{
    public class PictureManager
    {
        public struct EffectImage
        {
            public Image Map;
            public Image Aura;
        };

        public Image StartImage;
        public Image FinishImage;
        public Image DeimosImage;
        public Image PhobosImage;
        public Image CoinImage;
        public Image SlugImage;
        public Image SoulImage;
        public Image SlimeImage;
        public Image PortalImage;

        private int CellsCount;
        private Image[] Pictures;           // Blocks Images
        public EffectImage[] EffectImages;
        private string ImageDirectoryPath = GlobalConstants.IMAGES_PATH;

        public PictureManager()
        {
            CellsCount = 0;
            LoadImages();
        }
        
        private void LoadImages()
        {
            StreamReader CellsStream = File.OpenText(ImageDirectoryPath + "Cells.dat");
            CellsCount = Convert.ToInt32(CellsStream.ReadLine());
            CellsStream.Close();

            Pictures = new Image[CellsCount];
            for (int i = 0; i < CellsCount; ++i)
                Pictures[i] = Image.FromFile(ImageDirectoryPath + "Cell" + i.ToString() + ".bmp");

            FinishImage = Image.FromFile(ImageDirectoryPath + "Brain.png");
            StartImage = Image.FromFile(ImageDirectoryPath + "Start.bmp");
            CoinImage = Image.FromFile(ImageDirectoryPath + "Coin.bmp");
            DeimosImage = Image.FromFile(ImageDirectoryPath + "Deimos.png");
            PhobosImage = Image.FromFile(ImageDirectoryPath + "Phobos.png");
            SlimeImage = Image.FromFile(ImageDirectoryPath + "Slime.png");
            SlugImage = Image.FromFile(ImageDirectoryPath + "Slug.png");
            SoulImage = Image.FromFile(ImageDirectoryPath + "Soul.png");
            PortalImage = Image.FromFile(ImageDirectoryPath + "Portal.png");

            // Load effects images
            EffectImages = new EffectImage[DBStores.EffectStore.Count + 1];
            EffectImages[0].Map = Image.FromFile(ImageDirectoryPath + "Effects\\Hidden.png");
            for (int i = 1; i <= DBStores.EffectStore.Count; ++i)
            {
                string mapFileName = ImageDirectoryPath + "Effects\\Map" + i.ToString() + ".png";
                if (File.Exists(mapFileName))
                    EffectImages[i].Map = Image.FromFile(ImageDirectoryPath + "Effects\\Map" + i.ToString() + ".png");
            }


        }

        public Image GetPictureByType(uint Type)
        {
            if (Type >= CellsCount)
                return null;
            return Pictures[Type];
        }
        
    }
}
