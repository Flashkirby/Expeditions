using System;
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
            expedition.difficulty = 9;
            expedition.defeat = true;
            expedition.important = true;

            expedition.conditionDescription1 = "Fight the Rusted Company";
            expedition.conditionDescription2 = "Fight the Marching Bones";
            expedition.conditionDescription3 = "Fight the Molten Legion";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 20);
        }
        public override string Description(bool complete)
        {
            return "You must've heard those spooky screams, right? Do skeletons even scream? That's for you to go down and find out! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if(player.ZoneDungeon && !condition1 && !condition2 && !condition3)
            {
                NPC npc = Expeditions.LastHitNPC;
                if (npc.type >= 269 && npc.type <= 272) condition1 = true; // Rusty Bones
                if (npc.type >= 273 && npc.type <= 276) condition2 = true; // Blue Bones
                if (npc.type >= 277 && npc.type <= 280) condition3 = true; // Hell Bones)
            }
            return condition1 && condition2 && condition3;
        }
    }
}
