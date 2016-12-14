using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier1Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Cave Exploration kit";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 1;
            expedition.ctgExplore = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Enter the Underground";
            expedition.conditionDescription2 = "Reach the Cavern layer";
        }
        public override void AddItemsOnLoad()
        {
            AddRewardMoney(Item.buyPrice(0, 0, 25, 0));
            // Basic armour
            AddRewardItem(ItemID.CopperChainmail, 1);
            AddRewardItem(ItemID.CopperGreaves, 1);

            // Useful mining tools
            AddRewardItem(ItemID.IronPickaxe, 1);
            AddRewardItem(ItemID.Torch, 99);
            AddRewardItem(ItemID.RopeCoil, 10);
            AddRewardItem(ItemID.SpelunkerPotion, 10);
        }
        public override string Description(bool complete)
        {
            if (complete) return "The caverns should contain more interesting stuff than up on the surface right? Well look for awesome loot, and watch out for the bad guys! ";
            return "When you get down far enough, the walls around you should look more grey and rocky - that's the cavern layer. When you get there, report back to me or at an Expedition Board, and I can deliver some useful mining equipment! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.CompletedWelcomeQuest();
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if(!cond1) cond1 = (player.position.Y + player.height) * 2f / 16f - Main.worldSurface * 2.0 > 0;
            if(!cond2) cond2 = player.position.Y > Main.rockLayer * 16.0 + (double)(1080 / 2) + 16.0;
            return cond1 && cond2;
        }
    }
}
