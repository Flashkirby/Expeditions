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
            expedition.difficulty = 1;
            expedition.ctgExplore = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Enter the Underground";
            expedition.conditionDescription2 = "Reach the Cavern layer";
        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.CopperChainmail, 1);
            AddRewardItem(ItemID.CopperGreaves, 1);
            AddRewardItem(ItemID.IronPickaxe, 1);
            AddRewardItem(ItemID.Torch, 99);
            AddRewardItem(ItemID.RopeCoil, 10);
            AddRewardItem(ItemID.SpelunkerPotion, 10);
        }
        public override string Description(bool complete)
        {
            return "When you reach the caverns, report back for some basic tools. You should be able to tell by the surrounding walls becoming noticeably more rocky. More expeditions should be available after this. ";
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if(!condition1) condition1 = (player.position.Y + player.height) * 2f / 16f - Main.worldSurface * 2.0 > 0;
            if(!condition2) condition2 = player.position.Y > Main.rockLayer * 16.0 + (double)(1080 / 2) + 16.0;
            return condition1 && condition2;
        }
    }
}
