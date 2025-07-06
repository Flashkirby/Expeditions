using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

using Expeditions144;
using Terraria.GameContent.ObjectInteractions;
using System.Collections.Generic;

namespace Expeditions144.Tiles
{
    public class BountyBoard : ModTile
    {
        public static int itemType;

        public const int tileWidth = 4;
        public const int tileHeight = 3;
        public override void SetStaticDefaults()
        {
            //extra info
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            AddMapEntry(new Color(200, 180, 50), ModContent.GetInstance<Items.BountyBoard>().DisplayName);
            DustType = 7;
			//****DisableSmartCursor = true;

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

            itemType = ModContent.ItemType<Items.BountyBoard>();
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AdjTiles = new int[] { TileID.Chairs };
			RegisterItemDrop(itemType, 0, 1);
        }

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => false;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //****Item.NewItem(i * 16, j * 16, 64, 48, ModContent.ItemType<BountyBoard>());   -- not needed?
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            PlayerExplorer playerm = player.GetModPlayer<PlayerExplorer>();
            Tile tile = Main.tile[i, j];

            //Can't do it if something is in front
            if (Main.mouseText)
            {
                if(ExpeditionUI.viewMode == ExpeditionUI.viewMode_Tile) Expeditions144.CloseExpeditionMenu(true);
                return true;
            }

            int directionTileFrameY = tile.TileFrameY / 18;
            // Alt direction offset
            if (tile.TileFrameY >= 54) directionTileFrameY -= tileHeight;
            // Set custom open tile to top left
            playerm.tileOpened[0] = i - tile.TileFrameX / 18;
            playerm.tileOpened[1] = j - directionTileFrameY;

            Main.mouseRightRelease = false;
            
            if (player.sign >= 0) //close sign editing
            {
                player.sign = -1;
                Main.editSign = false;
                Expeditions144.CloseExpeditionMenu();
                return true;
            }
            if (Main.npcChatText != "") //gets hidden when an NPC is in front
            {
                Expeditions144.CloseExpeditionMenu();
                return true;
            }

            player.tileInteractionHappened = true;
            Expeditions144.ToggleExpeditionMenu(ExpeditionUI.viewMode_Tile);
			return true;
        }

        public override void MouseOver(int i, int j)
        {
            // Unfixable issue - always uses mouseover of most recent player
            // current comprimise is to remove when in multiplayer altogether
            if (Main.netMode == NetmodeID.Server) return; // **** changed to "== NetmodeID.Server", was "> 0"

			Player player = Main.LocalPlayer;

			player.noThrow = 2;
			player.cursorItemIconID = itemType;
			player.cursorItemIconEnabled = true;
        }
	}
}