using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public class ModExpedition
    {
        public Expedition expedition
        {
            get;
            internal set;
        }
        public Mod mod
        {
            get;
            internal set;
        }
        public ModExpedition()
        {
            this.expedition = new Expedition();
            this.expedition.mex = this;
        }

        /// <summary>
        /// Add an item with a specified stack to the expedition rewards.
        /// </summary>
        /// <param name="itemID">Item type</param>
        /// <param name="itemStack">The total amount in the stack, up to the maxStack</param>
        public void AddRewardItem(int itemID, int itemStack = 1)
        {
            Item i = new Item();
            i.SetDefaults(itemID);
            if (i.maxStack < itemStack) itemStack = i.maxStack;
            i.stack = itemStack;
            expedition.AddReward(i);
        }
        /// <summary>
        /// Add an item with a prefix to the expedition rewards.
        /// </summary>
        /// <param name="itemID">Item type</param>
        /// <param name="prefix">See AffixName_Old for a list of prefixes</param>
        public void AddRewardPrefix(int itemID, byte prefix = 0)
        {
            Item i = new Item();
            i.SetDefaults(itemID);
            i.prefix = prefix;
            i.Prefix(i.prefix);
            expedition.AddReward(i);
        }
        /// <summary>
        /// Add money reward. For simplicities sake use Item.buyPrice(int platinum, int gold, int silver, int copper) to get your value.
        /// </summary>
        /// <param name="value"></param>
        public void AddRewardMoney(int value)
        {
            int[] stacks = Expeditions.DivideValueIntoMoneyStack(value);
            if (stacks[0] > 0) AddRewardItem(74, stacks[0]);
            if (stacks[1] > 0) AddRewardItem(73, stacks[1]);
            if (stacks[2] > 0) AddRewardItem(72, stacks[2]);
            if (stacks[3] > 0) AddRewardItem(71, stacks[3]);
        }

        #region Virtual Methods
        /// <summary>
        /// The initialisation method for mods using this. Use it to set the title and category
        /// </summary>
        public virtual void SetDefaults()
        {

        }
        /// <summary>
        /// Called on world load, used to initialise the deliverables and rewards.
        /// </summary>
        public virtual void AddItemsOnLoad()
        {

        }
        /// <summary>
        /// Set the description of the expedition here.
        /// </summary>
        /// <param name="complete">Expedition is completed, sometimes you want to display different text.</param>
        /// <returns></returns>
        public virtual string Description(bool complete)
        {
            return "";
        }


        /// <summary>
        /// Put in any checks here to determine whether the expedition is complete, sans deliverables
        /// </summary>
        /// <param name="condition1">Used to keep track of a saved condition</param>
        /// <param name="condition2">Used to keep track of a saved condition</param>
        /// <param name="condition3">Used to keep track of a saved condition</param>
        /// <returns>True if custom conditions are met</returns>
        public virtual bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            return true;
        }

        /// <summary>
        /// Put in any checks here to determine whether the expedition is visible yet. Until they are met, expeditions will not call to check conditions, and do not appear in the browser. Effectively acts as if it is active or not. 
        /// </summary>
        /// <returns>True if prerequisites are met</returns>
        public virtual bool CheckPrerequisites(Player player)
        {
            return true;
        }

        /// <summary>
        /// Called before deducting items and granting the rewards of an expedition. 
        /// </summary>
        /// <param name="rewards">List of items to be rewarded</param>
        public virtual void PreCompleteExpedition(List<Item> rewards)
        {
            return;
        }
        /// <summary>
        /// Called after expedition is completed. 
        /// </summary>
        /// <param name="rewards">List of items to be rewarded</param>
        public virtual void PostCompleteExpedition(List<Item> rewards)
        {
            return;
        }

        /// <summary>
        /// Called on the dawn of each day, can be used with ResetExpedition to set up Daily Quests etc.
        /// </summary>
        public virtual void OnNewDay()
        {

        }
        #endregion
    }
}
