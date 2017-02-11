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
                if (i == Player.tileTargetX && j == Player.tileTargetY && !fail)
                {
                    me.OnKillTile(i, j, type, Main.LocalPlayer,
                              ref me.expedition.condition1Met,
                              ref me.expedition.condition2Met,
                              ref me.expedition.condition3Met,
                              me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                              );
                }
            }
        }
    }
}
