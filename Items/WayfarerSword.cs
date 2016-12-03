using Terraria;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class WayfarerSword : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Wayfarer's Sword";
            item.width = 32;
            item.height = 36;
            item.scale = 0.95f;
            item.useSound = 1;

            item.useStyle = 1;
            item.melee = true;
            item.damage = 16;
            item.useAnimation = 21;
            item.knockBack = 6f;
            
            item.value = Item.sellPrice(0, 0, 25, 0);
        }
    }
}
