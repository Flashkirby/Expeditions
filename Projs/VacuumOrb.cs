using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Projs
{
    class VacuumOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WaterBolt);
            projectile.aiStyle = 0;
            projectile.alpha = 0;
            projectile.penetrate = 256;

            projectile.name = "Vacuum Orb";
            projectile.alpha = 100;
            projectile.timeLeft = 600;
        }

        public const int explosionTimeLeft = 3;
        public const float explosionDelay = 20f;
        public override void AI()
        {
            int d;
            if (projectile.ai[1] == 0f)
            {
                d = Dust.NewDust(projectile.Center - new Vector2(4, 4), 4, 4, 173,
                    projectile.velocity.X * 10f, projectile.velocity.Y * 10f, 0, default(Color), 2f);
                Main.dust[d].velocity *= 0.08f;
                Main.dust[d].noGravity = true;

                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 71,
                    projectile.velocity.X, projectile.velocity.Y);
                Main.dust[d].velocity *= 0.25f;
            }
            else
            {
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 71,
                    0, 0, 50, default(Color), 0.5f);
                Main.dust[d].velocity *= 0.1f;
                if (projectile.scale > 0) projectile.scale *= 0.9f;
            }

            if (projectile.penetrate != projectile.maxPenetrate && projectile.penetrate > 0)
            {
                projectile.ai[1] = explosionDelay + explosionTimeLeft;
                projectile.localAI[0] = projectile.damage;
                projectile.timeLeft = (int)explosionDelay + explosionTimeLeft;

                projectile.penetrate = -1;
                projectile.damage = 0;
                projectile.tileCollide = false;
                projectile.velocity = projectile.velocity * 0.25f;

                Main.PlaySound(2, projectile.Center, 24);
            }
            
            if(projectile.timeLeft <= explosionTimeLeft)
            {
                if(projectile.timeLeft == explosionTimeLeft) Main.PlaySound(2, projectile.Center, 27);
                projectile.damage = (int)projectile.localAI[0];

                projectile.hide = true;

                projectile.Center = projectile.BottomRight;
                projectile.width = 64;
                projectile.height = 64;
                projectile.Center = projectile.TopLeft;

                for(int i = 0; i < 5; i++)
                {
                    d = Dust.NewDust(projectile.Center - new Vector2(4, 4), 0, 0, 72 + Main.rand.Next(2),
                        projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;

                    d = Dust.NewDust(projectile.Center - new Vector2(4, 4), 0, 0, 173,
                        projectile.velocity.X * -1f, projectile.velocity.Y * -1f, 0, default(Color), 2f);
                    Main.dust[d].velocity *= 3f;
                }
            }

            projectile.rotation += projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate = 1;
            projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D t = Main.projectileTexture[projectile.type];
            Vector2 p = projectile.position - Main.screenPosition;
            Vector2 c = new Vector2(Main.projectileTexture[projectile.type].Width / 2, Main.projectileTexture[projectile.type].Height / 2);
            spriteBatch.Draw(t,
                p + c,
                null, new Color(255, 255, 255, projectile.alpha),
                projectile.rotation,
                c,
                projectile.scale,
                SpriteEffects.None,
                0f);
            return false;
        }
    }
}
