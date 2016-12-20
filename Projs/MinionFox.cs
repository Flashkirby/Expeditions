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
            projectile.ignoreWater = true;
            projectile.netImportant = true;

            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;

            drawOriginOffsetY = (Main.projectileTexture[projectile.type].Width - projectile.width) / 2 ;
            drawOffsetX = (Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]) - projectile.height - 4;
        }

        public override void PostAI()
        {
            PlayerExplorer modPlayer = Main.player[projectile.owner].GetModPlayer<PlayerExplorer>(mod);
            /*
            Main.NewText("<Proj> player bool: " + modPlayer.familiarMinion + " | timeLeft: " + projectile.timeLeft);
            Main.NewText("<Proj> I am: " + projectile.type + "/" + mod.ProjectileType("MinionFox") + " | owner=" + projectile.owner + ", or "+ Main.player[projectile.owner].name);
            Main.NewText("<Proj> Am I active?: " + projectile.active);
            */
            //Main.NewText("<Proj> Grounded is " + (projectile.velocity.Y == 0f));
            //Main.NewText("<Proj> AI State is " + projectile.ai[0]);
            //Main.NewText("<Proj> Frame" + projectile.frame);
            //Main.NewText("<Proj> Framecounter" + projectile.frameCounter);
        }
    }
}
