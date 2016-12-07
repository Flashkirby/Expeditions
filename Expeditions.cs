using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using Expeditions.Quests;

namespace Expeditions
{
    public class Expeditions : Mod
    {
        internal const bool DEBUG = true;

        private UserInterface expeditionUserInterface;
        internal static ExpeditionUI expeditionUI;

        /// <summary> REMINDER: INTERNAL ONLY USE GetExpeditionsList() FOR SAFETY </summary>
        private static List<ModExpedition> expeditionList;
        public static Texture2D sortingTexture;

        private static int _npcClerk;
        public static int npcClerk { get { return _npcClerk; } }

        private static ModExpedition tier1ExpPointer;
        private static ModExpedition tier2ExpPointer;
        private static ModExpedition tier3ExpPointer;
        private static ModExpedition tier4ExpPointer;
        private static ModExpedition tier5ExpPointer;
        private static ModExpedition tier6ExpPointer;
        private static ModExpedition tier7ExpPointer;
        private static ModExpedition tier8ExpPointer;
        private static ModExpedition tier9ExpPointer;
        private static ModExpedition tier10ExpPointer;
        private static ModExpedition tier11ExpPointer;

        public Expeditions()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
            // Reset list every time we reload
            expeditionList = new List<ModExpedition>();
        }

        public override void Load()
        {
            if (Main.netMode != 2)
            {
                sortingTexture = GetTexture("UI/Sorting_Categories");
            }
            _npcClerk = NPCType("Clerk");

            expeditionUI = new ExpeditionUI();
            expeditionUI.Activate();
            expeditionUserInterface = new UserInterface();
            expeditionUserInterface.SetState(expeditionUI);

            tier1ExpPointer = new Tier1Quest();
            tier2ExpPointer = new Tier2Quest();

            //add quests
            AddExpeditionToList(new ExampleExpedition(), this);
            AddExpeditionToList(tier1ExpPointer, this);
            AddExpeditionToList(tier2ExpPointer, this);
        }
        /// <summary>
        /// Adds the designated expedition to the list of active expeditions.
        /// </summary>
        /// <param name="modExpedition">The expedition you are adding, just initialise as a new one.</param>
        /// <param name="mod">Your mod, which is probably just 'this'</param>
        public static void AddExpeditionToList(ModExpedition modExpedition, Mod mod)
        {
            modExpedition.mod = mod;
            GetExpeditionsList().Add(modExpedition);
            // TODO: delet
            //GetExpeditionsList().Add(modExpedition);
            //GetExpeditionsList().Add(modExpedition);
        }
        
        /// <summary>
        /// Returns the expedition list
        /// </summary>
        /// <returns></returns>
        public static List<ModExpedition> GetExpeditionsList()
        {
            if (expeditionList == null) expeditionList = new List<ModExpedition>();
            return expeditionList;
        }
        /// <summary>
        /// Finds the specified expedition
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ModExpedition FindModExpedition(Mod mod, string name)
        {
            foreach(ModExpedition me in GetExpeditionsList())
            {
                if(me.mod.Equals(mod) && me.GetType().Name.Equals(name))
                {
                    return me;
                }
            }
            return null;
        }
        public static Expedition FindExpedition(Mod mod, string name)
        {
            ModExpedition e = FindModExpedition(mod, name);
            if (e == null) return null;
            return e.expedition;
        }
        /// <summary> Reset progress and detach references </summary>
        internal static void ResetExpeditions()
        {
            //initiliase all the expeditions
            foreach (ModExpedition mex in GetExpeditionsList())
            {
                mex.expedition.ResetProgress();
                PlayerExplorer.dbgmsg += "(" + mex.expedition.name
                    + (mex.expedition.trackingActive ? "T" : "n") + ")";
            }
        }
        internal static void WorldInit()
        {
            foreach (ModExpedition mex in GetExpeditionsList())
            {
                mex.expedition.WorldInitialise();
            }
        }
        
        public override void AddRecipes()
        {
            //initiliase expedition defaults, values reset in PlayerExplorer
            foreach (ModExpedition mex in GetExpeditionsList())
            {
                mex.SetDefaults();
            }
        }

        public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
        {
            //All this stuff is jankyily adapted from ExampleMod
            //This is getting the mouse layer, and adding the UI just underneath it
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                layers.Insert(MouseTextIndex, new MethodSequenceListItem(
                    "ExpeditionsUIPanel",
                    delegate
                    {
                        if (ExpeditionUI.visible)
                        {
                            Player p = Main.player[Main.myPlayer];
                            if (Main.playerInventory ||
                                Main.player[Main.myPlayer].chest != -1 ||
                                Main.npcShop != 0 ||
                                (
                                    p.talkNPC > 0 &&
                                    ExpeditionUI.viewMode != ExpeditionUI.viewMode_NPC
                                ) ||
                                Main.InReforgeMenu ||
                                Main.InGuideCraftMenu ||
                                Main.gameMenu
                                )
                            {
                                //close this if other things are opened
                                CloseExpeditionMenu(true);
                                if (DEBUG) Main.NewText("Closing via obstruction");
                            }
                            else
                            {
                                //No idea what this does but the other one draws the UI
                                expeditionUserInterface.Update(Main._drawInterfaceGameTime);
                                expeditionUI.Draw(Main.spriteBatch);
                            }
                        }
                        return true;
                    },
                    null)
                );
            }

        }

        public override void PostUpdateInput()
        {
            Player player = Main.player[Main.myPlayer];
            // Keep track of active expeditions in-game
            if (!Main.gamePaused && !Main.gameMenu && Main.netMode != 2)
            {
                foreach (ModExpedition me in GetExpeditionsList())
                {
                    if (me.CheckPrerequisites(player))
                    {
                        me.expedition.ConditionsMet();
                    }
                }
            }
                    

            //DEBUG INFO
            if (DEBUG)
            {
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.L))
                {
                    if (Main.time % 60 == 0)
                    {
                        Main.NewText(ExpeditionUI.visible + " : UIVisible mode? pre:" + ExpeditionUI.viewMode, 150, 200, 255);
                        Main.NewText(WorldExplore.savedClerk + " : savedClerk?");
                        Main.NewText(unlockedTier1Quests + " : tier 1 expeditions");
                    }
                    if (Main.time % 60 == 30)
                    {

                        if (PlayerExplorer.dbgmsg != null)
                        {
                            Main.NewTextMultiline(PlayerExplorer.dbgmsg);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Opens the expediton menu
        /// </summary>
        /// <param name="viewMode"></param>
        public static void OpenExpeditionMenu(ushort viewMode)
        {
            if (DEBUG) Main.NewText("OpenMethod UI : " + viewMode);
            Player player = Main.player[Main.myPlayer];
            
            Main.playerInventory = false;
            player.sign = -1;
            Main.npcShop = 0;
            Main.npcChatText = "";
            if (viewMode != ExpeditionUI.viewMode_NPC && 
                player.talkNPC > 0)
            {
                player.talkNPC = 0;
            }

            Main.PlaySound(10, -1, -1, 1); //open menu
            expeditionUI.ListRecalculate();
            ExpeditionUI.visible = true;
            ExpeditionUI.viewMode = viewMode;
        }

        /// <summary>
        /// Close the expedetion menu. Also called by sign and npc.
        /// </summary>
        public static void CloseExpeditionMenu(bool silent = false)
        {
            if (DEBUG) Main.NewText("CloseMethod UI");
            Main.npcChatText = "";

            if (!silent) Main.PlaySound(11, -1, -1, 1); //close menu
            ExpeditionUI.visible = false;
        }

        /// <summary>
        /// Toggle expedition menu visibility
        /// </summary>
        public static void ToggleExpeditionMenu(ushort viewMode)
        {
            if (ExpeditionUI.visible)
            {
                Expeditions.CloseExpeditionMenu();
            }
            else
            {
                Expeditions.OpenExpeditionMenu(viewMode);
            }
        }

        /// <summary>
        /// Spawn an item from a client input for a player. An increaed options version of QuickSpawnItem()
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="stack"></param>
        public static void ClientNetSpawnItem(int itemType, int stack = 1, int prefix = 0)
        {
            int id = Item.NewItem(
                (int)Main.player[Main.myPlayer].position.X,
                (int)Main.player[Main.myPlayer].position.Y,
                Main.player[Main.myPlayer].width,
                Main.player[Main.myPlayer].height,
                itemType, stack, false, prefix, false, false);
            if (Main.netMode == 1)
            {
                NetMessage.SendData(21, -1, -1, "", id, 1f);
            }
        }
        /// <summary>
        /// Spawn an item from a client input for a player
        /// </summary>
        /// <param name="item"></param>
        public static void ClientNetSpawnItem(Item item)
        {
            ClientNetSpawnItem(item.type, item.stack, item.prefix);
        }
        /// <summary>
        /// Spawn money on the player equal to the amount specified
        /// </summary>
        /// <param name="value"></param>
        public static void ClientNetSpawnItemMoney(int value)
        {
            int denomination;
            int coinType;
            int stack;
            while (value > 0)
            {
                if (value >= 1000000) //platinum
                {
                    denomination = 1000000;
                    coinType = 74;
                }
                if (value >= 10000) //gold
                {
                    denomination = 10000;
                    coinType = 73;
                }
                if (value >= 100) //silver
                {
                    denomination = 100;
                    coinType = 72;
                }
                else
                {
                    denomination = 1;
                    coinType = 71;
                }

                stack = value / denomination;
                value -= stack * denomination;

                ClientNetSpawnItem(coinType, stack);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentExpeditionTier()
        {
            if (tier11ExpPointer.expedition.completed) return 11;
            if (tier10ExpPointer.expedition.completed) return 10;
            if (tier9ExpPointer.expedition.completed) return 9;
            if (tier8ExpPointer.expedition.completed) return 8;
            if (tier7ExpPointer.expedition.completed) return 7;
            if (tier6ExpPointer.expedition.completed) return 6;
            if (tier5ExpPointer.expedition.completed) return 5;
            if (tier4ExpPointer.expedition.completed) return 4;
            if (tier3ExpPointer.expedition.completed) return 3;
            if (tier2ExpPointer.expedition.completed) return 2;
            if (tier1ExpPointer.expedition.completed) return 1;
            return 0;
        }
    }
}