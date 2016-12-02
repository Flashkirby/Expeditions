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
            npc.name = "Suspicious Grass Patch";
            npc.width = 32;
            npc.height = 22;
            npc.friendly = true;
            npc.dontTakeDamage = true;

            npc.aiStyle = 0;
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
                spawnInfo.spawnTileX > third && spawnInfo.spawnTileX < Main.maxTilesX - third &&
                // in the overworld
                spawnInfo.player.ZoneOverworldHeight &&
                // Not in special biomes mear spawm
                !spawnInfo.player.ZoneSnow &&
                !spawnInfo.player.ZoneJungle &&
                !spawnInfo.player.ZoneCorrupt &&
                !spawnInfo.player.ZoneCrimson
                )
                return 1f;
            return 0f;
        }

        public override string GetChat()
        {
            npc.Transform(mod.NPCType("Clerk"));
            return "Oh, this is a test. Sorry about that. ";
        }
    }
}
