using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier4Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Underworld Mystery";
            expedition.difficulty = 4;
            expedition.defeat = true;
            expedition.important = true;

            expedition.conditionDescription1 = "Enter the Underworld";
            expedition.conditionDescription2 = "Investigate voodoo doll ritual";

            AddRewardItem(ItemID.GoldCoin, 4);
            AddRewardItem(ItemID.HealingPotion, 20);
            AddRewardItem(ItemID.ManaPotion, 20);
            AddRewardItem(ItemID.LifeforcePotion, 5);
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 65); //Warding
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 72); //Menacing
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 76); //Quick
        }
        public override string Description(bool complete)
        {
            return "So I've been trying to decipher these dungeon books, but so far not much luck. However there are a couple that seem to illustrate some kind of underworld ritual using... voodoo dolls? Mind taking a look - it could be something big! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (!condition1) condition1 = player.ZoneUnderworldHeight;
            if (condition1 && player.ZoneUnderworldHeight && !condition2)
            {
                condition2 = Main.hardMode;
                if(!condition2) //check if we've seen the WoF
                {
                    condition2 = player.HasBuff(BuffID.Horrified) >= 0;
                }
            }
            return condition1 && condition2;
        }
    }
}
