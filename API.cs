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
        #region Expeditions.cs Fields
        /// <summary> The NPC that was last struck my the player this frame, or an empty NPC type 0 </summary>
        public static NPC LastHitNPC { get { return Expeditions.LastHitNPC; } }
        /// <summary> The NPC that was last killed by the player this frame, or an empty NPC type 0 </summary>
        public static NPC LastKilledNPC { get { return Expeditions.LastKilledNPC; } }
        /// <summary> The NPC type of the Clerk NPC </summary>
        public static int NPCIDClerk { get { return Expeditions.npcClerk; } }
        /// <summary> 
        /// Get the current tier of major expeditions completed.
        /// <para/> Tier 1: Reaching the caverns, roughly gold/plat armour.
        /// <para/> Tier 2: Defeated the eye of cthulu, 200+ life and demonite equips.
        /// <para/> Tier 3: Entered the dungeon after beating skeletron. Meteor and dungeon items.
        /// <para/> Tier 4: Faced (not defeated) Wall of Flesh. 400 life, Jungle, Necro and Molten.
        /// <para/> Tier 5: Obtained wings, usually done during altar smashing. 
        /// <para/> Tier 6: Faced (not defeated) a mechanical boss. Gives the player arena materials.
        /// <para/> Tier 7: Defeated all mechanical bosses. Hallowed gear for chlorophyte and life fruit.
        /// <para/> Tier 8: Defeated plantera, and entered the temple. Lime and yellow gear.
        /// <para/> Tier 9: Been through the new dungeon and found all skeleton types. Maybe also bonus bosses and events. 
        /// <para/> Tier 10: Faced (not defeated) the cultist.
        /// <para/> Tier 11: Defeated the lunar event. The end of vanilla Terraria's progression.
        /// </summary>
        public static int CurrentTierExpedition()
        {
            return Expeditions.GetCurrentExpeditionTier();
        }
        /// <summary> Check if the initial housing quest has been completed. </summary>
        public static bool CompletedInitialQuest()
        {
            return Expeditions.CompletedWelcomeQuest();
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
        #endregion
    }
}
