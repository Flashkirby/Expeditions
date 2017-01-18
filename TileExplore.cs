using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions
{
    public class TileExplore : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                if (i == Player.tileTargetX && j == Player.tileTargetY)
                {
                    me.OnKillTile(i, j, type);
                }
            }
        }

        public override void RightClick(int i, int j, int type)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.tileInteractionHappened)
            {
                if (player.position.X / 16f - (float)Player.tileRangeX <= (float)i &&
                    (player.position.X + (float)player.width) / 16f + (float)Player.tileRangeX - 1f >= (float)i &&
                    player.position.Y / 16f - (float)Player.tileRangeY <= (float)j &&
                    (player.position.Y + (float)player.height) / 16f + (float)Player.tileRangeY - 2f >= (float)j)
                {
                    // Feasibly in tile range when activated since we can't tell directly
                    foreach (ModExpedition me in Expeditions.GetExpeditionsList())
                    {
                        if (i == Player.tileTargetX && j == Player.tileTargetY)
                        {
                            me.OnInteractTile(i, j, type);
                        }
                    }
                }
            }
        }
    }
}
