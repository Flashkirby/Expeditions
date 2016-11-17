using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Expeditions
{
    public class Expedition
    {
        public string title;
        public string description;
        public string conditionDescription;
        public int difficulty;
        public bool active;
        public bool important;
        public bool explore;
        public bool deliver;
        public bool defeat;
        public bool partyShare;
        private List<KeyValuePair<int, int>> deliverables = new List<KeyValuePair<int, int>>();
        private List<KeyValuePair<int, int>> rewards = new List<KeyValuePair<int, int>>();

        public bool Completed()
        {
            return true;
        }

        /// <summary>
        /// Add an item to be handed in for the expedition to be successful
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stack"></param>
        public void AddDeliverable(int type, int stack)
        {
            deliverables.Add(new KeyValuePair<int, int>(
                type,
                stack
                ));
        }
        public void AddDeliverable(Item item, int stack)
        {
            AddDeliverable(item.type, stack);
        }
        public void AddDeliverable(ModItem moditem, int stack)
        {
            AddDeliverable(moditem.item.type, stack);
        }

        /// <summary>
        /// Add an item to be given out to participants who finished the expedition
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stack"></param>
        public void AddReward(int type, int stack)
        {
            rewards.Add(new KeyValuePair<int, int>(
                type,
                stack
                ));
        }
        public void AddReward(Item item, int stack)
        {
            AddReward(item.type, stack);
        }
        public void AddReward(ModItem moditem, int stack)
        {
            AddReward(moditem.item.type, stack);
        }
    }
}
