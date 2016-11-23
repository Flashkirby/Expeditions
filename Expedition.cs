using System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Expeditions
{
    public class Expedition
    {
        public ModExpedition mex
        {
            get;
            internal set;
        }

        /// <summary>Title of expedition</summary>
        public string title = "";
        /// <summary>Description of expedition</summary>
        public string description = "";
        /// <summary>Description of conditions to be met</summary>
        public string conditionDescription = "";
        /// <summary>Tier of expedition, same as item rarity</summary>
        public int difficulty = 0;
        /// <summary>Check if expedition is being tracked, this calls conditions met</summary>
        public bool trackingActive = false;
        /// <summary>Category: Is prioritised on the board</summary>
        public bool important = false;
        /// <summary>Category: Involves discovering things</summary>
        public bool explore = false;
        /// <summary>Category: Involves collecting items</summary>
        public bool deliver = false;
        /// <summary>Category: Involves defeating monsters</summary>
        public bool defeat = false;
        /// <summary>Completed expeditions are archived and cannot be redone unless repeatable</summary>
        public bool completed = false;
        /// <summary>Allows archived expeditions to be redone</summary>
        public bool repeatable = false;
        /// <summary>Calls expedition success to all party members when completed</summary>
        public bool partyShare = false;
        private List<KeyValuePair<int, int>> deliverables = new List<KeyValuePair<int, int>>();
        private List<Item> rewards = new List<Item>();

        /// <summary>
        /// Checks against all conditions to see if completeable
        /// </summary>
        /// <returns></returns>
        public bool ConditionsMet()
        {
            if (mex != null && !mex.CheckPrerequisites()) return false;
            if (deliverables.Count > 0)
            {
                //get as temp array of required
                int[] items = new int[deliverables.Count];
                int[] stacks = new int[deliverables.Count];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = deliverables[i].Key;
                    stacks[i] = deliverables[i].Value;
                }

                //keep track of stacks player has
                int[] hasStacks = new int[deliverables.Count];
                Item[] inventory = Main.player[Main.myPlayer].inventory;
                for (int i = 0; i < inventory.Length; i++)
                {
                    addToStackIfMatching(inventory[i], items, ref hasStacks);
                }

                //check to see if all item stacks are == or above
                try
                {
                    for (int i = 0; i < stacks.Length; i++)
                    {
                        if (hasStacks[i] < stacks[i]) return false;
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Check against itemTypes to see if it matches, if so add to corrosponding index in stack counter
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemTypes"></param>
        /// <param name="itemStackCount"></param>
        private void addToStackIfMatching(Item item, int[] itemTypes, ref int[] itemStackCount)
        {
            for (int i = 0; i < itemTypes.Length; i++)
            {
                if (item.type == itemTypes[i])
                {
                    itemStackCount[i] += item.stack;
                    return;
                }
            }
        }
        
        /// <summary>
        /// Toggles the state of this expedition being tracked
        /// </summary>
        /// <returns>The new state of the tracking</returns>
        public bool ToggleTrackingActive()
        {
            trackingActive = !trackingActive;
            return trackingActive;
        }

        /// <summary>
        /// Sets the expedition to complete, removing tracking and dropping items (check net as well)
        /// </summary>
        public void CompleteExpedition()
        {
            Main.NewText("yay you did it");
        }

        /// <summary>
        /// Add an item to be handed in for the expedition to be successful
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stack"></param>
        public void AddDeliverable(int type, int stack)
        {
            if (stack < 1) stack = 1;
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

        public Item[] GetDeliverablesArray()
        {
            Item[] deliverables = new Item[this.deliverables.Count];
            for (int i = 0; i < deliverables.Length; i++)
            {
                Item it = new Item();
                it.SetDefaults(this.deliverables[i].Key);
                it.stack = Math.Max(1, Math.Min(this.deliverables[i].Value, it.maxStack));
                deliverables[i] = it;
            }
            return deliverables;
        }

        /// <summary>
        /// Add an item to be given out to participants who finished the expedition
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stack"></param>
        public void AddReward(Item item)
        {
            rewards.Add(item);
        }
        public void AddReward(ModItem moditem)
        {
            AddReward(moditem.item);
        }

        public Item[] GetRewardsArray()
        {
            Item[] rewards = new Item[this.rewards.Count];
            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i] = this.rewards[i];
            }
            return rewards;
        }
        public Item[] GetRewardsCloneToArray()
        {
            Item[] rewards = new Item[this.rewards.Count];
            for(int i = 0; i < rewards.Length; i++)
            {
                rewards[i] = this.rewards[i].Clone();
            }
            return rewards;
        }
    }
}
