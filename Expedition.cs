using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions
{
    public class Expedition
    {
        public readonly static Color textColour = new Color(80, 255, 160);
        public readonly static Color muteColour = new Color(75, 150, 112);
        public readonly static Color poorColour = new Color(150, 75, 75);

        public ModExpedition mex
        {
            get;
            internal set;
        }

        /// <summary>Title of expedition</summary>
        public string name = "";
        /// <summary>Description of conditions to be met</summary>
        public string conditionDescription1 = "";
        public string conditionDescription2 = "";
        public string conditionDescription3 = "";
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
        /// <summary>Tracks the conditionals not related to deliverable items</summary>
        public bool condition1Met = false;
        public bool condition2Met = false;
        public bool condition3Met = false;
        /// <summary>Completed expeditions are archived and cannot be redone unless repeatable</summary>
        public bool completed = false;
        /// <summary>Allows archived expeditions to be redone</summary>
        public bool repeatable = false;
        /// <summary>Calls expedition success to all party members when completed</summary>
        public bool partyShare = false;
        private List<KeyValuePair<int, int>> deliverables = new List<KeyValuePair<int, int>>();
        private List<Item> rewards = new List<Item>();

        /// <summary> All conditions met </summary>
        private bool trackCondition = false;
        /// <summary> All items gathered </summary>
        private bool trackItems = false;

        /// <summary> Gets the description of this expedition </summary>
        public string GetDescription()
        {
            if (mex != null)
            {
                return mex.Description(completed);
            }
            return "";
        }


        /// <summary>
        /// Checks against all conditions to see if completeable
        /// </summary>
        /// <returns></returns>
        public bool ConditionsMet()
        {
            if (Main.netMode == 2) return false;
            bool meet1 = condition1Met;
            bool meet2 = condition2Met;
            bool meet3 = condition3Met;

            // check conditions
            bool checkConditions = mex.CheckConditions(Main.player[Main.myPlayer], ref condition1Met, ref condition2Met, ref condition3Met);
            if (!trackCondition && checkConditions) { trackCondition = true; }
            if (trackCondition && !checkConditions) { trackCondition = false; }

            // tracker
            if (trackingActive)
            {
                // Apply green colour to gains
                if (!meet1 && condition1Met) Main.NewText("Expedition Tracker: '" + name + "' " + conditionDescription1 + " accomplished!", muteColour.R, muteColour.G, muteColour.B);
                if (!meet2 && condition2Met) Main.NewText("Expedition Tracker: '" + name + "' " + conditionDescription2 + " accomplished!", muteColour.R, muteColour.G, muteColour.B);
                if (!meet3 && condition3Met) Main.NewText("Expedition Tracker: '" + name + "' " + conditionDescription3 + " accomplished!", muteColour.R, muteColour.G, muteColour.B);
                // Apply red colour to lossess
                if (meet1 && !condition1Met) Main.NewText("Expedition Tracker: '" + name + "' " + conditionDescription1 + " is no longer valid...", poorColour.R, poorColour.G, poorColour.B);
                if (meet2 && !condition2Met) Main.NewText("Expedition Tracker: '" + name + "' " + conditionDescription2 + " is no longer valid...", poorColour.R, poorColour.G, poorColour.B);
                if (meet3 && !condition3Met) Main.NewText("Expedition Tracker: '" + name + "' " + conditionDescription3 + " is no longer valid...", poorColour.R, poorColour.G, poorColour.B);
            }
            
            if (deliverables.Count > 0)
            {
                if (CheckRequiredItems())
                {
                    if(trackingActive && !trackItems)
                    {
                        Main.NewText("Expedition Tracker: '" + name + "' Collect expedition items accomplished!", muteColour.R, muteColour.G, muteColour.B);
                        trackItems = true;
                    }
                }
                else
                {
                    if (trackingActive && trackItems && Main.mouseItem.type == 0)
                    {
                        Main.NewText("Expedition Tracker: '" + name + "' Collect expedition items is no longer valid...", poorColour.R, poorColour.G, poorColour.B);
                        trackItems = false;
                    }
                    return false;
                }
            }
            return !(mex != null && !checkConditions);
        }

        public bool PrerequisitesMet()
        {
            if (Main.netMode == 2) return false;
            if (mex != null && !mex.CheckPrerequisites(Main.player[Main.myPlayer])) return false;
            return true;
        }

        private bool CheckRequiredItems(bool deductItems = false)
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
                addToStackIfMatching(inventory[i], items, ref hasStacks, stacks, deductItems);
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
            return true;
        }

        /// <summary>
        /// Check against itemTypes to see if it matches, if so add to corrosponding index in stack counter
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemTypes"></param>
        /// <param name="itemStackCount"></param>
        private void addToStackIfMatching(Item item, int[] itemTypes, ref int[] itemStackCount, int[]itemStacks, bool deductItems = false)
        {
            for (int i = 0; i < itemTypes.Length; i++)
            {
                if (item.type == itemTypes[i]) //the itemtype matches
                {
                    if (deductItems) //behaviour if removing items
                    {
                        int deductAmount = itemStacks[i] - itemStackCount[i];
                        if (item.stack > deductAmount)
                        {
                            //item stack is larger than amount remaining;
                            item.stack -= deductAmount;
                            itemStackCount[i] += item.stack;
                        }
                        else
                        {
                            //item stack is less than amount remaining;
                            item.SetDefaults(0);
                            itemStackCount[i] += deductAmount;
                        }
                    }
                    else //normal counting behaviour
                    {
                        //add to total owned in stack
                        itemStackCount[i] += item.stack;
                    }
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
            // check mod hook first
            if (mex != null && !mex.CompleteExpedition(rewards)) return;

            // deduct deliverables
            CheckRequiredItems(true);

            // grant items
            foreach (Item item in rewards)
            {
                Expeditions.ClientNetSpawnItem(item);
            }


            //complete this
            Main.PlaySound(24, -1, -1, 1);
            if (!repeatable || (repeatable && !completed))
            {
                Main.NewText("Expeditions: '" + name + "' completed!", textColour.R, textColour.G, textColour.B);
                Player p = Main.player[Main.myPlayer];
                Projectile.NewProjectile(p.Center, new Vector2(0f, -6f), ProjectileID.RocketFireworkBlue, 0, 0f, p.whoAmI);
            }
            else
            {
                Main.NewText("Expeditions: '" + name + "' recompleted!", textColour.R, textColour.G, textColour.B);
            }
            condition1Met = false;
            condition2Met = false;
            condition3Met = false;

            trackCondition = false;
            trackItems = false;
            trackingActive = false;

            completed = true;

            //net message sender
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
        public void ResetProgress()
        {
            completed = false;
            trackCondition = false;
            trackingActive = false;
            trackItems = false;
            condition1Met = false;
            condition2Met = false;
            condition3Met = false;
        }
        public void CopyProgress(Expedition e)
        {
            completed = e.completed;
            trackCondition = e.trackCondition;
            trackingActive = e.trackingActive;
            trackItems = e.trackItems;
            condition1Met = e.condition1Met;
            condition2Met = e.condition2Met;
            condition3Met = e.condition3Met;
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

        public static int GetHashID(Expedition expedition)
        {
            String identifier = "";
            int code = 0;

            // Set up the unique fields
            if (expedition.mex == null)
            { identifier = "Terraria@" + expedition.name; }
            else
            { identifier = expedition.mex.mod.Name + "@" + (object)(expedition.mex).GetType().Name; }

            // Custom runtime independant hash not dependant on runtime
            // I ran it >100000 times with random values and it got no collisions,
            // so it's probably safe enough...
            int salt = identifier.Length;
            foreach (char c in identifier)
            {
                code += c + salt;
                salt *= c + code;
            }
            return code;
        }
    }
}
