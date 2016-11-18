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
            expedition.description = "This is a sample quest";
            expedition.difficulty = 0;
            expedition.AddDeliverable(ItemID.DirtBlock, 1);

            Item i = new Item();
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
