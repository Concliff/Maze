﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace Maze.Classes
{
    class PictureManager
    {
        public Image StartImage;
        public Image FinishImage;
        public Image DeimosImage;
        public Image PhobosImage;
        public Image CoinImage;

        private int CellsCount;
        private Image[] Pictures;           // Blocks Images
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

            FinishImage = Image.FromFile(ImageDirectoryPath + "Finish.bmp");
            StartImage = Image.FromFile(ImageDirectoryPath + "Start.bmp");
            CoinImage = Image.FromFile(ImageDirectoryPath + "Coin.bmp");
            DeimosImage = Image.FromFile(ImageDirectoryPath + "Deimos.png");
            PhobosImage = Image.FromFile(ImageDirectoryPath + "Phobos.png");
        }

        public Image GetPictureByType(int Type)
        {
            if (Type >= CellsCount)
                return null;
            return Pictures[Type];
        }
        
    }
}