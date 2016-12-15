using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    class WayfarerBook : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WaterBolt);
            item.name = "Wayfarer's Tome";
            item.toolTip = "Casts a gust of wind";

            item.mana = 7;
            item.damage = 8;
            item.useAnimation = 20;
            item.useTime = 20;
            item.knockBack = 8f;
            item.shoot = ProjectileID.MagnetSphereBall;
            item.shootSpeed = 11f;

            item.value = Item.sellPrice(0, 3, 0, 0);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }
    }
}
