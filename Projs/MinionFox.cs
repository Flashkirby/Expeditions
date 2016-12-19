using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Projs
{
    class MinionFox : FamiliarMinion
    {
        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.name = "Familiar Fox";
            projectile.width = 24;
            projectile.height = 22;

            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
            projectile.aiStyle = 26;

            //Main.projectileTexture[projectile.type]
            Main.projFrames[projectile.type] = 12;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.CanDistortWater[projectile.type] = true;
            drawOriginOffsetY = (Main.projectileTexture[projectile.type].Width - projectile.width) / 2 ;
            drawOffsetX = (Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]) - projectile.height;
        }
        
    }
}
