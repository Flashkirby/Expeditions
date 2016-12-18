using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    class WayfarerStaff : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.AquaScepter);
            item.name = "Wayfarer's Cane";
            item.toolTip = "Shoots an explosive bolt";
            item.width = 42;
            item.height = 14;
            item.UseSound = SoundID.Item72;

            item.mana = 12;
            item.damage = 20;
            item.useAnimation = 33;
            item.useTime = 33;
            item.knockBack = 3.5f;
            item.shoot = mod.ProjectileType("VacuumOrb");
            item.shootSpeed = 7f;

            item.value = Item.buyPrice(0, 50, 0, 0);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }
    }
}
