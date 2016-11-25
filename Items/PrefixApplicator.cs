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
            item.rare = 9;
            item.accessory = true;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        Item matchingAccessory = null;
        public override bool CanRightClick()
        {
            return matchingAccessory != null;
        }
        public override void UpdateInventory(Player player)
        {
            // Search through the inventory for where I am, then
            // look for favourited items from that point, looping 
            // back to where my index is
            int myIndex = -1;
            Item invItem;
            
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
                    invItem.favorited)
                {
                    matchingAccessory = invItem;
                    item.toolTip2 = "Apply to: " + invItem.name;
                    return true;
                }
            }
            return false;
        }

        public override void RightClick(Player player)
        {
            if (matchingAccessory != null)
            {
                int type = matchingAccessory.type;
                int stack = matchingAccessory.stack;
                int prefix = item.prefix;
                matchingAccessory.SetDefaults(0);
                item.SetDefaults(0);
                Expeditions.ClientNetSpawnItem(type, stack, prefix);
            }
            else
            {
                item.stack++; //cancel consuming this
            }
        }
    }
}
