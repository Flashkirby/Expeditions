using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.NPCs
{
    class ClerkHiding : ModNPC
    {
        public override void SetDefaults()
        {
            npc.name = "Sleeping Clerk";
            npc.width = 32;
            npc.height = 22;
            npc.friendly = true;
            npc.dontTakeDamage = true; //hide the health bar

            npc.aiStyle = -1;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            npc.rarity = 1;
        }

        public override float CanSpawn(NPCSpawnInfo spawnInfo)
        {
            // Skip if 'rescued' clerk already (no more natural spawn)
            if (WorldExplore.savedClerk) return 0f;

            try
            {
                int third = Main.maxTilesX / 3;
                if (Expeditions.DEBUG && !WorldExplore.savedClerk)
                {
                    Main.NewTextMultiline(
                        (spawnInfo.spawnTileX > third && spawnInfo.spawnTileX < Main.maxTilesX - third) + ": In middle third\n" +
                        (spawnInfo.player.ZoneOverworldHeight) + ": In the overworld\n" +
                        ((int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass) + ": spawn tile is grass? : " + (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type + "|" + (int)TileID.Grass + "\n" +
                        ((int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe) + ": spawn tile is not dirt? : " + (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall + "|" + (int)WallID.DirtUnsafe + " or 196-199 \n" +
                        ((int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY - 1].liquid == 0) + ": spawn tile is not submerged? : " + (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY - 1].liquid
                        );
                }
                if (
                    // Within centre third of world
                    spawnInfo.spawnTileX > third && spawnInfo.spawnTileX < Main.maxTilesX - third &&
                    // in the overworld
                    spawnInfo.player.ZoneOverworldHeight &&
                    // Not near bad biomes
                    !spawnInfo.player.ZoneCorrupt &&
                    !spawnInfo.player.ZoneCrimson &&
                    // Can only spawn on grass with no natural dirt background or liquid (so in open air)
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass &&
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe &&
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe1 &&
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe2 &&
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe3 &&
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe4 &&
                    (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY - 1].liquid == 0 &&
                    // Not 'saved' yet
                    !WorldExplore.savedClerk &&
                    // None of me exists
                    !NPC.AnyNPCs(npc.type) &&
                    !NPC.AnyNPCs(Expeditions.npcClerk)
                    )
                {
                    if (Expeditions.DEBUG) Main.NewText("Spawned succesfully!", 50, 255, 100);
                    return 1f; //guaranteed to spawn on next call (because we want to be found)
                }
            }
            catch { } //I hate array errors
            return 0f;
        }

        bool onSpawn = true;
        public override void AI()
        {
            //face away from player on spawn
            if (onSpawn)
            {
                onSpawn = false;
                npc.TargetClosest();
                npc.direction = npc.direction * -1;
                npc.spriteDirection = npc.direction;

                npc.townNPC = true; //not a townNPC by default but this means you can talk to them
            }

            //always invincible (to enemy npcs)
            npc.immune[255] = 30;

            //transform if someone be chatting me up
            foreach (Player p in Main.player)
            {
                if(p.active && p.talkNPC == npc.whoAmI)
                {
                    //appear out of the grass
                    for(int i = 0; i < 40; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height,
                            DustID.GrassBlades, (i - 20) * 0.1f, -1.5f);
                    }
                    Main.PlaySound(6, npc.Center);

                    npc.dontTakeDamage = false;
                    npc.Transform(mod.NPCType("Clerk"));
                }
            }

            // Floor friction
            npc.velocity.X = npc.velocity.X * 0.93f;
            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
            {
                npc.velocity.X = 0f;
            }
        }

        public override string GetChat()
        {
            switch(Main.rand.Next(3))
            {
                case 1:
                    return "Waah!? I wasn't sleeping on the job, honest. ";
                case 2:
                    return "Oh! Don't mind me, I was just taking a power nap. ";
                default:
                    return "Y-yes sir? Wait a minute, you're not my boss. ";
            }
        }
    }
}
