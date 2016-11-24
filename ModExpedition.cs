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

        #region Virtual Methods
        /// <summary>
        /// The initialisation method for mods using this. Use it to set the title, rewards etc.
        /// </summary>
        public virtual void SetDefaults()
        {

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
        /// Put in any checks here to determine whether the expedition is visible yet
        /// </summary>
        /// <returns>True if prerequisites are met</returns>
        public virtual bool CheckPrerequisites(Player player)
        {
            return true;
        }

        /// <summary>
        /// Called before granting the rewards of an expedition
        /// </summary>
        /// <param name="rewards">List of items to be rewarded</param>
        /// <returns>True to actually drop reward items</returns>
        public virtual bool CompleteExpedition(List<Item> rewards)
        {
            return true;
        }
        #endregion
    }
}
