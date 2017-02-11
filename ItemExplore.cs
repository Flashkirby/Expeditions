using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions
{
    public class ItemExplore : GlobalItem
    {
        public override void OnCraft(Item item, Recipe recipe)
        {
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                me.OnCraftItem(item, recipe, Main.LocalPlayer,
                              ref me.expedition.condition1Met,
                              ref me.expedition.condition2Met,
                              ref me.expedition.condition3Met,
                              me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                              );
            }
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (player.ItemSpace(item))
            {
                foreach (ModExpedition me in Expeditions.GetExpeditionsList())
                {
                    me.OnPickupItem(item, Main.LocalPlayer,
                              ref me.expedition.condition1Met,
                              ref me.expedition.condition2Met,
                              ref me.expedition.condition3Met,
                              me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                              );
                }
            }
            return base.OnPickup(item, player);
        }
    }
}
