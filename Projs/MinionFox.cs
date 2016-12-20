using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Projs
{
    public class MinionFox : FamiliarMinion
    {
        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.name = "Familiar Fox";
            projectile.width = 24;
            projectile.height = 22;

            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.netImportant = true;

            AIPrioritiseNearPlayer = false;
            AIPrioritiseFarEnemies = false;

            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;

            drawOriginOffsetY = (Main.projectileTexture[projectile.type].Width - projectile.width) / 2 ;
            drawOffsetX = (Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]) - projectile.height - 4;
        }
    }
}
