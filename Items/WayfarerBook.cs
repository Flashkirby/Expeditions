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
            item.name = "Wayfarer's Wind";
            item.toolTip = "Casts a mighty gust of wind";
            item.UseSound = SoundID.Item34;

            item.mana = 20;
            item.damage = 8;
            item.useAnimation = 40;
            item.useTime = 40;
            item.knockBack = 7f;
            item.shoot = mod.ProjectileType("Gust");
            item.shootSpeed = 7f;

            item.value = Item.sellPrice(0, 8, 0, 0);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(
                speedX + 2f * (Main.rand.NextFloat() - 0.5f),
                speedY + 2f * (Main.rand.NextFloat() - 0.5f)
                ), type, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, new Vector2(
                speedX + 4f * (Main.rand.NextFloat() - 0.5f),
                speedY + 4f * (Main.rand.NextFloat() - 0.5f)
                ), type, damage, knockBack, player.whoAmI);
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}
