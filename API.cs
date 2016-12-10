using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public static class API
    {
        #region Expeditions.cs Fields
        /// <summary> The NPC that was last struck my the player. </summary>
        public static NPC LastHitNPC { get { return Expeditions.LastHitNPC; } }
        /// <summary> The NPC that was last killed by the player. </summary>
        public static NPC LastKilledNPC { get { return Expeditions.LastKilledNPC; } }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int CurrentTierExpedition()
        {
            return Expeditions.GetCurrentExpeditionTier();
        }
        #endregion
        #region Expeditions.cs Methods
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
        /// Net-friendly method to spawn item on top of my player.
        /// An increased options version of player.QuickSpawnItem(). 
        /// Doesn't spawn item if netMode is 2 (server)
        /// </summary>
        /// <param name="itemType">ItemID item type</param>
        /// <param name="stack"></param>
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
        #endregion
    }
}
