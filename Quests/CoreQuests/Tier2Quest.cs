using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier2Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Tier 2 Quest";
            expedition.difficulty = 2;
            expedition.deliver = true;
            expedition.important = true;
        }
        public override void AddItemsOnLoad()
        {
            if (!WorldGen.crimson)
            {
                expedition.name = "A Demon's Metal";
                expedition.AddDeliverable(ItemID.DemoniteOre, 10);
                AddRewardItem(ItemID.GoldCoin, 1);
                AddRewardPrefix(mod.ItemType("PrefixApplicator"), 79); //Intrepid
                AddRewardPrefix(mod.ItemType("PrefixApplicator"), 66); //Arcane
                AddRewardPrefix(WorldGen.copperBar, 10);
                AddRewardPrefix(WorldGen.ironBar, 10);
            }
            else
            {
                expedition.name = "Streaks of Crimson";
                expedition.AddDeliverable(ItemID.CrimtaneOre, 10);
                AddRewardItem(ItemID.GoldCoin, 1);
                AddRewardPrefix(mod.ItemType("PrefixApplicator"), 64); //Armored
                AddRewardPrefix(mod.ItemType("PrefixApplicator"), 75); //Hasty
                AddRewardPrefix(WorldGen.copperBar, 10);
                AddRewardPrefix(WorldGen.ironBar, 10);
            }
        }
        public override string Description(bool complete)
        {
            return "Hey, do you think the " + (WorldGen.crimson ? "crimson" : "corruption") + " hides some ore that could be made into workable metal? I mean it's a possibility considering how it transforms things. Please investigate it! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
    }
}
