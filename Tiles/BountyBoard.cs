using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Expeditions.Tiles
{
    public class BountyBoard : ModTile
    {
        public override void SetDefaults()
        {
            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            AddMapEntry(new Color(90, 190, 20), "Expeditions Board");
            dustType = 7;
            //disableSmartCursor = true;
            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        }
    }
}