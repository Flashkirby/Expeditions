using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Expeditions
{
    /// <summary>
    /// Class that holds all the important data for expeditions.
    /// </summary>
    public class Expedition
    {
        /// <summary> Colour of important text. Bright green. </summary>
        public readonly static Color textColour = new Color(80, 255, 160);
        /// <summary> Colour of mild positive text. Dull green. </summary>
        public readonly static Color muteColour = new Color(75, 150, 112);
        /// <summary> Colour of mild negative text. Dull red. </summary>
        public readonly static Color poorColour = new Color(150, 75, 75);

        public static string ExpeditionBonusCondTag = "(Bonus) ";
        public static string ExpeditionBonusTag = "Bonus";
        public static string ExpeditionTrackerTemplate = "Expedition Tracker: ";
        public static string ExpeditionTrackerAccomplished = " accomplished!";
        public static string ExpeditionTrackerNotValid = " is no longer valid...";

        public ModExpedition mex
        {
            get;
            internal set;
        }

        /// <summary>Title of expedition</summary>
        public string name = "";
        /// <summary>The NPC Head that will be shown with the quest. By default, 0 and invalid integers will show the expediiton board. See ModExpedition's SetNPCHead()</summary>
        public int npcHead = 0;
        /// <summary> Quest will not appear unless this NPC is present, if >0 </summary>
        public int requireNPC = 0;
        /// <summary>Description of a condition to be met - see condition1Met</summary>
        public string conditionDescription1 = "";
        /// <summary>Description of a condition to be met - see condition2Met</summary>
        public string conditionDescription2 = "";
        /// <summary>Description of a condition to be met - see condition3Met</summary>
        public string conditionDescription3 = "";
        /// <summary>Description of a condition to be met - see conditionCount</summary>
        public string conditionDescriptionCountable = "";
        /// <summary>Tier of expedition, same as item rarity</summary>
        public int difficulty = 0;
        /// <summary>Check if expedition is being tracked, this calls conditions met</summary>
        public bool trackingActive = false;
        /// <summary>Category: Is prioritised on the board</summary>
        public bool ctgImportant = false;
        /// <summary>Category: Involves discovering things, is default if not specified</summary>
        public bool ctgExplore = false;
        /// <summary>Category: Involves collecting items</summary>
        public bool ctgCollect = false;
        /// <summary>Category: Involves defeating monsters</summary>
        public bool ctgSlay = false;
        /// <summary>Tracks a conditional, will be displayed when conditionDescription1 is not empty</summary>
        public bool condition1Met = false;
        /// <summary>Tracks a conditional, will be displayed when conditionDescription2 is not empty</summary>
        public bool condition2Met = false;
        /// <summary>Tracks a conditional, will be displayed when conditionDescription3 is not empty</summary>
        public bool condition3Met = false;
        /// <summary>Tracks a conditional, will be displayed when conditionDescriptionCountable is not empty</summary>
        public int conditionCounted = 0;
        /// <summary>Tracks a conditional, will be displayed when conditionDescriptionCountable is not empty and is > 0. Returns true by default when set to 0. </summary>
        public int conditionCountedMax = 0;

        /// <summary> Should the tracker tell the player when the count is halfway complete? TrackQuarterCompleted will override this. </summary>
        public bool conditionCountedTrackHalfCompleted = false;
        /// <summary> Should the tracker tell the player when each quarter of the count is reached? This field will override TrackHalfCompleted. </summary>
        public bool conditionCountedTrackQuarterCompleted = false;

        /// <summary> Do not show a quest prompt when unlocked </summary>
        public bool hideQuestUnlock = false;
        /// <summary>Completed expeditions are archived and cannot be redone unless repeatable</summary>
        public bool completed = false;
        /// <summary>Allows archived expeditions to be redone</summary>
        public bool repeatable = false;
        /// <summary>Calls expedition success to all party members when completed. Does not work with repeatable after the first completion</summary>
        public bool partyShare = false;
        
        /// <summary> Deliverables will substitute any wood in the player's inventory. </summary>
        public bool anyWood = true;
        /// <summary> Deliverables will substitute any ores and bars in the player's inventory. </summary>
        public bool anySameTierOreBar = true;

        private List<KeyValuePair<int, int>> deliverables = new List<KeyValuePair<int, int>>();
        public bool hasDeliverables { get { return deliverables.Count > 0; } }
        private List<Item> rewards = new List<Item>();
        private List<Item> oneTimeRewards = new List<Item>();

        /// <summary> All conditions met </summary>
        private bool trackCondition = false;
        /// <summary> All items gathered </summary>
        private bool trackItems = false;
        
        private bool lastPrereq = true; // The last result of checkPrerequisites

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
        /// Updates the countable once per frame, after which NPC trackers are reset
        /// </summary>
        public void UpdateCountable()
        {
            if (mex != null)
            {
                meetc = conditionCounted >= conditionCountedMax;
                mex.CheckConditionCountable(Main.LocalPlayer, ref conditionCounted, conditionCountedMax);
            }
        }
        private bool meetc = false;
        private bool lastCond1 = false;
        private bool lastCond2 = false;
        private bool lastCond3 = false;
        private int lastCounted = 0; // The previous counted value

        /// <summary>
        /// Checks against all conditions to see if completeable. 
        /// </summary>
        /// <returns></returns>
        public bool ConditionsMet()
        {
            if (Main.netMode == 2) return false;

            // check conditions
            bool checkConditions = true;
            if (mex != null) checkConditions = mex.CheckConditions(Main.LocalPlayer, ref condition1Met, ref condition2Met, ref condition3Met, conditionCounted >= conditionCountedMax);

            if (!trackCondition && checkConditions) { trackCondition = true; }
            if (trackCondition && !checkConditions) { trackCondition = false; }

            // tracker
            bool showText = Expeditions.ShowTrackingText;
            if (trackingActive)
            {
                // Apply green colour to gains
                if (!lastCond1 && condition1Met && conditionDescription1 != "")
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescription1 + ExpeditionTrackerAccomplished, muteColour.R, muteColour.G, muteColour.B);
                    }
                }
                if (!lastCond2 && condition2Met && conditionDescription2 != "")
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescription2 + ExpeditionTrackerAccomplished, muteColour.R, muteColour.G, muteColour.B);
                    }
                }
                if (!lastCond3 && condition3Met && conditionDescription3 != "")
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescription3 + ExpeditionTrackerAccomplished, muteColour.R, muteColour.G, muteColour.B);
                    }
                }
                if (!meetc && conditionDescriptionCountable != "")
                {
                    if (Expeditions.DEBUG && conditionCountedMax > 0 && conditionCounted > 0) Main.NewText(conditionCounted + " from " + lastCounted);
                    if (conditionCounted >= conditionCountedMax)
                    {
                        if (showText)
                        {
                            Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescriptionCountable + ExpeditionTrackerAccomplished, muteColour.R, muteColour.G, muteColour.B);
                        }
                    }
                    else if (
                       conditionCountedTrackQuarterCompleted ||
                       conditionCountedTrackHalfCompleted
                       )
                    {
                        if (
                            (lastCounted < conditionCountedMax / 2 &&
                            conditionCounted >= conditionCountedMax / 2
                            ) ||
                            (lastCounted < conditionCountedMax / 4 &&
                            conditionCounted >= conditionCountedMax / 4 &&
                            conditionCountedTrackQuarterCompleted
                            ) ||
                            (lastCounted < 3 * conditionCountedMax / 4 &&
                            conditionCounted >= 3 * conditionCountedMax / 4 &&
                            conditionCountedTrackQuarterCompleted)
                            )
                        {
                            if (showText)
                            {
                                Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescriptionCountable + " Progress is [" + conditionCounted + "/" + conditionCountedMax + "]", muteColour.R, muteColour.G, muteColour.B);
                            }
                        }
                    }
                }

                // Apply red colour to lossess
                if (lastCond1 && !condition1Met)
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescription1 + ExpeditionTrackerNotValid, poorColour.R, poorColour.G, poorColour.B);
                    }
                }
                if (lastCond2 && !condition2Met)
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescription2 + ExpeditionTrackerNotValid, poorColour.R, poorColour.G, poorColour.B);
                    }
                }
                if (lastCond3 && !condition3Met)
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescription3 + ExpeditionTrackerNotValid, poorColour.R, poorColour.G, poorColour.B);
                    }
                }
                if (meetc && conditionCounted < conditionCountedMax)
                {
                    if (showText)
                    {
                        Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' " + conditionDescriptionCountable + ExpeditionTrackerNotValid, poorColour.R, poorColour.G, poorColour.B);
                    }
                }
            }

            if (lastCond1 != condition1Met ||
                lastCond2 != condition2Met ||
                lastCond3 != condition3Met ||
                lastCounted != conditionCounted)
            {
                TrackerUI.recentChangeTick = TrackerUI.ChangeTickMax;
            }

            // Set coditions after checking
            lastCond1 = condition1Met;
            lastCond2 = condition2Met;
            lastCond3 = condition3Met;
            // Set last counted after use
            lastCounted = conditionCounted;

            if (deliverables.Count > 0)
            {
                if (CheckRequiredItems())
                {
                    if (trackingActive && !trackItems)
                    {
                        if (showText)
                        {
                            Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' Collect expedition items" + ExpeditionTrackerAccomplished, muteColour.R, muteColour.G, muteColour.B);
                        }
                        TrackerUI.recentChangeTick = TrackerUI.ChangeTickMax;
                        trackItems = true;
                    }
                }
                else
                {
                    if (trackingActive && trackItems && Main.mouseItem.type == 0)
                    {
                        if (showText)
                        {
                            Main.NewText(ExpeditionTrackerTemplate + "'" + name + "' Collect expedition items" + ExpeditionTrackerNotValid, poorColour.R, poorColour.G, poorColour.B);
                        }
                        TrackerUI.recentChangeTick = TrackerUI.ChangeTickMax;
                        trackItems = false;
                    }
                    return false;
                }
            }

            return (mex == null || checkConditions) && meetc;
        }
        /// <summary>
        /// Checks against arbitrary conditions to see if visible. 
        /// </summary>
        /// <returns></returns>
        public bool PrerequisitesMet()
        {
            if (Main.netMode == 2) return false;

            // Don't show if NPC not around and uncomplete
            if(requireNPC > 0 && !completed)
            {
                if (NPC.FindFirstNPC(requireNPC) < 0) return false;
            }

            // Check the mod call (I can't remember why this has array errors sometimes,
            // Lazy try catch until I can think of what to do
            try
            {
                if (mex != null && !mex.CheckPrerequisites(Main.LocalPlayer, ref condition1Met, ref condition2Met, ref condition3Met, conditionCounted >= conditionCountedMax))
                {
                    lastPrereq = false;
                    return false;
                }
            }catch
            {
                return false;
            }

            if (!hideQuestUnlock && !lastPrereq)
            {
                if (WorldExplore.IsCurrentDaily(this))
                {
                    Expeditions.DisplayUnlockedExpedition(this, "Daily Expedition: ");
                } else
                {
                    Expeditions.DisplayUnlockedExpedition(this);
                }
            }

            lastPrereq = true;
            return true;
        }

        internal bool CheckRequiredItems(bool deductItems = false)
        {
            //get as temp array of required
            int[] requiredItems = new int[deliverables.Count];
            int[] requiredStack = new int[deliverables.Count];
            for (int i = 0; i < requiredItems.Length; i++)
            {
                requiredItems[i] = deliverables[i].Key;
                requiredStack[i] = deliverables[i].Value;
            }

            //keep track of stacks player has, as this the total number can be divided across multiple
            int[] countedStack = new int[deliverables.Count];
            Item[] inventory = Main.LocalPlayer.inventory;
            for (int i = 0; i < inventory.Length; i++)
            {
                addToStackIfMatching(inventory[i], requiredItems, ref countedStack, requiredStack, deductItems);
            }

            //check to see if all item stacks are == or above
            try
            {
                for (int i = 0; i < requiredStack.Length; i++)
                {
                    if (countedStack[i] < requiredStack[i]) // Player doesn't have enough of an item
                    {
                        // if (Expeditions.DEBUG && trackingActive) Main.NewText("Missing " + Lang.itemName(items[i]));
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check against itemTypes to see if it matches, if so add to corrosponding index in stack counter. 
        /// </summary>
        private bool addToStackIfMatching(Item item, int[] itemTypes, ref int[] itemStackCount, int[]itemTotalStack, bool deductItems = false)
        {
            for (int i = 0; i < itemTypes.Length; i++)
            {
                int type = item.type;
                int checkedType = itemTypes[i];
                ConvertIDToSameTierOreBar(ref type);
                ConvertIDToSameTierOreBar(ref checkedType);

                if (type == checkedType) //the itemtype matches
                {
                    // Does this item stack actually count towards deliverables
                    bool contributeToRequirement = false;
                    contributeToRequirement = itemStackCount[i] < itemTotalStack[i];

                    // Behaviour if removing items
                    // But only if it is still less 
                    // (otherwise will cause item duplicaiton glitch)
                    if (deductItems && contributeToRequirement) 
                    {
                        int deductAmount = itemTotalStack[i] - itemStackCount[i];
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
                    return contributeToRequirement;
                }
            }
            return false;
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
        /// Sets the expedition to complete, removing tracking and dropping items (check net as well). 
        /// </summary>
        /// <param name="serverMessage">Force the method to forego sending packets, and just run. </param>
        public void CompleteExpedition(bool serverMessage = false)
        {
            // What to do if the call wasn't made from a server's packet send (ie. client code)
            if (!serverMessage)
            {
                // Check if this is a shared expedition on a multiplayer server
                if (partyShare && Main.netMode == 1 && mex != null)
                {
                    if (Expeditions.DEBUG) Main.NewText("#sending");
                    // Send net message and return early without running the rest of the code
                    Expeditions.SendNet_PartyComplete(
                        this.mex.mod,
                        Main.LocalPlayer.team,
                        this.mex);
                    return;
                }
            }

            if (Expeditions.DEBUG) Main.NewText("#complete repeat");
            // What if I receive this from someone else who hasn't finished it yet?
            if (completed && !repeatable) return;

            // Initialise a new set of rewards that don't affect the original listed
            List<Item> tempRewards = new List<Item>();
            if (!completed)
            {
                foreach (Item i in oneTimeRewards)
                {
                    // Create new editable instance of this item
                    Item item = new Item();
                    item.SetDefaults(i.type);
                    item.Prefix(i.prefix);
                    item.stack = i.stack;
                    tempRewards.Add(item);
                }
            }
            foreach (Item i in rewards)
            {
                // Create new editable instance of this item
                Item item = new Item();
                item.SetDefaults(i.type);
                item.Prefix(i.prefix);
                item.stack = i.stack;
                tempRewards.Add(item);
            }

            List<Item> validDeliverables = new List<Item>();
            GetConsumedDeliverables(validDeliverables);

            // check mod hook
            mex.PreCompleteExpedition(tempRewards, validDeliverables);

            // deduct deliverables
            CheckRequiredItems(true);

            // grant items
            foreach (Item item in tempRewards)
            {
                Expeditions.ClientNetSpawnItem(item);
            }

            if (Expeditions.DEBUG) Main.NewText("#compelte quest");
            //complete this
            Main.PlaySound(24, -1, -1, 1);
            if (!repeatable || (repeatable && !completed))
            {
                Main.NewText("Expeditions: '" + name + "' completed!", textColour.R, textColour.G, textColour.B);
                Player p = Main.LocalPlayer;
                Projectile.NewProjectile(p.Center, new Vector2(0f, -6f), ProjectileID.RocketFireworkBlue, 0, 0f, p.whoAmI);
                TrackerUI.recentChangeTick = TrackerUI.ChangeTickMax;
            }
            else
            {
                Main.NewText("Expeditions: '" + name + "' recompleted!", textColour.R, textColour.G, textColour.B);
                TrackerUI.recentChangeTick = TrackerUI.ChangeTickMax;
            }

            if (Expeditions.DEBUG) Main.NewText("#set progress");
            if (!repeatable)
            {
                // Save conditions used to finish, on 1 time expeditions
                // So just disable tracking
                trackCondition = false;
                trackingActive = false;
                trackItems = false;
            }
            else
            {
                // Repeatables always restart once completed
                ResetProgress(false);
            }
            completed = true;

            if (Expeditions.DEBUG) Main.NewText("#postcomplete, complete = " + completed);
            // check mod hook
            mex.PostCompleteExpedition();
            
            // Force the expeditions list to recalculate in this instance
            if (ExpeditionUI.visible)
            {
                Expeditions.expeditionUI.ListRecalculate();
            }
        }
        
        /// <summary>
        /// Check if this quest is viable to be selected for a daily. This will automatically reset it when it is added. Keep in mind this is called by server so code accordingly
        /// </summary>
        public bool CheckDailyAssigned()
        {
            if (mex != null)
            {
                return mex.IncludeAsDaily();
            }
            return false;
        }

        private void GetConsumedDeliverables(List<Item> validDeliverables)
        {
            //get as temp array of required
            int[] items = new int[deliverables.Count];
            int[] stacks = new int[deliverables.Count];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = deliverables[i].Key;
                stacks[i] = deliverables[i].Value;
            }

            //keep track of stacks player has, as this the total number can be divided across multiple
            int[] hasStacks = new int[deliverables.Count];
            Item[] inventory = Main.LocalPlayer.inventory;
            for (int i = 0; i < inventory.Length; i++)
            {
                // Item in inventory matches, we'll add it to the items being delivered
                if (addToStackIfMatching(inventory[i], items, ref hasStacks, stacks))
                {
                    validDeliverables.Add(inventory[i]);
                }
            }
        }

        /// <summary> Add items and such after world init. </summary>
        public void WorldInitialise()
        {
            lastPrereq = true;

            deliverables.Clear();
            rewards.Clear();
            oneTimeRewards.Clear();

            PlayerExplorer.dbgmsg += "\n" + WorldGen.GoldTierOre + " | " + TileID.Gold + " : " + WorldGen.oreTier1 + " | " + TileID.Cobalt;

            if (mex != null)
            {
                mex.deliveryItemGroups.Clear();

                mex.AddItemsOnLoad();
            }

            // Reset last state tracker
            lastCond1 = condition1Met;
            lastCond2 = condition2Met;
            lastCond3 = condition3Met;
            lastCounted = conditionCounted; 
    }

        /// <summary>
        /// Add an item to be handed in for the expedition to be successful. 
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
        /// <summary>
        /// Add an item to be handed in for the expedition to be successful. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stack"></param>
        public void AddDeliverable(Item item, int stack)
        {
            AddDeliverable(item.type, stack);
        }
        /// <summary>
        /// Add an item to be handed in for the expedition to be successful. 
        /// </summary>
        /// <param name="moditem"></param>
        /// <param name="stack"></param>
        public void AddDeliverable(ModItem moditem, int stack)
        {
            AddDeliverable(moditem.item.type, stack);
        }

        /// <summary>
        /// Gets the required items list as an array. 
        /// </summary>
        /// <returns></returns>
        public Item[] GetDeliverablesArray()
        {
            Item[] deliverables = new Item[this.deliverables.Count];
            for (int i = 0; i < deliverables.Length; i++)
            {
                Item it = new Item();
                it.SetDefaults(this.deliverables[i].Key);
                it.stack = Math.Max(1, Math.Min(this.deliverables[i].Value, it.maxStack));

                int itemType = it.type;
                int converted = ConvertIDToSameTierOreBar(ref itemType);
                if (anyWood && converted > 0)
                {
                    if (converted != 3)
                    {
                        if (itemType == ItemID.DemoniteBar)
                        {
                            it.SetNameOverride("Any" + Lang.GetNPCName(NPCID.Demon) + " Bar");
                        }
                        else if (itemType == ItemID.DemoniteOre)
                        {
                            it.SetNameOverride("Any" + Lang.GetNPCName(NPCID.Demon) + " Ore");
                        }
                        else
                        {
                            it.SetNameOverride("Any" + Lang.GetItemName(itemType));
                        }
                    }
                    else
                    {
                        it.SetNameOverride(mex.GetCustomItemGroupName(itemType));
                    }
                }

                deliverables[i] = it;
            }
            return deliverables;
        }
        /// <summary>
        /// Reset the progress and tracking on this expedition. 
        /// </summary>
        /// <param name="resetComplete">Should we also reset expedition complete?</param>
        public void ResetProgress(bool resetComplete = true)
        {
            if (resetComplete) completed = false;
            trackCondition = false;
            trackingActive = false;
            trackItems = false;
            condition1Met = false;
            condition2Met = false;
            condition3Met = false;
            conditionCounted = 0;
        }
        /// <summary>
        /// Copy attributes of an expedition from the target. 
        /// </summary>
        /// <param name="e">The Expedition to copy progress from</param>
        public void CopyProgress(Expedition e)
        {
            completed = e.completed;
            trackCondition = e.trackCondition;
            trackingActive = e.trackingActive;
            trackItems = e.trackItems;
            condition1Met = e.condition1Met;
            condition2Met = e.condition2Met;
            condition3Met = e.condition3Met;
            conditionCounted = e.conditionCounted;
        }
        /// <summary>
        /// Modifies the item type if it matches a tiered group. Reads the "any*" bools to determine converting or not.
        /// </summary>
        /// <param name="itemID">The item type which can be changed. DO NOT pass in the raw item.type value. </param>
        /// <returns>True if an item matches a conversion group. </returns>
        public int ConvertIDToSameTierOreBar(ref int itemID)
        {
            if (RecipeGroup.recipeGroups[RecipeGroupID.Wood].ValidItems.Contains(itemID)
                && anyWood)
            {
                itemID = ItemID.Wood;
                return 1;
            }

            if (anySameTierOreBar)
            {
                #region Ores
                if (itemID == ItemID.CopperOre || itemID == ItemID.TinOre)
                { itemID = ItemID.CopperOre; return 2; }
                if (itemID == ItemID.IronOre || itemID == ItemID.LeadOre)
                { itemID = ItemID.IronOre; return 2; }
                if (itemID == ItemID.SilverOre || itemID == ItemID.TungstenOre)
                { itemID = ItemID.SilverOre; return 2; }
                if (itemID == ItemID.GoldOre || itemID == ItemID.PlatinumOre)
                { itemID = ItemID.GoldOre; return 2; }

                if (itemID == ItemID.DemoniteOre || itemID == ItemID.CrimtaneOre)
                { itemID = ItemID.DemoniteOre; return 2; }

                if (itemID == ItemID.CobaltOre || itemID == ItemID.PalladiumOre)
                { itemID = ItemID.CobaltOre; return 2; }
                if (itemID == ItemID.MythrilOre || itemID == ItemID.OrichalcumOre)
                { itemID = ItemID.MythrilOre; return 2; }
                if (itemID == ItemID.AdamantiteOre || itemID == ItemID.TitaniumOre)
                { itemID = ItemID.AdamantiteOre; return 2; }
                #endregion
                #region Bars
                if (itemID == ItemID.CopperBar || itemID == ItemID.TinBar)
                { itemID = ItemID.CopperBar; return 2; }
                if (itemID == ItemID.IronBar || itemID == ItemID.LeadBar)
                { itemID = ItemID.IronBar; return 2; }
                if (itemID == ItemID.SilverBar || itemID == ItemID.TungstenBar)
                { itemID = ItemID.SilverBar; return 2; }
                if (itemID == ItemID.GoldBar || itemID == ItemID.PlatinumBar)
                { itemID = ItemID.GoldBar; return 2; }

                if (itemID == ItemID.DemoniteBar || itemID == ItemID.CrimtaneBar)
                { itemID = ItemID.DemoniteBar; return 2; }

                if (itemID == ItemID.CobaltBar || itemID == ItemID.PalladiumBar)
                { itemID = ItemID.CobaltBar; return 2; }
                if (itemID == ItemID.MythrilBar || itemID == ItemID.OrichalcumBar)
                { itemID = ItemID.MythrilBar; return 2; }
                if (itemID == ItemID.AdamantiteBar || itemID == ItemID.TitaniumBar)
                { itemID = ItemID.AdamantiteBar; return 2; }
                #endregion
            }

            if (mex != null)
            {
                if(mex.ConvertCustomItems(ref itemID))
                {
                    return 3;
                }
            }
            return 0;
        }

        /// <summary>
        /// Add an item to be given out to participants who finished the expedition. 
        /// </summary>
        /// <param name="item"></param>
        public void AddReward(Item item)
        {
            rewards.Add(item);
        }
        /// <summary>
        /// Add an item to be given out to participants who finished the expedition. 
        /// </summary>
        /// <param name="moditem"></param>
        public void AddReward(ModItem moditem)
        {
            AddReward(moditem.item);
        }

        /// <summary>
        /// Add an item to be given only ONCE when an expedition is finished (no repeatables)
        /// </summary>
        /// <param name="item"></param>
        public void AddRewardOnce(Item item)
        {
            oneTimeRewards.Add(item);
        }
        /// <summary>
        /// Add an item to be given only ONCE when an expedition is finished (no repeatables)
        /// </summary>
        /// <param name="item"></param>
        public void AddRewardOnce(ModItem moditem)
        {
            oneTimeRewards.Add(moditem.item);
        }

        /// <summary>
        /// Gets the current reward list as an array. 
        /// </summary>
        /// <returns></returns>
        public Item[] GetRewardsArray()
        {
            Item[] rewards = new Item[this.rewards.Count];
            if (!completed)
            {
                rewards = new Item[this.oneTimeRewards.Count + this.rewards.Count];
                for (int i = 0; i < oneTimeRewards.Count; i++)
                {
                    rewards[i] = this.oneTimeRewards[i];
                }
                for (int i = 0; i < this.rewards.Count; i++)
                {
                    rewards[i + oneTimeRewards.Count] = this.rewards[i];
                }
            }
            else
            {
                for (int i = 0; i < rewards.Length; i++)
                {
                    rewards[i] = this.rewards[i];
                }
            }
            return rewards;
        }
        /// <summary>
        /// Clones the items in the rewards list and returns it in a new array. 
        /// </summary>
        /// <returns></returns>
        public Item[] GetRewardsCloneToArray()
        {
            Item[] rewards = new Item[this.rewards.Count];
            if (!completed)
            {
                rewards = new Item[this.oneTimeRewards.Count + this.rewards.Count];
                for (int i = 0; i < oneTimeRewards.Count; i++)
                {
                    rewards[i] = this.oneTimeRewards[i].Clone();
                }
                for (int i = 0; i < this.rewards.Count; i++)
                {
                    rewards[i + oneTimeRewards.Count] = this.rewards[i].Clone();
                }
            }
            else
            {
                for (int i = 0; i < rewards.Length; i++)
                {
                    rewards[i] = this.rewards[i].Clone();
                }
            }
            return rewards;
        }

        /// <summary>
        /// Returns the unique ID of this expedition generated by a consistent algorithm,
        /// based on the mod name and expedition's class name. 
        /// </summary>
        /// <param name="expedition"></param>
        /// <returns></returns>
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
        public int GetHashID()
        {
            return Expedition.GetHashID(this);
        }

        public static bool CompareExpeditions(Expedition e1, Expedition e2)
        {
            return GetHashID(e1) == GetHashID(e2);
        }
    }
}
