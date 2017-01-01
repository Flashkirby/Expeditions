using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
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
        /// Add an item with a specified required stack to the expedition deliver conditions.
        /// </summary>
        /// <param name="itemID">Item type</param>
        /// <param name="itemStack">The total amount in the stack, up to the maxStack</param>
        public void AddDeliverable(int itemID, int itemStack = 1)
        {
            expedition.AddDeliverable(itemID, itemStack);
        }

        internal List<int[]> deliveryItemGroups = new List<int[]>();
        /// <summary>
        /// Adds a group of items with a specified required stack to the expedition deliver conditions. This can be used for optional deliveries, such as Cobalt OR Palladium sword.
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="itemStack"></param>
        public void AddDeliverableAnyOf(int[] itemIDs, int itemStack = 1)
        {
            if (itemIDs.Length <= 0) return;
            deliveryItemGroups.Add(itemIDs);
            expedition.AddDeliverable(itemIDs[0], itemStack);
        }

        internal bool ConvertCustomItems(ref int itemID)
        {
            foreach(int[] itemGroup in deliveryItemGroups)
            {
                foreach(int itemType in itemGroup)
                {
                    if(itemID == itemType)
                    {
                        itemID = itemGroup[0];
                        return true;
                    }
                }
            }
            return false;
        }
        internal string GetCustomItemGroupName(int firstItemIDInGroup)
        {
            foreach (int[] itemGroup in deliveryItemGroups)
            {
                if (firstItemIDInGroup == itemGroup[0]) // ItemID matches this group
                {
                    string itemNames = "";
                    Item i = new Item();
                    int index = 0;
                    foreach (int itemType in itemGroup)
                    {
                        i.SetDefaults(itemType);
                        itemNames += i.name;
                        if(itemGroup.Length > 1)
                        {
                            if(index < itemGroup.Length - 2)
                            {
                                itemNames += ", ";
                            }
                            else if (index < itemGroup.Length - 1)
                            {
                                itemNames += " or ";
                            }
                        }
                        index++;
                    }
                    return itemNames;
                }
            }
            return "Unknown Item Group";
        }

        /// <summary>
        /// Add an item with a specified stack to the expedition rewards.
        /// </summary>
        /// <param name="itemID">Item type</param>
        /// <param name="itemStack">The total amount in the stack, up to the maxStack</param>
        /// <param name="addTag">Should the item have [addTag] appended to it (eg. as a bonus item)</param>
        public void AddRewardItem(int itemID, int itemStack = 1, string addTag = "")
        {
            Item i = new Item();
            i.SetDefaults(itemID);
            if (i.maxStack < itemStack) itemStack = i.maxStack;
            i.stack = itemStack;

            if(addTag != "")
            {
                i.name = string.Concat("[", addTag, "] ", i.name);
            }

            expedition.AddReward(i);
        }
        /// <summary>
        /// Add an item with a prefix to the expedition rewards.
        /// </summary>
        /// <param name="itemID">Item type</param>
        /// <param name="prefix">See AffixName_Old for a list of prefixes</param>
        /// <param name="addTag">Should the item have [addTag] appended to it (eg. as a bonus item)</param>
        public void AddRewardPrefix(int itemID, byte prefix = 0, string addTag = "")
        {
            Item i = new Item();
            i.SetDefaults(itemID);
            i.prefix = prefix;
            i.Prefix(i.prefix);

            if (addTag != "")
            {
                i.name = string.Concat("[", addTag, "] ", i.name);
            }

            expedition.AddReward(i);
        }
        /// <summary>
        /// Add money reward. For simplicities sake use Item.buyPrice(int platinum, int gold, int silver, int copper) to get your value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addTag">Should the item have [addTag] appended to it (eg. as a bonus item)</param>
        public void AddRewardMoney(int value, string addTag = "")
        {
            int[] stacks = Expeditions.DivideValueIntoMoneyStack(value);
            if (stacks[0] > 0) AddRewardItem(74, stacks[0], addTag);
            if (stacks[1] > 0) AddRewardItem(73, stacks[1], addTag);
            if (stacks[2] > 0) AddRewardItem(72, stacks[2], addTag);
            if (stacks[3] > 0) AddRewardItem(71, stacks[3], addTag);
        }

        /// <summary>
        /// Attempts to set the head sprite associated with this npc.
        /// </summary>
        /// <param name="npcType">Type of NPC. Clerk's type is accessed via API.NPCIDClerk </param>
        public void SetNPCHead(int npcType)
        {
            //First run through known vanilla NPC head slots
            switch (npcType)
            {
                case NPCID.Guide: expedition.npcHead = 1; return;
                case NPCID.Merchant: expedition.npcHead = 2; return;
                case NPCID.Nurse: expedition.npcHead = 3; return;
                case NPCID.Demolitionist: expedition.npcHead = 4; return;
                case NPCID.Dryad: expedition.npcHead = 5; return;
                case NPCID.ArmsDealer: expedition.npcHead = 6; return;
                case NPCID.Clothier: expedition.npcHead = 7; return;
                case NPCID.Mechanic: expedition.npcHead = 8; return;
                case NPCID.GoblinTinkerer: expedition.npcHead = 9; return;
                case NPCID.Wizard: expedition.npcHead = 10; return;
                case NPCID.SantaClaus: expedition.npcHead = 11; return;
                case NPCID.Truffle: expedition.npcHead = 12; return;
                case NPCID.Steampunker: expedition.npcHead = 13; return;
                case NPCID.DyeTrader: expedition.npcHead = 14; return;
                case NPCID.PartyGirl: expedition.npcHead = 15; return;
                case NPCID.Cyborg: expedition.npcHead = 16; return;
                case NPCID.Painter: expedition.npcHead = 17; return;
                case NPCID.WitchDoctor: expedition.npcHead = 18; return;
                case NPCID.Pirate: expedition.npcHead = 19; return;
                case NPCID.Stylist: expedition.npcHead = 20; return;
                case NPCID.TravellingMerchant: expedition.npcHead = 21; return;
                case NPCID.Angler: expedition.npcHead = 22; return;
                case NPCID.TaxCollector: expedition.npcHead = 23; return;
                case NPCID.DD2Bartender: expedition.npcHead = 24; return;
                default:
                    try
                    {
                        //Then if all else fails, see if a modded one exists
                        MethodInfo method = typeof(NPCHeadLoader).GetMethod("GetNPCHeadSlot", BindingFlags.NonPublic | BindingFlags.Static);
                        object result = method.Invoke(null, new object[] { npcType });
                        expedition.npcHead = (int)result;
                        //PlayerExplorer.dbgmsg += "\ninvoke result: " + result;
                    }
                    catch { }
                    break;
            }
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
        /// Put in any checks here to determine whether to modify count. 
        /// If count >= max, this condition is cleared. 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="count"></param>
        /// <param name="max"></param>
        public virtual void CheckConditionCountable(Player player, ref int count, int max)
        {

        }

        /// <summary>
        /// Put in any checks here to determine whether the expedition is complete, sans deliverables and the counted condition.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cond1">Used to keep track of a saved condition</param>
        /// <param name="cond2">Used to keep track of a saved condition</param>
        /// <param name="cond3">Used to keep track of a saved condition</param>
        /// <param name="condCount">Used to keep track of a the counted condition</param>
        /// <returns>True if custom conditions are met</returns>
        public virtual bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
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
        /// Use this to modify the rewards before distributing, and checking which 
        /// of the "any deliverables" were turned in, in inventory order.
        /// A caution, the lists won't necessarily be correct in multiplayer if a
        /// quest is completed through party share.
        /// </summary>
        /// <param name="rewards">List of items to be rewarded. </param>
        /// <param name="deliveredItems">List of items being delivered. </param>
        public virtual void PreCompleteExpedition(List<Item> rewards, List<Item> deliveredItems)
        {
            return;
        }
        /// <summary>
        /// Called after expedition is completed. 
        /// </summary>
        public virtual void PostCompleteExpedition()
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
