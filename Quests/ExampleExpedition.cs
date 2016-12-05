﻿using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class ExampleExpedition : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Hello World";
            expedition.difficulty = 1;
            expedition.deliver = true;
            expedition.important = false;
            expedition.repeatable = true;
            expedition.conditionDescription1 = "Stand on the ground";
            expedition.conditionDescription2 = "Run at 30mph";
            expedition.AddDeliverable(ItemID.DirtBlock, 1);
            /*
            expedition.AddDeliverable(ItemID.Silk, 151);
            expedition.AddDeliverable(ItemID.SiltBlock, 151);
            expedition.AddDeliverable(ItemID.SilverAndBlackDye, 151);
            expedition.AddDeliverable(ItemID.Bacon, 151);
            expedition.AddDeliverable(ItemID.YellowAndGreenBulb, 151);
            expedition.AddDeliverable(ItemID.Holly, 151);
            expedition.AddDeliverable(ItemID.Hive, 151);
            expedition.AddDeliverable(ItemID.GiantBow, 1);
            expedition.AddDeliverable(ItemID.AnglerFishBanner, 151);
            expedition.AddDeliverable(ItemID.AngryBonesBanner, 151);
            expedition.AddDeliverable(ItemID.BanquetTable, 151);
            expedition.AddDeliverable(ItemID.BatBanner, 151);
            expedition.AddDeliverable(ItemID.Bananarang, 6);
            */

            AddRewardItem(ItemID.DirtBlock, 2);
            AddRewardPrefix(ItemID.Shackle, 65);
        }
        public override string Description(bool complete)
        {
            if (complete) return "You've completed this repeatable expedition. Good for you! ";
            return "This is a sample quest, with a lot of random text to help build unnessecary amounts of space to create lines. ";
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            condition1 = player.velocity.Y == 0f;
            if (!condition2 && condition1) 
            {
                //if(Expeditions.DEBUG) Main.NewText(Math.Abs(player.velocity.X) + " is < 6");
                condition2 = Math.Abs(player.velocity.X) > 6;
            }
            return condition1 && condition2;
        }

        public override bool CheckPrerequisites(Player player)
        {
            if (Expeditions.DEBUG)
            {
                foreach (Item item in player.inventory)
                {
                    if (item.rare == -1) return true;
                }
            }
            return false;
        }
    }
}
