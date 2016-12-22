using Terraria;
using Terraria.ID;

namespace Expeditions.Projs.Familiars
{
    class MinionChicken : FamiliarMinion
    {
        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.name = "Familiar Fowl";
            projectile.width = 30;
            projectile.height = 24;

            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.netImportant = true;

            AIPrioritiseNearPlayer = false;
            AIPrioritiseFarEnemies = false;

            // Animation Frames
            attackFrame = 8;
            attackFrameCount = 3;
            runFrame = 1;
            runFrameCount = 7;
            flyFrame = 11;
            flyFrameSpeed = 3;
            flyRotationMod = 0.3f;
            fallFrame = 11;

            Main.projFrames[projectile.type] = 15;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;

            drawOriginOffsetY = (Main.projectileTexture[projectile.type].Width - projectile.width) / 2;
            drawOffsetX = (Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]) - projectile.height - 4;
        }
    }
}
