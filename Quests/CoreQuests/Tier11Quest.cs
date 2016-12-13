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
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 11;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Defeat an eldritch demon";
            expedition.conditionDescriptionCountable = "Destroy the pillars";
            expedition.conditionCountedMax = 4;

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 50);
        }
        public override string Description(bool complete)
        {
            return "Ok, serious time. The whole kerfuffle with cultists and pillars, looks to be leading to the summoning of a huge, eldritch demon. Plus, it doesn't look like there's a way to remove the pillars without triggering this event, so I guess you'll have to face it soon. Prepare yourself, looks like it might get messy. ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return 
                Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1
                && NPC.downedAncientCultist;
        }

        public override void CheckConditionCountable(Player player, ref ushort count, ushort max)
        {
            count = 0;
            if (NPC.downedTowerSolar) count++;
            if (NPC.downedTowerVortex) count++;
            if (NPC.downedTowerNebula) count++;
            if (NPC.downedTowerStardust) count++;
        }
        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (!cond1) cond1 = NPC.downedMoonlord;
            return cond1;
        }
    }
}
