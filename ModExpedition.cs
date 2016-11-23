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
        /// <returns>True if conditions are met</returns>
        public virtual bool CheckConditions()
        {
            return true;
        }

        /// <summary>
        /// Put in any checks here to determine whether the expedition is visible yet
        /// </summary>
        /// <returns>True if prerequisites are met</returns>
        public virtual bool CheckPrerequisites()
        {
            return true;
        }
        #endregion

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
    }
}
