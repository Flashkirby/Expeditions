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
    class Expeditions : Mod
    {
        internal const bool DEBUG = true;

        private UserInterface expeditionUserInterface;
        internal static ExpeditionUI expeditionUI;

        /// <summary> REMINDER: INTERNAL ONLY USE GetExpeditionsList() FOR SAFETY </summary>
        private static List<ModExpedition> _expeditionList;
        internal static List<ModExpedition> expeditionList
        {
            get
            {
                if (_expeditionList == null) _expeditionList = new List<ModExpedition>();
                return _expeditionList;
            }
            set { _expeditionList = value; }
        }
        public static Texture2D sortingTexture;

        private static int _npcClerk;
        public static int npcClerk { get { return _npcClerk; } }

        public Expeditions()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void Load()
        {
            sortingTexture = GetTexture("UI/Sorting_Categories");
            _npcClerk = NPCType("Clerk");

            expeditionUI = new ExpeditionUI();
            expeditionUI.Activate();
            expeditionUserInterface = new UserInterface();
            expeditionUserInterface.SetState(expeditionUI);

            //add quests
            AddExpeditionToList(new WelcomeQuest(), this);
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
        /// Returns a copy of the current expedition list. 
        /// </summary>
        /// <returns></returns>
        public static List<ModExpedition> GetExpeditionsListCopy()
        {
            List<ModExpedition> list = new List<ModExpedition>();
            list.AddRange(GetExpeditionsList());
            return list;
        }

        public override void AddRecipes()
        {
            //initiliase expedition defaults, values reset in PlayerExplorer
            foreach (ModExpedition mex in expeditionList)
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
                            if (Main.playerInventory ||
                                Main.player[Main.myPlayer].chest != -1 ||
                                Main.npcShop != 0 ||
                                Main.InReforgeMenu ||
                                Main.InGuideCraftMenu ||
                                Main.gameMenu
                                )
                            {
                                //close this if other things are opened
                                CloseExpeditionMenu();
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
                foreach (ModExpedition me in expeditionList)
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
                    }
                    if (Main.time % 60 == 30)
                    {

                        if (PlayerExplorer.message != null)
                        {
                            Main.NewTextMultiline(PlayerExplorer.message);
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
            if (viewMode != ExpeditionUI.viewMode_NPC && player.talkNPC > 0)
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
        public static void CloseExpeditionMenu()
        {
            if (DEBUG) Main.NewText("CloseMethod UI");
            Main.npcChatText = "";

            Main.PlaySound(11, -1, -1, 1); //close menu
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
    }
}