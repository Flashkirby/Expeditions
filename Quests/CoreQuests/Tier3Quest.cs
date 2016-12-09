using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier3Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Dungeon Admission";
            expedition.difficulty = 3;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Gain access to the Dungeon";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 2);
            AddRewardItem(ItemID.GoldenKey, 1);
            AddRewardItem(ItemID.AnkletoftheWind, 1);
            AddRewardPrefix(ItemID.HealingPotion, 15);
            AddRewardPrefix(ItemID.ManaPotion, 15);
            AddRewardPrefix(ItemID.Bomb, 10);
        }
        public override string Description(bool complete)
        {
            return "Sooner or later we'll need access to the dungeon so we can map it out. That's where you come in! Maybe try asking that old guy nicely? ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (NPC.downedBoss3 && !condition1) condition1 = player.ZoneDungeon;
            return condition1;
        }
    }
}
