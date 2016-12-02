using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.NPCs
{
    public class ClerkHiding : ModNPC
    {
        public override void SetDefaults()
        {
            npc.name = "Camoflauged Clerk";
            npc.width = 32;
            npc.height = 22;
            npc.friendly = true;
            npc.dontTakeDamage = true; //hide the health bar

            npc.aiStyle = -1;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.soundHit = 1;
            npc.soundKilled = 1;
            npc.knockBackResist = 0.5f;
            npc.rarity = 1;
        }

        public override float CanSpawn(NPCSpawnInfo spawnInfo)
        {
            // Skip if 'rescued' clerk already (no more natural spawn)
            if (WorldExplore.savedClerk) return 0f;

            int third = Main.maxTilesX / 3;
            if (
                // Within centre third of world
                //spawnInfo.spawnTileX > third && spawnInfo.spawnTileX < Main.maxTilesX - third &&
                // in the overworld
                spawnInfo.player.ZoneOverworldHeight &&
                // Not in special biomes mear spawm
                !spawnInfo.player.ZoneSnow &&
                !spawnInfo.player.ZoneJungle &&
                !spawnInfo.player.ZoneCorrupt &&
                !spawnInfo.player.ZoneCrimson &&
                // Not 'saved' yet
                !WorldExplore.savedClerk &&
                // None of me exists
                !NPC.AnyNPCs(npc.type)
                )
                return 1f;
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
            return "Oh, this is a test. Sorry about that. ";
        }
    }
}
