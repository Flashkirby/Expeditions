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
            expedition.repeatable = true;

            expedition.conditionDescription1 = "Defeat the Moon Lord";
            expedition.conditionDescriptionCountable = "Destroy the pillars";
            expedition.conditionCountedMax = 4;

        }
        public override void AddItemsOnLoad()
        {
            expedition.conditionDescription1 = "Defeat an eldritch demon";

            AddRewardMoney(Item.buyPrice(0, 30, 0, 0));
            AddRewardItem(mod.ItemType("StockBox2"), 1);

            // More accessory fine-tuning
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 68); //Lucky

            // A few extra fragments
            AddRewardItem(ItemID.FragmentSolar, 10);
            AddRewardItem(ItemID.FragmentVortex, 10);
            AddRewardItem(ItemID.FragmentNebula, 10);
            AddRewardItem(ItemID.FragmentStardust, 10);

            // Always need chests
            AddRewardItem(ItemID.MartianChest, 1);
        }
        public override string Description(bool complete)
        {
            if (complete) return "Wow! You did it! This is most definitely one for the books! Phew... pat yourself on the back a job well done, but we're not done yet - far from it! There's always more to discover, take a look around; perhaps its time to visit new lands? There's more to the world than just " + Main.worldName + ", y'know. ";
            return "Ok, serious time. The whole kerfuffle with cultists and pillars, looks to be leading to the summoning of a huge, eldritch demon. Plus, it doesn't look like there's a way to remove the pillars without triggering this event, so I guess you'll have to face it soon. Prepare yourself, looks like it might get messy. ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return
                Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1
                && NPC.downedAncientCultist;
        }

        public override void CheckConditionCountable(Player player, ref int count, int max)
        {
            if (count < 4)
            {
                count = 0;
                if (NPC.downedTowerSolar) count++;
                if (NPC.downedTowerVortex) count++;
                if (NPC.downedTowerNebula) count++;
                if (NPC.downedTowerStardust) count++;
            }
            else
            {
                expedition.conditionDescription1 = "Defeat the Moon Lord";
            }
        }
        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (!cond1) cond1 = NPC.downedMoonlord;
            return cond1;
        }
    }
}
