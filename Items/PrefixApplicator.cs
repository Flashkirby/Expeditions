using Terraria;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class PrefixApplicator : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Prefix Applicator";
            item.toolTip = "Right click to use on next favourited accessory";
            item.width = 32;
            item.height = 24;
            item.consumable = true;
            item.rare = 0; // Shouldn't be at this value anyway because prefixes
            item.accessory = true;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        Item matchingAccessory = null;
        public override bool CanRightClick()
        {
            return matchingAccessory != null && item.prefix != 0;
        }
        public override void UpdateInventory(Player player)
        {
            if (item.prefix == 0)
            {
                item.toolTip2 = "Apply to: No prefix to apply";
                return;
            }

            // Search through the inventory for where I am, then
            // look for favourited items from that point, looping 
            // back to where my index is
            int myIndex = -1;
            
            // iterate first time to find me and items past me
            for(int i = 0; i < player.inventory.Length; i++)
            {
                if (LookForMeAndMatch(ref myIndex, player.inventory[i], i))
                {
                    return;
                }
            }
            //iterate a second time and end at me
            for (int i = 0; i < myIndex; i++)
            {
                if (LookForMeAndMatch(ref myIndex, player.inventory[i], i))
                {
                    return;
                }
            }

            matchingAccessory = null;
            item.toolTip2 = "Apply to: No favourited accessory";
        }

        private bool LookForMeAndMatch(ref int myIndex, Item invItem, int i)
        {
            if (myIndex == -1)
            {
                if (invItem.IsTheSameAs(this.item)) { myIndex = i; }
            }
            else
            {
                if (invItem.accessory &&
                    invItem.type != item.type &&
                    invItem.prefix != item.prefix &&
                    invItem.favorited)
                {
                    matchingAccessory = invItem;
                    item.toolTip2 = "Apply to: " + invItem.name;
                    return true;
                }
            }
            return false;
        }

        bool consume;
        public override void RightClick(Player player)
        {
            if (matchingAccessory != null && item.prefix != 0)
            {
                // Apply the new prefix
                matchingAccessory.Prefix(item.prefix);

                // Make it obvious it's changed
                matchingAccessory.favorited = false;
                matchingAccessory.newAndShiny = true;

                consume = true;
            }
            else
            {
                consume = false;
            }
        }
        public override bool ConsumeItem(Player player)
        {
            return consume;
        }
    }
}
