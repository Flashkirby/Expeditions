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
            AddRewardMoney(Item.buyPrice(0, 1, 0, 0));
            AddRewardItem(mod.ItemType("StockBox"), 1);

            // Hellstone bars to help get equipment faster in case you did this on accident
            AddRewardItem(ItemID.HellstoneBar, 5);
            
            // Helpful for not dying in boss fights
            AddRewardItem(ItemID.HealingPotion, 5);
            AddRewardItem(ItemID.ManaPotion, 5);
            AddRewardItem(ItemID.GravitationPotion, 1);

            // For building a runway
            AddRewardItem(ItemID.AshBlock, 500);

            // Always need chests
            AddRewardItem(ItemID.ShadowChest);
        }
        public override string Description(bool complete)
        {
            string npcName = "the Guide";
            if (NPC.FindFirstNPC(NPCID.Guide) != -1) npcName = NPC.firstNPCName(NPCID.Guide);
            if (complete)
            {
                if (Main.hardMode)
                {
                    return "So sacrificing " + npcName + " summons a great big horrifying flesh wall? Well uh... that's new. But defeating it releases spirits? I've never heard of such a thing! ";
                }
                else
                {
                    return "So sacrificing " + npcName + " summons a great big horrifying flesh wall? Well uh... that's new. If you want to defeat that thing you'll probably need to be as well-equipped as possible. ";
                }
            }
            return "So I've been trying to decipher these dungeon books, but so far not much luck. However there are a couple that seem to illustrate some kind of underworld ritual using... voodoo dolls... of " + npcName + "? This could be something big! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (!cond1) cond1 = player.ZoneUnderworldHeight;
            if (cond1 && player.ZoneUnderworldHeight && !cond2)
            {
                cond2 = Main.hardMode;
                if(!cond2) //check if we've seen the WoF
                {
                    cond2 = player.FindBuffIndex(BuffID.Horrified) >= 0;
                }
            }
            return cond1 && cond2;
        }
    }
}
