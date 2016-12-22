using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Projs
{
    class WayfarerPike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Spear);
            projectile.name = "Pike";
            projectile.width = 14;
            projectile.height = 14;
            projectile.scale = 1.1f;
        }

        public const float extend = 0.9f;
        public const float retract = 1.1f;
        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = 4f;
                projectile.netUpdate = true;
            }
            if (Main.player[projectile.owner].itemAnimation < Main.player[projectile.owner].itemAnimationMax / 3)
            {
                projectile.ai[0] -= retract;
            }
            else
            {
                projectile.ai[0] += extend;
            }
        }

    }
}
