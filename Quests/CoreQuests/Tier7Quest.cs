using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier7Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Thrashing Wilds";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 7;
            expedition.ctgCollect = true;
            expedition.ctgImportant = true;

        }
        public override void AddItemsOnLoad()
        {
            expedition.AddDeliverable(ItemID.ChlorophyteOre, 3);

            AddRewardItem(ItemID.GoldCoin, 10);
        }
        public override string Description(bool complete)
        {
            return "I don't know how, but defeating those mechanical bosses seems to have triggered a sudden aggressiveness from the jungle. Don't suppose you could... take a peek? You'll be fine! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
    }
}
