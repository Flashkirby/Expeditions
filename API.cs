using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    /// <summary>
    /// Big Friendly Class provides access to all the important and useful methods
    /// required to interface with this mod!
    /// <para>
    /// To create an expedition, create a new .cs file which extends ModExpedition. 
    /// Once you've set them all up, add them to the game via the Mod class' Load()
    /// override method by calling AddExpedition()
    /// </para> 
    /// </summary>
    public static class API
    {
        #region Fields

        /// <summary> The NPC that was last struck my the player this frame, or an empty NPC type 0 </summary>
        public static NPC LastHitNPC { get { return Expeditions.LastHitNPC; } }
        /// <summary> The NPC that was last killed by the player this frame, or an empty NPC type 0 </summary>
        public static NPC LastKilledNPC { get { return Expeditions.LastKilledNPC; } }

        /// <summary>
        /// A list of tiles to check for onscreen.
        /// </summary>
        public static List<ushort> TileCheckList { get { return WorldExplore.TileCheckList; } }

        /// <summary>
        /// A list of onscreen tiles found
        /// </summary>
        public static List<ushort> TilesChecked { get { return WorldExplore.TilesChecked; } }

        public static int ItemIDExpeditionBook { get { return Expeditions.bookID; } }
        public static int ItemIDExpeditionBoard { get { return Expeditions.boardID; } }
        public static int ItemIDRustedBox { get { return Expeditions.stockBox1; } }
        public static int ItemIDRelicBox { get { return Expeditions.stockBox2; } }

        #endregion

        #region Methods
        /// <summary>
        /// Add your expedition to the list. Call this in your Mod's Load() method. 
        /// No Autoload support yet.
        /// </summary>
        /// <param name="mod">Your mod object</param>
        /// <param name="modExpedition">Your CustomExpedition, usually just 'new CustomExpedition()'</param>
        public static void AddExpedition(Mod mod, ModExpedition modExpedition)
        {
            Expeditions.AddExpeditionToList(modExpedition, mod);
        }

        /// <summary>
        /// Returns the list that manages the expeditions. Don't mess with it unless you know what you're doing... consider FindExpedition().
        /// </summary>
        /// <returns>The list of ModExpeditions. Will not return null, only an empty list.</returns>
        public static List<ModExpedition> GetExpeditionsList()
        {
            return Expeditions.GetExpeditionsList();
        }

        /// <summary>
        /// Attempts to find the specified mod expedition by class name. 
        /// </summary>
        /// <param name="mod">Your mod object</param>
        /// <param name="name">The classname of the ModExpedition</param>
        /// <returns>ModExpedition, or null if no result</returns>
        public static ModExpedition FindModExpedition(Mod mod, string name)
        {
            return Expeditions.FindModExpedition(mod, name);
        }
        /// <summary>
        /// Attempts to find the specified expedition by class name. 
        /// </summary>
        /// <param name="mod">Your mod object</param>
        /// <param name="name">The classname of the Expedition's ModExpedition</param>
        /// <returns>Expedition, or null if no result</returns>
        public static Expedition FindExpedition(Mod mod, string name)
        {
            return Expeditions.FindExpedition(mod, name);
        }
        
        /// <summary>
        /// Show the expedition as an item pickup.
        /// Automatically called when expeditions meet prerequisites, 
        /// and can also be used eg. daily quests.
        /// </summary>
        /// <param name="expedition"></param>
        public static void ShowExpeditionAsNewItem(Expedition expedition)
        {
            Expeditions.DisplayUnlockedExpedition(expedition);
        }

        /// <summary>
        /// Net-friendly method to spawn item on top of my player.
        /// An increased options version of player.QuickSpawnItem(). 
        /// Doesn't spawn item if netMode is 2 (server)
        /// </summary>
        /// <param name="itemType">ItemID item type</param>
        /// <param name="stack"></param>
        /// <param name="prefix"></param>
        public static void ClientSpawnItem(int itemType, int stack = 1, int prefix = 0)
        {
            Expeditions.ClientNetSpawnItem(itemType, stack, prefix);
        }
        /// <summary>
        /// Net-friendly method to spawn item on top of my player.
        /// An increased options version of player.QuickSpawnItem(). 
        /// Doesn't spawn item if netMode is 2 (server)
        /// </summary>
        /// <param name="item"></param>
        public static void ClientNetSpawnItem(Item item)
        {
            Expeditions.ClientNetSpawnItem(item);
        }
        /// <summary>
        /// Takes a value and divides it into stacks of coins.
        /// Doesn't spawn item if netMode is 2 (server)
        /// </summary>
        /// <param name="value">int[4] array of coin stacks from platinum[0] to copper[3]</param>
        /// <returns></returns>
        public static int[] DivideValueIntoMoneyStacks(int value)
        {
            return Expeditions.DivideValueIntoMoneyStack(value);
        }



        /// <summary>
        /// Counts the number of tiles onscreen matching tiles from TileCheckList
        /// </summary>
        /// <param name="tileID"></param>
        /// <returns></returns>
        public static int CountTilesInCheckedOnScreen(ushort tileID)
        {
            return WorldExplore.CountTilesInChecked(tileID);
        }
        #endregion
    }
}
