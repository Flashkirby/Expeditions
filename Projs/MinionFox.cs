using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Expeditions.Projs
{
    class MinionFox : FamiliarMinion
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.PirateCaptain);
            Main.projFrames[projectile.type] = 12;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }
    }
}
