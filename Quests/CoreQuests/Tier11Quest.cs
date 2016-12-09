using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier11Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Terrarian Enemy No. 1";
            expedition.difficulty = 11;
            expedition.defeat = true;
            expedition.important = true;

            expedition.conditionDescription1 = "Defeat an eldritch demon";

            AddRewardItem(ItemID.GoldCoin, 50);
        }
        public override string Description(bool complete)
        {
            return "Ok, serious time. The whole kerfuffle with cultists and pillars, looks to be leading to the summoning of a huge, eldritch demon. Plus, it doesn't look like there's a way to remove the pillars without triggering this event, so I guess you'll have to face it soon. Prepare yourself, looks like it might get messy. ";
        }
        public override void AddItemsOnLoad()
        {

        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (!condition1) condition1 = NPC.downedMoonlord;
            return condition1;
        }
    }
}
