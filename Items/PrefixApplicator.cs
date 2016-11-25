using Terraria;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class PrefixApplicator : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Prefix Applicator";
            item.toolTip = "Right click to attach prefix to accessory";
            item.width = 32;
            item.height = 24;
            item.rare = 9;
            item.accessory = true;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override bool CanRightClick() { return true; }
        public override void RightClick(Player player)
        {
            foreach(Item pItem in player.inventory)
            {
                if(pItem.accessory && pItem.type != item.type)
                {
                    Expeditions.ClientNetSpawnItem(pItem.type, pItem.stack, item.prefix);
                    item.SetDefaults(0);
                    pItem.SetDefaults(0);
                    break;
                }
            }
        }
    }
}
