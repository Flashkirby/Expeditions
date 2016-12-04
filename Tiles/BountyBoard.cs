using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

using Expeditions;

namespace Expeditions.Tiles
{
    class BountyBoard : ModTile
    {
        public static int itemType;

        public const int tileWidth = 4;
        public const int tileHeight = 3;
        public override void SetDefaults()
        {
            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            AddMapEntry(new Color(200, 180, 50), "Expeditions Board");
            dustType = 7;
            disableSmartCursor = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = tileWidth;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            //offset into ground
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            itemType = mod.ItemType("BountyBoard");
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            adjTiles = new int[] { TileID.Chairs };
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 64, 48, mod.ItemType("BountyBoard"));
        }
        public override void RightClick(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            PlayerExplorer playerm = player.GetModPlayer<PlayerExplorer>(mod);
            Tile tile = Main.tile[i, j];

            //Can't do it if something is in front
            if (Main.mouseText)
            {
                if(ExpeditionUI.viewMode == ExpeditionUI.viewMode_Tile) Expeditions.CloseExpeditionMenu(true);
                return;
            }

            // Set custom open tile to top left
            playerm.tileOpened[0] = i - tile.frameX / 18;
            playerm.tileOpened[1] = j - tile.frameY / 18;
            // Alt direction offset
            if (tile.frameY > 54) playerm.tileOpened[1] -= tileHeight;

            Main.mouseRightRelease = false;
            
            if (player.sign >= 0) //close sign editing
            {
                player.sign = -1;
                Main.editSign = false;
                Expeditions.CloseExpeditionMenu();
                return;
            }
            if (Main.npcChatText != "") //gets hidden when an NPC is in front
            {
                Expeditions.CloseExpeditionMenu();
                return;
            }

            Expeditions.ToggleExpeditionMenu(ExpeditionUI.viewMode_Tile);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            Tile tile = Main.tile[i, j];
            
            player.showItemIconText = "";
            player.showItemIcon2 = itemType;
            player.noThrow = 2;
            player.showItemIcon = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            player.showItemIcon = false;
            player.showItemIcon2 = 0;
        }
    }
}