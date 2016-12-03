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
            if (Expeditions.DEBUG && !WorldExplore.savedClerk)
            {
                Main.NewTextMultiline(
                    (spawnInfo.spawnTileX > third && spawnInfo.spawnTileX < Main.maxTilesX - third) + ": In middle third\n" +
                    (spawnInfo.player.ZoneOverworldHeight) + ": In the overworld\n" +
                    ((int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass) + ": spawn tile is grass? : " + (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type + "|" + (int)TileID.Grass + "\n" +
                    ((int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe) + ": spawn tile is not dirt? : " + (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall + "|" + (int)WallID.DirtUnsafe + " or 196-199"
                    );
            }
            if (
                // Within centre third of world
                spawnInfo.spawnTileX > third && spawnInfo.spawnTileX < Main.maxTilesX - third &&
                // in the overworld
                spawnInfo.player.ZoneOverworldHeight &&
                // Not in special biomes mear spawm
                !spawnInfo.player.ZoneSnow &&
                !spawnInfo.player.ZoneJungle &&
                !spawnInfo.player.ZoneCorrupt &&
                !spawnInfo.player.ZoneCrimson &&
                // Can only spawn on grass with no natural dirt background (so in open air)
                (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass &&
                (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe &&
                (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe1 &&
                (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe2 &&
                (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe3 &&
                (int)Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].wall != WallID.DirtUnsafe4 &&
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
