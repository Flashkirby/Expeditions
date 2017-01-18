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
                me.OnCraftItem(item, recipe);
            }
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (player.ItemSpace(item))
            {
                foreach (ModExpedition me in Expeditions.GetExpeditionsList())
                {
                    me.OnPickupItem(item);
                }
            }
            return base.OnPickup(item, player);
        }
        public override bool GrabStyle(Item item, Player player)
        {
            return base.GrabStyle(item, player);
        }
    }
}
