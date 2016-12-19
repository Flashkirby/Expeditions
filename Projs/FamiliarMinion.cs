using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions.Projs
{
    abstract class FamiliarMinion : ModProjectile
    {
        public override void AI()
        {
            CheckActive();

            ManageFrames(ref projectile.frame, ref projectile.frameCounter);
        }

        private void CheckActive()
        {
            Player player = Main.player[projectile.owner];
            PlayerExplorer modPlayer = player.GetModPlayer<PlayerExplorer>(mod);
            if (player.dead)
            {
                modPlayer.familiarMinion = false;
            }
            if (modPlayer.familiarMinion)
            {
                projectile.timeLeft = 2;
            }
        }

        private void ManageFrames(ref int frame, ref int frameCounter)
        {
            
        }
    }
}


/*
Vector2 vector53 = player3.Center;
if (flag26)
{
vector53.X -= (float)((15 + player3.width / 2) * player3.direction);
vector53.X -= (float)(this.minionPos * 40 * player3.direction);
}
if (this.type == 500)
{
vector53.X -= (float)((15 + player3.width / 2) * player3.direction);
vector53.X -= (float)(40 * player3.direction);
}
if (this.type == 653)
{
float num653 = (float)(15 + (player3.crimsonHeart ? 40 : 0));
vector53.X -= (num653 + (float)(player3.width / 2)) * (float)player3.direction;
vector53.X -= (float)(40 * player3.direction);
}
if (this.type == 500)
{
Lighting.AddLight(base.Center, 0.9f, 0.1f, 0.3f);
int num654 = 6;
if (this.frame == 0 || this.frame == 2)
{
num654 = 12;
}
int num3 = this.frameCounter + 1;
this.frameCounter = num3;
if (num3 >= num654)
{
this.frameCounter = 0;
num3 = this.frame + 1;
this.frame = num3;
if (num3 >= Main.projFrames[this.type])
{
    this.frame = 0;
}
}
this.rotation += this.velocity.X / 20f;
Vector2 vector54 = (-Vector2.UnitY).RotatedBy((double)this.rotation, default(Vector2)).RotatedBy((double)((float)this.direction * 0.2f), default(Vector2));
int num655 = Dust.NewDust(base.Center + vector54 * 10f - new Vector2(4f), 0, 0, 5, vector54.X, vector54.Y, 0, Color.Transparent, 1f);
Main.dust[num655].scale = 1f;
Main.dust[num655].velocity = vector54.RotatedByRandom(0.78539818525314331) * 3.5f;
Main.dust[num655].noGravity = true;
Main.dust[num655].shader = GameShaders.Armor.GetSecondaryShader(Main.player[this.owner].cLight, Main.player[this.owner]);
}
if (this.type == 653)
{
this.rotation += this.velocity.X / 20f;
if (this.velocity.Y == 0f)
{
this.rotation = this.rotation.AngleTowards(0f, 0.7f);
}
if (this.owner >= 0 && this.owner < 255)
{
Projectile._CompanionCubeScreamCooldown[this.owner] -= 1f;
if (Projectile._CompanionCubeScreamCooldown[this.owner] < 0f)
{
    Projectile._CompanionCubeScreamCooldown[this.owner] = 0f;
}
}
Tile tileSafely = Framing.GetTileSafely(base.Center);
if (tileSafely.liquid > 0 && tileSafely.lava())
{
this.localAI[0] += 1f;
}
else
{
this.localAI[0] -= 1f;
}
this.localAI[0] = MathHelper.Clamp(this.localAI[0], 0f, 20f);
if (this.localAI[0] >= 20f)
{
if (Projectile._CompanionCubeScreamCooldown[this.owner] == 0f)
{
    Projectile._CompanionCubeScreamCooldown[this.owner] = 3600f;
    Main.PlaySound((Main.rand.Next(10) == 0) ? SoundID.NPCDeath61 : SoundID.NPCDeath59, this.position);
}
this.Kill();
}
Vector3 vector55 = Lighting.GetColor((int)base.Center.X / 16, (int)base.Center.Y / 16).ToVector3();
Vector3 vector56 = Lighting.GetColor((int)player3.Center.X / 16, (int)player3.Center.Y / 16).ToVector3();
if (vector55.Length() < 0.15f && vector56.Length() < 0.15f)
{
this.localAI[1] += 1f;
}
else if (this.localAI[1] > 0f)
{
this.localAI[1] -= 1f;
}
this.localAI[1] = MathHelper.Clamp(this.localAI[1], -3600f, 120f);
if (this.localAI[1] > (float)Main.rand.Next(30, 120) && !player3.immune && player3.velocity == Vector2.Zero)
{
if (Main.rand.Next(5) == 0)
{
    Main.PlaySound(SoundID.Item16, base.Center);
    this.localAI[1] = -600f;
}
else
{
    Main.PlaySound(SoundID.Item1, base.Center);
    player3.Hurt(PlayerDeathReason.ByOther(6), 3, 0, false, false, false, -1);
    player3.immune = false;
    player3.immuneTime = 0;
    this.localAI[1] = (float)(-300 + Main.rand.Next(30) * -10);
}
}
}
bool flag27 = true;
if (this.type == 500 || this.type == 653)
{
flag27 = false;
}
int num656 = -1;
float num657 = 450f;
if (flag26)
{
num657 = 800f;
}
int num658 = 15;
if (this.ai[0] == 0f & flag27)
{
NPC ownerMinionAttackTargetNPC4 = this.OwnerMinionAttackTargetNPC;
if (ownerMinionAttackTargetNPC4 != null && ownerMinionAttackTargetNPC4.CanBeChasedBy(this, false))
{
float num659 = (ownerMinionAttackTargetNPC4.Center - base.Center).Length();
if (num659 < num657)
{
    num656 = ownerMinionAttackTargetNPC4.whoAmI;
    num657 = num659;
}
}
if (num656 < 0)
{
int num3;
for (int num660 = 0; num660 < 200; num660 = num3 + 1)
{
    NPC nPC3 = Main.npc[num660];
    if (nPC3.CanBeChasedBy(this, false))
    {
        float num661 = (nPC3.Center - base.Center).Length();
        if (num661 < num657)
        {
            num656 = num660;
            num657 = num661;
        }
    }
    num3 = num660;
}
}
}
if (this.ai[0] == 1f)
{
this.tileCollide = false;
float num662 = 0.2f;
float num663 = 10f;
int num664 = 200;
if (num663 < Math.Abs(player3.velocity.X) + Math.Abs(player3.velocity.Y))
{
num663 = Math.Abs(player3.velocity.X) + Math.Abs(player3.velocity.Y);
}
Vector2 vector57 = player3.Center - base.Center;
float num665 = vector57.Length();
if (num665 > 2000f)
{
this.position = player3.Center - new Vector2((float)this.width, (float)this.height) / 2f;
}
if (num665 < (float)num664 && player3.velocity.Y == 0f && this.position.Y + (float)this.height <= player3.position.Y + (float)player3.height && !Collision.SolidCollision(this.position, this.width, this.height))
{
this.ai[0] = 0f;
this.netUpdate = true;
if (this.velocity.Y < -6f)
{
    this.velocity.Y = -6f;
}
}
if (num665 >= 60f)
{
vector57.Normalize();
vector57 *= num663;
if (this.velocity.X < vector57.X)
{
    this.velocity.X = this.velocity.X + num662;
    if (this.velocity.X < 0f)
    {
        this.velocity.X = this.velocity.X + num662 * 1.5f;
    }
}
if (this.velocity.X > vector57.X)
{
    this.velocity.X = this.velocity.X - num662;
    if (this.velocity.X > 0f)
    {
        this.velocity.X = this.velocity.X - num662 * 1.5f;
    }
}
if (this.velocity.Y < vector57.Y)
{
    this.velocity.Y = this.velocity.Y + num662;
    if (this.velocity.Y < 0f)
    {
        this.velocity.Y = this.velocity.Y + num662 * 1.5f;
    }
}
if (this.velocity.Y > vector57.Y)
{
    this.velocity.Y = this.velocity.Y - num662;
    if (this.velocity.Y > 0f)
    {
        this.velocity.Y = this.velocity.Y - num662 * 1.5f;
    }
}
}
if (this.velocity.X != 0f)
{
this.spriteDirection = Math.Sign(this.velocity.X);
}
if (flag26)
{
int num3 = this.frameCounter;
this.frameCounter = num3 + 1;
if (this.frameCounter > 3)
{
    num3 = this.frame;
    this.frame = num3 + 1;
    this.frameCounter = 0;
}
if (this.frame < 10 | this.frame > 13)
{
    this.frame = 10;
}
this.rotation = this.velocity.X * 0.1f;
}
}
if (this.ai[0] == 2f)
{
this.friendly = true;
this.spriteDirection = this.direction;
this.rotation = 0f;
this.frame = 4 + (int)((float)num658 - this.ai[1]) / (num658 / 3);
if (this.velocity.Y != 0f)
{
this.frame += 3;
}
this.velocity.Y = this.velocity.Y + 0.4f;
if (this.velocity.Y > 10f)
{
this.velocity.Y = 10f;
}
this.ai[1] -= 1f;
if (this.ai[1] <= 0f)
{
this.ai[1] = 0f;
this.ai[0] = 0f;
this.friendly = false;
this.netUpdate = true;
return;
}
}
if (num656 >= 0)
{
float num666 = 400f;
float num667 = 20f;
if (flag26)
{
num666 = 700f;
}
if ((double)this.position.Y > Main.worldSurface * 16.0)
{
num666 *= 0.7f;
}
NPC nPC4 = Main.npc[num656];
Vector2 center3 = nPC4.Center;
float num668 = (center3 - base.Center).Length();
Collision.CanHit(this.position, this.width, this.height, nPC4.position, nPC4.width, nPC4.height);
if (num668 < num666)
{
vector53 = center3;
if (center3.Y < base.Center.Y - 30f && this.velocity.Y == 0f)
{
    float num669 = Math.Abs(center3.Y - base.Center.Y);
    if (num669 < 120f)
    {
        this.velocity.Y = -10f;
    }
    else if (num669 < 210f)
    {
        this.velocity.Y = -13f;
    }
    else if (num669 < 270f)
    {
        this.velocity.Y = -15f;
    }
    else if (num669 < 310f)
    {
        this.velocity.Y = -17f;
    }
    else if (num669 < 380f)
    {
        this.velocity.Y = -18f;
    }
}
}
if (num668 < num667)
{
this.ai[0] = 2f;
this.ai[1] = (float)num658;
this.netUpdate = true;
}
}
if (this.ai[0] == 0f && num656 < 0)
{
float num670 = 500f;
if (this.type == 500)
{
num670 = 200f;
}
if (this.type == 653)
{
num670 = 170f;
}
if (Main.player[this.owner].rocketDelay2 > 0)
{
this.ai[0] = 1f;
this.netUpdate = true;
}
Vector2 vector58 = player3.Center - base.Center;
if (vector58.Length() > 2000f)
{
this.position = player3.Center - new Vector2((float)this.width, (float)this.height) / 2f;
}
else if (vector58.Length() > num670 || Math.Abs(vector58.Y) > 300f)
{
this.ai[0] = 1f;
this.netUpdate = true;
if (this.velocity.Y > 0f && vector58.Y < 0f)
{
    this.velocity.Y = 0f;
}
if (this.velocity.Y < 0f && vector58.Y > 0f)
{
    this.velocity.Y = 0f;
}
}
}
if (this.ai[0] == 0f)
{
this.tileCollide = true;
float num671 = 0.5f;
float num672 = 4f;
float num673 = 4f;
float num674 = 0.1f;
if (num673 < Math.Abs(player3.velocity.X) + Math.Abs(player3.velocity.Y))
{
num673 = Math.Abs(player3.velocity.X) + Math.Abs(player3.velocity.Y);
num671 = 0.7f;
}
int num675 = 0;
bool flag28 = false;
float num676 = vector53.X - base.Center.X;
if (Math.Abs(num676) > 5f)
{
if (num676 < 0f)
{
    num675 = -1;
    if (this.velocity.X > -num672)
    {
        this.velocity.X = this.velocity.X - num671;
    }
    else
    {
        this.velocity.X = this.velocity.X - num674;
    }
}
else
{
    num675 = 1;
    if (this.velocity.X < num672)
    {
        this.velocity.X = this.velocity.X + num671;
    }
    else
    {
        this.velocity.X = this.velocity.X + num674;
    }
}
if (!flag26)
{
    flag28 = true;
}
}
else
{
this.velocity.X = this.velocity.X * 0.9f;
if (Math.Abs(this.velocity.X) < num671 * 2f)
{
    this.velocity.X = 0f;
}
}
if (num675 != 0)
{
int num677 = (int)(this.position.X + (float)(this.width / 2)) / 16;
int num678 = (int)this.position.Y / 16;
num677 += num675;
num677 += (int)this.velocity.X;
int num3;
for (int num679 = num678; num679 < num678 + this.height / 16 + 1; num679 = num3 + 1)
{
    if (WorldGen.SolidTile(num677, num679))
    {
        flag28 = true;
    }
    num3 = num679;
}
}
if (this.type == 500 && this.velocity.X != 0f)
{
flag28 = true;
}
if (this.type == 653 && this.velocity.X != 0f)
{
flag28 = true;
}
Collision.StepUp(ref this.position, ref this.velocity, this.width, this.height, ref this.stepSpeed, ref this.gfxOffY, 1, false, 0);
if (this.velocity.Y == 0f & flag28)
{
int num3;
for (int num680 = 0; num680 < 3; num680 = num3 + 1)
{
    int num681 = (int)(this.position.X + (float)(this.width / 2)) / 16;
    if (num680 == 0)
    {
        num681 = (int)this.position.X / 16;
    }
    if (num680 == 2)
    {
        num681 = (int)(this.position.X + (float)this.width) / 16;
    }
    int num682 = (int)(this.position.Y + (float)this.height) / 16;
    if (WorldGen.SolidTile(num681, num682) || Main.tile[num681, num682].halfBrick() || Main.tile[num681, num682].slope() > 0 || (TileID.Sets.Platforms[(int)Main.tile[num681, num682].type] && Main.tile[num681, num682].active() && !Main.tile[num681, num682].inActive()))
    {
        try
        {
            num681 = (int)(this.position.X + (float)(this.width / 2)) / 16;
            num682 = (int)(this.position.Y + (float)(this.height / 2)) / 16;
            num681 += num675;
            num681 += (int)this.velocity.X;
            if (!WorldGen.SolidTile(num681, num682 - 1) && !WorldGen.SolidTile(num681, num682 - 2))
            {
                this.velocity.Y = -5.1f;
            }
            else if (!WorldGen.SolidTile(num681, num682 - 2))
            {
                this.velocity.Y = -7.1f;
            }
            else if (WorldGen.SolidTile(num681, num682 - 5))
            {
                this.velocity.Y = -11.1f;
            }
            else if (WorldGen.SolidTile(num681, num682 - 4))
            {
                this.velocity.Y = -10.1f;
            }
            else
            {
                this.velocity.Y = -9.1f;
            }
            goto IL_1DF80;
        }
        catch
        {
            this.velocity.Y = -9.1f;
            goto IL_1DF80;
        }
        continue;
    }
    IL_1DF80:
    num3 = num680;
}
}
if (this.velocity.X > num673)
{
this.velocity.X = num673;
}
if (this.velocity.X < -num673)
{
this.velocity.X = -num673;
}
if (this.velocity.X < 0f)
{
this.direction = -1;
}
if (this.velocity.X > 0f)
{
this.direction = 1;
}
if (this.velocity.X > num671 && num675 == 1)
{
this.direction = 1;
}
if (this.velocity.X < -num671 && num675 == -1)
{
this.direction = -1;
}
this.spriteDirection = this.direction;
if (flag26)
{
this.rotation = 0f;
if (this.velocity.Y == 0f)
{
    if (this.velocity.X == 0f)
    {
        this.frame = 0;
        this.frameCounter = 0;
    }
    else if (Math.Abs(this.velocity.X) >= 0.5f)
    {
        this.frameCounter += (int)Math.Abs(this.velocity.X);
        int num3 = this.frameCounter;
        this.frameCounter = num3 + 1;
        if (this.frameCounter > 10)
        {
            num3 = this.frame;
            this.frame = num3 + 1;
            this.frameCounter = 0;
        }
        if (this.frame >= 4)
        {
            this.frame = 0;
        }
    }
    else
    {
        this.frame = 0;
        this.frameCounter = 0;
    }
}
else if (this.velocity.Y != 0f)
{
    this.frameCounter = 0;
    this.frame = 14;
}
}
this.velocity.Y = this.velocity.Y + 0.4f;
if (this.velocity.Y > 10f)
{
this.velocity.Y = 10f;
}
}
if (flag26)
{
this.localAI[0] += 1f;
if (this.velocity.X == 0f)
{
this.localAI[0] += 1f;
}
if (this.localAI[0] >= (float)Main.rand.Next(900, 1200))
{
this.localAI[0] = 0f;
int num3;
for (int num683 = 0; num683 < 6; num683 = num3 + 1)
{
    int num684 = Dust.NewDust(base.Center + Vector2.UnitX * -(float)this.direction * 8f - Vector2.One * 5f + Vector2.UnitY * 8f, 3, 6, 216, -(float)this.direction, 1f, 0, default(Color), 1f);
    Dust dust3 = Main.dust[num684];
    dust3.velocity /= 2f;
    Main.dust[num684].scale = 0.8f;
    num3 = num683;
}
int num685 = Gore.NewGore(base.Center + Vector2.UnitX * -(float)this.direction * 8f, Vector2.Zero, Main.rand.Next(580, 583), 1f);
Gore gore = Main.gore[num685];
gore.velocity /= 2f;
Main.gore[num685].velocity.Y = Math.Abs(Main.gore[num685].velocity.Y);
Main.gore[num685].velocity.X = -Math.Abs(Main.gore[num685].velocity.X) * (float)this.direction;
return;
}
}
 */
