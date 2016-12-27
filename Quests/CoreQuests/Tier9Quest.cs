﻿using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier9Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Back to the Dungeons";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 9;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Fight the Rusted Company";
            expedition.conditionDescription2 = "Fight the Marching Bones";
            expedition.conditionDescription3 = "Fight the Molten Legion";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 20);
            AddRewardItem(mod.ItemType("StockBox2"), 1);

            // Lunar event prep
            AddRewardItem(ItemID.LifeforcePotion, 3);
            AddRewardItem(ItemID.GravitationPotion, 3);
            AddRewardItem(ItemID.WrathPotion, 3);
            AddRewardItem(ItemID.RagePotion, 3);

            // Always need chests
            AddRewardItem(ItemID.PinkDungeonChest);
            AddRewardItem(ItemID.GreenDungeonChest);
            AddRewardItem(ItemID.BlueDungeonChest);
        }
        public override string Description(bool complete)
        {
            if (complete) return "Woah, sounds like a bone-a-fide party of skeletons down there then? Eh? Not funny? Oh well. But in all seriousness it seems like different skeletons only show up in specific parts of the dungeon. ";
            return "You must've heard those spooky screams, right? Do skeletons even scream? That's for you to go down and find out! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if(player.ZoneDungeon && !cond1 || !cond2 || !cond3)
            {
                NPC npc = Expeditions.LastHitNPC;
                if (npc.type >= 269 && npc.type <= 272) cond1 = true; // Rusty Bones
                if (npc.type >= 273 && npc.type <= 276) cond2 = true; // Blue Bones
                if (npc.type >= 277 && npc.type <= 280) cond3 = true; // Hell Bones)
            }
            return cond1 && cond2 && cond3;
        }
    }
}
