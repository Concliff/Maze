using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace Maze.Classes
{
    /// <summary>
    /// Contains all Images objects that are used in <see cref="Play"/> form rendering.
    /// </summary>
    public static class PictureManager
    {
        public struct EffectImage
        {
            public Image Map;
            public Image Aura;
        };

        public static Image StartImage;
        public static Image FinishImage;
        public static Image DeimosImage;
        public static Image PhobosImage;
        public static Image DropImage;
        public static Image SlugImage;
        public static Image SoulImage;
        public static Image SlimeImage;
        public static Image PortalImage;
        public static Image SmokeCloud;
        public static EffectImage[] EffectImages;

        private static int mapImageCount;
        private static Image[] mapImage;           // Blocks Images
        private static Image SlugInvisible;
        private static string ImageDirectoryPath = GlobalConstants.IMAGES_PATH;

        public static void Load()
        {
            StreamReader CellsStream = File.OpenText(ImageDirectoryPath + "Cells.dat");
            mapImageCount = Convert.ToInt32(CellsStream.ReadLine());
            CellsStream.Close();

            mapImage = new Image[mapImageCount];
            for (int i = 0; i < mapImageCount; ++i)
                mapImage[i] = Image.FromFile(ImageDirectoryPath + "Cell" + i.ToString() + ".bmp");

            FinishImage = Image.FromFile(ImageDirectoryPath + "Brain.png");
            StartImage = Image.FromFile(ImageDirectoryPath + "Start.bmp");
            DropImage = Image.FromFile(ImageDirectoryPath + "Drop.png");
            DeimosImage = Image.FromFile(ImageDirectoryPath + "Deimos.png");
            PhobosImage = Image.FromFile(ImageDirectoryPath + "Phobos.png");
            SlimeImage = Image.FromFile(ImageDirectoryPath + "Slime.png");
            SlugImage = Image.FromFile(ImageDirectoryPath + "Slug.png");
            SlugInvisible = Image.FromFile(ImageDirectoryPath + "Slug_Invisible.png");
            SoulImage = Image.FromFile(ImageDirectoryPath + "Soul.png");
            PortalImage = Image.FromFile(ImageDirectoryPath + "Portal.png");
            SmokeCloud = Image.FromFile(ImageDirectoryPath + "SmokeCloud.png");

            // Load effects images
            EffectImages = new EffectImage[DBStores.EffectStore.Count + 1];
            EffectImages[0].Map = Image.FromFile(ImageDirectoryPath + "Effects\\Hidden.png");
            for (int i = 1; i <= DBStores.EffectStore.Count; ++i)
            {
                // Map(Bonus) Images
                string mapFileName = ImageDirectoryPath + "Effects\\Map" + i.ToString() + ".png";
                if (File.Exists(mapFileName))
                    EffectImages[i].Map = Image.FromFile(mapFileName);

                // Aura(PlayForm) Images
                string auraFileName = ImageDirectoryPath + "Effects\\Aura" + i.ToString() + ".png";
                if (File.Exists(auraFileName))
                    EffectImages[i].Aura = Image.FromFile(auraFileName);
            }
        }

        public static Image GetPictureByType(uint Type)
        {
            if (Type >= mapImageCount)
                return null;
            return mapImage[Type];
        }

        public static Image GetUnitImage(Unit unit)
        {
            // Separate method for Slug
            if (unit.ObjectType == ObjectTypes.Slug)
                return GetSlugImage((Slug)unit);

            // Do not draw Invisible and Dead units
            if (!unit.IsVisible || !unit.IsAlive)
                return null;

            switch (unit.UnitType)
            {
                case UnitTypes.Deimos:
                    return DeimosImage;
                case UnitTypes.Phobos:
                    return PhobosImage;
                case UnitTypes.SlugClone:
                    return SlugImage;
            }

            // Else nothing to draw
            return null;
        }

        public static Image GetSlugImage(Slug slug)
        {
            if (!slug.IsAlive)
                return SoulImage;
            if (!slug.IsVisible)
                return SlugInvisible;

            return SlugImage;
        }

        public static Image GetGridObjectImage(GridObject gridObject)
        {
            // Only Active objects are visible
            if (!gridObject.IsActive)
                return null;

            switch (gridObject.GridObjectType)
            {
                case GridObjectTypes.SmokeCloud:
                    return PictureManager.SmokeCloud;
                case GridObjectTypes.OozeDrop:
                    return PictureManager.DropImage;
                case GridObjectTypes.Portal:
                    return PictureManager.PortalImage;
                case GridObjectTypes.Bonus:
                    return PictureManager.EffectImages[((Bonus)gridObject).GetEffect()].Map;
            }

            // Else nothing to draw
            return null;
        }

        public static Image GetObjectImage(Object obj)
        {
            switch (obj.ObjectType)
            {
                case ObjectTypes.Slug:
                    return GetSlugImage((Slug)obj);
                case ObjectTypes.Unit:
                    return GetUnitImage((Unit)obj);
                case ObjectTypes.GridObject:
                    return GetGridObjectImage((GridObject)obj);
                default:
                    return null;
            }
        }
    }
}
