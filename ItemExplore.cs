using System;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions144
{
    public class ItemExplore : GlobalItem
    {
        public override void OnCreated(Item item, ItemCreationContext context)
        {
			if (context is RecipeItemCreationContext craftContext)
			{
				foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
				{
					me.OnCraftItem(item, craftContext.Recipe, Main.LocalPlayer,
								  ref me.expedition.condition1Met,
								  ref me.expedition.condition2Met,
								  ref me.expedition.condition3Met,
								  me.expedition.conditionCounted >= me.expedition.conditionCountedMax
								  );
				}
			}
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (player.ItemSpace(item).CanTakeItem)
            {
                foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
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
