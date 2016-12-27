using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class WayfarerSword : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.GoldBroadsword);
            item.name = "Wayfarer's Sword";
            item.width = 32;
            item.height = 36;
            item.scale = 0.95f;
            
            item.damage = 16;
            item.useAnimation = 21;
            item.knockBack = 6f;
            
            item.value = Item.sellPrice(0, 0, 25, 0);
        }
    }
}
