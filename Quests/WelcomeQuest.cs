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
            expedition.difficulty = -12;
            expedition.deliver = true;
            expedition.important = true;
            expedition.conditionDescription = "Keep being awesome!";
            expedition.AddDeliverable(ItemID.DirtBlock, 151);
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

            Item i = new Item();
            i.SetDefaults(ItemID.SoulofFlight);
            i.stack = 50;
            expedition.AddReward(i);
            i = new Item();
            i.SetDefaults(ItemID.Shackle);
            i.prefix = 1;
            i.Prefix(i.prefix);
            expedition.AddReward(i);
        }
        /*
         
        public string conditionDescription = "";
        public int difficulty = 0;
        public bool trackingActive = false;
        public bool important = false;
        public bool explore = false;
        public bool deliver = false;
        public bool defeat = false;
        public bool completed = false;
        public bool partyShare = false;
        private List<KeyValuePair<int, int>> deliverables = new List<KeyValuePair<int, int>>();
        private List<Item> rewards = new List<Item>();
        */
    }
}
