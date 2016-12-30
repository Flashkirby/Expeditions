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
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 3;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Gain access to the Dungeon";
            expedition.conditionDescription2 = "Enter the Dungeon";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardMoney(Item.buyPrice(0, 1, 0, 0));
            AddRewardItem(mod.ItemType("StockBox"), 1);

            // Stay alaive for a bit longer
            AddRewardItem(ItemID.HealingPotion, 5);
            AddRewardItem(ItemID.ManaPotion, 5);

            // Blow up spikes!
            AddRewardItem(ItemID.Bomb, 10);

            // Always need chests
            AddRewardItem(ItemID.GoldChest);
        }
        public override string Description(bool complete)
        {
            if (complete) return "Well big scary skeletons aside, that went pretty smoothly, don'tcha think? ";
            if (expedition.condition2Met && !expedition.condition1Met) return "Ouch, that sounds... painful. Looks like you'll have to help the old man with his curse. ";
            return "Sooner or later we'll need access to the dungeon so we can map it out. That's where you come in! Maybe try asking that old guy nicely? ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (NPC.downedBoss3 && !cond1) cond1 = NPC.downedBoss3;
            if (!cond2) cond2 = player.ZoneDungeon;
            return cond1 && cond2;
        }
    }
}
