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
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 4;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Enter the Underworld";
            expedition.conditionDescription2 = "Investigate the ritual";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 1);
            AddRewardItem(ItemID.HellstoneBar, 10);
            AddRewardItem(ItemID.HealingPotion, 20);
            AddRewardItem(ItemID.ManaPotion, 20);
            AddRewardItem(ItemID.IronskinPotion, 5);
            AddRewardItem(ItemID.GravitationPotion, 1);
        }
        public override string Description(bool complete)
        {
            if (complete)
            {
                string npcName = "the Guide";
                if (NPC.FindFirstNPC(NPCID.Guide) != -1) npcName = NPC.firstNPCName(NPCID.Guide);

                if (Main.hardMode)
                {
                    return "So sacrificing " + npcName + " summons a great big horrifying flesh wall? Well uh... that's new. But defeating it releases spirits? I've never heard of such a thing! ";
                }
                else
                {
                    return "So sacrificing " + npcName + " summons a great big horrifying flesh wall? Well uh... that's new. If you want to defeat that thing you'll probably need to be as well-equipped as possible. ";
                }
            }
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
