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
            expedition.AddDeliverable(ItemID.ChlorophyteOre, 1);
            expedition.AddDeliverable(ItemID.LifeFruit, 1);

            AddRewardItem(ItemID.GoldCoin, 10);
            AddRewardItem(ItemID.GoldenCrate, 1);

            // Find more life fruit and chlorophyte
            AddRewardItem(ItemID.MiningPotion, 20);
            AddRewardItem(ItemID.SpelunkerPotion, 20);

            // Mushrooms for ranged stuff, enough for a small biome
            AddRewardItem(ItemID.DarkBlueSolution, 20);

        }
        public override string Description(bool complete)
        {
            if (complete) return "Some kind of... super tough, organic ore? I wonder if it can grow like other plants? Oh and those life fruits - fascinating! Please, keep a lookout and see if there's anything else of note. I'd greatly appreciate it. ";
            return "I don't know how, but defeating that mechanical boss seems to have triggered a certain... change from the jungle. Don't suppose you could take a peek? You'll be fine! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return 
                Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1
                && (NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3);
        }
    }
}
