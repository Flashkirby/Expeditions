using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Expeditions.Quests
{
    public class WelcomeQuest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.title = "Hello World";
            expedition.description = "This is a sample quest, with a lot of random text to help build unnessecary amounts of space to create lines. ";
            expedition.descriptionCompleted = "You've completed this repeatable expedition. Good for you! ";
            expedition.difficulty = 1;
            expedition.deliver = true;
            expedition.important = true;
            expedition.repeatable = true;
            expedition.conditionDescription = "Stand on the ground";
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

        public override bool CheckConditions()
        {
            return Main.player[Main.myPlayer].velocity.Y == 0f;
        }

        public override bool CheckPrerequisites()
        {
            foreach(Item item in Main.player[Main.myPlayer].inventory)
            {
                if (item.rare == -1) return true;
            }
            return false;
        }
    }
}
