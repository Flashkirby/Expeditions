using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier8Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Temple Raid";
            expedition.difficulty = 8;
            expedition.ctgCollect = true;
            expedition.ctgImportant = true;

        }
        public override void AddItemsOnLoad()
        {
            expedition.AddDeliverable(ItemID.SolarTablet, 3);

            AddRewardItem(ItemID.GoldCoin, 15);
            AddRewardItem(ItemID.LihzahrdPowerCell, 3);
        }
        public override string Description(bool complete)
        {
            return "I WANT YOU!... to investigate that jungle temple. But how can we get in I wonder. Knowing this place you'll probably need to defeat a bigger, scarier jungle boss, like with Skeletron for the dungeon.";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
    }
}
