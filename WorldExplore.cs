using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    class WorldExplore : ModWorld
    {
        #region TileChecks
        protected static List<ushort> tileCheckList;
        public static List<ushort> TileCheckList
        {
            get
            {
                if (tileCheckList == null) { tileCheckList = new List<ushort>(); }
                return tileCheckList;
            }
        }
        protected static List<ushort> tilesChecked;
        public static List<ushort> TilesChecked
        {
            get
            {
                if (tilesChecked == null) { tilesChecked = new List<ushort>(); }
                return tilesChecked;
            }
        }

        public static int CountTilesInChecked(ushort tileID)
        {
            int count = 0;
            foreach (ushort t in TilesChecked)
            {
                if (t == tileID) count++;
            }
            return count;
        }
        #endregion
        
        public override void PostUpdate()
        {
            if (Main.netMode == 2) return;
            // Calculate the dimensions of the viewport
            Point tPos = Main.screenPosition.ToTileCoordinates();
            Rectangle viewPortTiles = new Rectangle(tPos.X, tPos.Y, Main.screenWidth / 16, Main.screenHeight / 16);

            TilesChecked.Clear();
            if (tileCheckList.Count > 0)
            {
                for (int y = viewPortTiles.Top; y < viewPortTiles.Bottom; y++)
                {
                    for (int x = viewPortTiles.Left; x < viewPortTiles.Right; x++)
                    {
                        try
                        {
                            // Add tilesChecked if the viewport contains these tiles
                            ushort type = Main.tile[x, y].type;
                            if (TileCheckList.Contains(type))
                            {
                                TilesChecked.Add(type);
                            }
                        }
                        catch { }
                    }
                }
            }
            TileCheckList.Clear();
            /*
            string tilesfound = "Found |";
            if (Main.time % 15 == 0)
            {
                foreach (ushort type in TilesChecked)
                {
                    tilesfound += type + "|";
                }
                Main.NewText(tilesfound, Color.Beige.R, Color.Beige.G, Color.Beige.B);
            }
            */
        }
    }
}
