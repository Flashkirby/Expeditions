using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    class WayfarerPike : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Spear);
            item.name = "Wayfarer's Pike";
            item.width = 32;
            item.height = 36;
            
            item.damage = 10;
            item.useAnimation = 28;
            item.useTime = 28;
            item.knockBack = 5.5f;
            item.shoot = mod.ProjectileType("WayfarerPike");
            item.shootSpeed = 3.6f;

            item.value = Item.sellPrice(0, 0, 25, 0);
        }
    }
}
