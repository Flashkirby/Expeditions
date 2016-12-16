using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Projs
{
    class Gust : ModProjectile
    {
        public const int extraSize = 16;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WaterBolt);
            projectile.aiStyle = 0;

            projectile.name = "Gust";
            projectile.width = 28 + extraSize;
            projectile.height = 28 + extraSize;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.velocity *= 0.98f;

            if ((int)projectile.ai[0] % 3 == 0)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height,
                    16, projectile.velocity.X, projectile.velocity.Y, 200, default(Color), 0.7f);
                Main.dust[d].fadeIn = 1.5f;
                Main.dust[d].velocity *= 0.1f;

                int alpha = (int)MathHelper.Clamp(255 - projectile.timeLeft, 0, 256);
                int g = Gore.NewGore(new Vector2(
                    projectile.position.X + Main.rand.Next(extraSize + 1), 
                    projectile.position.Y + Main.rand.Next(extraSize + 1)), 
                    default(Vector2), Main.rand.Next(11, 14), 1f);
                Main.gore[g].velocity += projectile.velocity * 2;
                Main.gore[g].velocity *= 0.2f;
                Main.gore[g].alpha = alpha;
            }

            projectile.ai[0]++;

            if (projectile.velocity.Length() < 0.1f || projectile.wet)
            {
                projectile.Kill();
            }
        }

        public override void TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 8;
            height = 8;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X == 0) projectile.velocity.X = projectile.oldVelocity.X * -0.2f;
            if (projectile.velocity.Y == 0) projectile.velocity.Y = projectile.oldVelocity.Y * -0.2f;
            return false;
        }
    }
}
