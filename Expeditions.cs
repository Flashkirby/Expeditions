using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.DataStructures;

using Expeditions.Quests;

namespace Expeditions
{
    /// <summary>
    /// See the API class. Do not use unless you know precisely what you're doing.
    /// </summary>
    public class Expeditions : Mod
    {
        internal const bool DEBUG = false;

        private UserInterface expeditionUserInterface;
        internal static ExpeditionUI expeditionUI;

        /// <summary> REMINDER: INTERNAL ONLY USE GetExpeditionsList() FOR SAFETY </summary>
        private static List<ModExpedition> expeditionList;

        public static Texture2D sortingTexture;
        public static Texture2D bountyBoardTexture;

        public static int bookID;
        public static int boardID;
        public static int voucherID;
        public static int stockBox1;
        public static int stockBox2;

        public static int currencyVoucherID;

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
            // Load textures
            if (Main.netMode != 2)
            {
                sortingTexture = GetTexture("UI/Sorting_Categories");
                bountyBoardTexture = GetTexture("Items/BountyBoard");
            }

            if (Main.netMode != 2)
            {
                expeditionUI = new ExpeditionUI();
                expeditionUI.Activate();
                expeditionUserInterface = new UserInterface();
                expeditionUserInterface.SetState(expeditionUI);
            }

            bookID = ItemType("BountyBook");
            boardID = ItemType("BountyBoard");
            voucherID = ItemType("BountyVoucher");
            stockBox1 = ItemType("StockBox");
            stockBox2 = ItemType("StockBox2");

            // Register the voucher as a new currency
            CustomCurrencySingleCoin c = new CustomCurrencySingleCoin(voucherID, 999L);
            c.CurrencyTextColor = new Color(226, 51, 240); // Purple
            c.CurrencyTextKey = Items.BountyVoucher.itemName;
            currencyVoucherID = CustomCurrencyManager.RegisterCurrency(c);


            // Add test quests
            if (DEBUG)
            {
                AddExpeditionToList(new ExampleExpedition(), this);
                AddExpeditionToList(new HeaderTest(), this);
            }
        }

        #region External Expedition Support

        /// <summary>
        /// Adds the designated expedition to the list of active expeditions.
        /// </summary>
        /// <param name="modExpedition">The expedition you are adding, just initialise as a new one.</param>
        /// <param name="mod">Your mod, which is probably just 'this'</param>
        public static void AddExpeditionToList(ModExpedition modExpedition, Mod mod)
        {
            modExpedition.mod = mod;
            GetExpeditionsList().Add(modExpedition);
            if (Main.netMode == 2) Console.WriteLine("  > Adding Expedition: " + modExpedition.GetType().ToString());
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
        /// Finds the specified mod expedition or null
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
        /// <summary>
        /// Finds the specified mod expedition or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static ModExpedition FindModExpedition<T>(Mod mod)
        {
            return FindModExpedition(mod, typeof(T).Name);
        }
        /// <summary>
        /// Finds the specified expedition or an empty one
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Expedition FindExpedition(Mod mod, string name)
        {
            ModExpedition e = FindModExpedition(mod, name);
            if (e == null) return new Expedition();
            return e.expedition;
        }
        /// <summary>
        /// Finds the specified expedition via hashID, or null
        /// </summary>
        /// <param name="hashID">Expedition.GetHashID</param>
        /// <returns></returns>
        public static Expedition FindExpedition(int hashID)
        {
            foreach (ModExpedition me in GetExpeditionsList())
            {
                if (Expedition.GetHashID(me.expedition) == hashID)
                {
                    return me.expedition;
                }
            }
            return null;
        }
        /// <summary>
        /// Finds the specified expedition or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static Expedition FindExpedition<T>(Mod mod)
        {
            return FindExpedition(mod, typeof(T).Name);
        }

        #endregion

        /// <summary> Reset progress and detach references </summary>
        internal static void ResetExpeditions()
        {
            if (Main.netMode == 2) Console.WriteLine("  > Resetting Expeditions");
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
            if (Main.netMode == 2) Console.WriteLine("  > WorldInit");
            foreach (ModExpedition mex in GetExpeditionsList())
            {
                mex.expedition.WorldInitialise();
            }
        }
        
        /// <summary>
        /// Set the defaults for each expedition
        /// </summary>
        public override void AddRecipes()
        {
            //initiliase expedition defaults, values reset in PlayerExplorer
            if (Main.netMode == 2) Console.WriteLine("  > Setting Defaults");
            foreach (ModExpedition mex in GetExpeditionsList())
            {
                mex.SetDefaults();
                if (!mex.expedition.ctgCollect &&
                    !mex.expedition.ctgExplore &&
                    !mex.expedition.ctgImportant &&
                    !mex.expedition.ctgSlay
                    )
                {
                    mex.expedition.ctgExplore = true;
                }
            }

            Items.ItemRewardPool.GenerateRewardPool();
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
            if (Main.netMode == 2) return;

            Player player = Main.player[Main.myPlayer];
            // Keep track of active expeditions in-game
            if (!Main.gamePaused && !Main.gameMenu && Main.netMode != 2)
            {
                // RESET Expedition called values
                unlockedSoundFrame = false;

                if (Main.time == 0.0)
                {
                    foreach (ModExpedition me in GetExpeditionsList())
                    {
                        // Dawn of a new day
                        if (Main.dayTime)
                        { me.OnNewDay(); }
                        else
                        { me.OnNewNight(); }
                        
                        // Check conditions as long as prerequisites are met
                        if (me.expedition.PrerequisitesMet())
                        {
                            // As long as an expedition is not completed yet, or repeats, check this
                            if (!me.expedition.completed || me.expedition.repeatable)
                            {
                                me.expedition.UpdateCountable();
                                me.expedition.ConditionsMet();
                            }
                        }
                    }
                }
                else
                {
                    foreach (ModExpedition me in GetExpeditionsList())
                    {
                        // Check conditions as long as prerequisites are met
                        if (me.expedition.PrerequisitesMet())
                        {
                            if (!me.expedition.completed || me.expedition.repeatable)
                            {
                                me.expedition.UpdateCountable();
                                me.expedition.ConditionsMet();
                            }
                        }
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
                        int[] stacks = DivideValueIntoMoneyStack(1234567);
                        Main.NewText("stacks: " +
                            stacks[0] + "plat, " +
                            stacks[1] + "gold, " +
                            stacks[2] + "silv, " +
                            stacks[3] + "copr, "
                            );
                        SendTestModPacket(Main.myPlayer, 1337);
                    }
                    if (Main.time % 60 == 20)
                    {

                        if (PlayerExplorer.svmsg != null)
                        {
                            Main.NewTextMultiline(PlayerExplorer.svmsg, false, Color.LightSeaGreen);
                        }
                    }
                    if (Main.time % 60 == 40)
                    {

                        if (PlayerExplorer.dbgmsg != null)
                        {
                            Main.NewTextMultiline(PlayerExplorer.dbgmsg, false, Color.LightSteelBlue);
                        }
                    }
                }
            }
        }

        #region Netcode

        public const int packetID_test = 0;
        public const int packetID_partyComplete = 1;
        public const int packetID_dailyExpedition = 2;
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            //get my packet type
            int packetID = reader.ReadUInt16();
            if (DEBUG)
            {
                if(Main.netMode == 2)
                {
                    Console.WriteLine("Received a Packet with id " + packetID + " for " + whoAmI);
                }else
                {
                    Main.NewText("Received a Packet with id " + packetID + " for " + whoAmI, 50, 50, 100);
                }
            }
            switch (packetID)
            {
                case packetID_test:
                    // get the sender and content
                    int senderAmI = reader.ReadInt32();
                    int message = reader.ReadInt32();
                    ReceiveTestModPacket(senderAmI, message);

                    // As the server, resdistribute to all clients
                    if (Main.netMode == 2)
                    {
                        if (DEBUG) Console.WriteLine("Sending packet to clients");
                        ModPacket packet = GetPacket();
                        packet.Write(packetID_test);
                        packet.Write(senderAmI);
                        packet.Write(message);
                        foreach (Player player in Main.player)
                        {
                            if (player.active) packet.Send(player.whoAmI);
                        }
                    }
                    break;
                case packetID_partyComplete:
                    // Get variables
                    int expIndex = reader.ReadInt32();
                    int senderTeam = reader.ReadInt32();

                    if (Main.netMode == 2)
                    {
                        if (DEBUG) Console.WriteLine("Packet for team " + senderTeam);
                        ModPacket packet = GetPacket();
                        packet.Write(packetID_partyComplete);
                        packet.Write(expIndex);
                        packet.Write(senderTeam);
                        foreach (Player player in Main.player)
                        {
                            if (player.active && player.team == senderTeam) packet.Send(player.whoAmI);
                        }
                    }
                    else
                    {
                        Receive_PartyComplete(expIndex);
                    }
                    break;
                case packetID_dailyExpedition:
                    // Get variables
                    int dailyIndex = reader.ReadInt32();

                    // If the server sent this, distribute it to all clients
                    if (Main.netMode == 2)
                    {
                        if (DEBUG) Console.WriteLine("Packet for new daily " + dailyIndex);
                        ModPacket packet = GetPacket();
                        packet.Write(packetID_partyComplete);
                        packet.Write(dailyIndex);
                        foreach (Player player in Main.player)
                        {
                            if (player.active) packet.Send(player.whoAmI);
                        }
                    }
                    // If a client receieved this, sync up
                    else
                    {
                        Receive_NewDaily(dailyIndex);
                    }
                    break;
                default:
                    break;
            }
        }

        private void SendTestModPacket(int senderWhoAmI, int message)
        {
            if (Main.netMode == 1)
            {
                if (DEBUG) Main.NewText("sending packet", 50, 50, 100);
                ModPacket packet = GetPacket();
                packet.Write(packetID_test);
                packet.Write(senderWhoAmI);
                packet.Write(message);
                packet.Send();
            }
            else
            {
                ReceiveTestModPacket(senderWhoAmI, message);
            }
        }
        private static void ReceiveTestModPacket(int senderWhoAmI, int message)
        {
            Main.NewText("This is a net test from " + Main.player[senderWhoAmI].name +  ", with message: " + message);
        }

        internal static void SendNet_PartyComplete(Mod mod, int team, ModExpedition expedition)
        {
            if (Main.netMode == 1 && team > 0)
            {
                int index = GetExpeditionsList().IndexOf(expedition);
                if (index >= 0)
                {
                    // Generate a new packet
                    ModPacket packet = mod.GetPacket();
                    // Add the variables
                    packet.Write(packetID_partyComplete);
                    packet.Write(index);
                    packet.Write(team);
                    // Send to the server
                    packet.Send();
                }
            }
            else
            {
                expedition.expedition.CompleteExpedition(true);
            }
        }
        private static void Receive_PartyComplete(int expeditionIndex)
        {
            Expedition expedition = GetExpeditionsList()[expeditionIndex].expedition;
            expedition.CompleteExpedition(true);
        }

        internal static void SendNet_NewDaily(Mod mod, Expedition expedition)
        {
            if(Main.netMode == 2)
            {
                int index = GetExpeditionsList().IndexOf(expedition.mex);
                if (index >= 0)
                {
                    // Generate a new packet
                    ModPacket packet = mod.GetPacket();
                    // Add the variables
                    packet.Write(packetID_dailyExpedition);
                    packet.Write(index);
                    // Send to the server
                    packet.Send();
                }
            }
        }
        private static void Receive_NewDaily(int index)
        {
            try
            { WorldExplore.NetSyncDaily(GetExpeditionsList()[index].expedition); }
            catch { }
        }

        #endregion Netcode

        /// <summary>
        /// Opens the expediton menu
        /// </summary>
        /// <param name="viewMode"></param>
        public static void OpenExpeditionMenu(int viewMode)
        {
            if (Main.netMode == 2) return;

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
            if (Main.netMode == 2) return;

            if (DEBUG) Main.NewText("CloseMethod UI");
            Main.npcChatText = "";

            if (!silent) Main.PlaySound(11, -1, -1, 1); //close menu
            ExpeditionUI.visible = false;
        }

        /// <summary>
        /// Toggle expedition menu visibility
        /// </summary>
        public static void ToggleExpeditionMenu(int viewMode)
        {
            if (Main.netMode == 2) return;

            if (ExpeditionUI.visible)
            {
                Expeditions.CloseExpeditionMenu();
            }
            else
            {
                Expeditions.OpenExpeditionMenu(viewMode);
            }
        }

        private static bool unlockedSoundFrame = false;
        /// <summary>
        /// Show the expedition as an item being "picked up". Called when an expedition meets
        /// the prerequisite for the first time.
        /// </summary>
        /// <param name="expedition"></param>
        public static void DisplayUnlockedExpedition(Expedition expedition, string customPrefix = "Expedition: ")
        {
            Item exp = new Item();
            exp.name = customPrefix + expedition.name;
            exp.stack = 1;
            exp.active = true;
            exp.Center = Main.player[Main.myPlayer].Center;
            exp.rare = expedition.difficulty;
            exp.expert = expedition.ctgImportant;

            ItemText.NewText(exp, 1, true, true);

            if (!unlockedSoundFrame)
            {
                Main.PlaySound(SoundID.Chat, Main.player[Main.myPlayer].Center);
                unlockedSoundFrame = true;
            }
        }
        /// <summary>
        /// Spawn an item from a client input for a player. An increaed options version of QuickSpawnItem()
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="stack"></param>
        public static void ClientNetSpawnItem(int itemType, int stack = 1, int prefix = 0)
        {
            if (Main.netMode == 2) return;

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
            int[] stacks = DivideValueIntoMoneyStack(value);
            if (stacks[0] > 0) ClientNetSpawnItem(74, stacks[0]);
            if (stacks[1] > 0) ClientNetSpawnItem(73, stacks[1]);
            if (stacks[2] > 0) ClientNetSpawnItem(72, stacks[2]);
            if (stacks[3] > 0) ClientNetSpawnItem(71, stacks[3]);
        }

        /// <summary>
        /// Takes a value and divides it into stacks of coins.
        /// </summary>
        /// <param name="value">int[4] array of coin stacks from plat to copper</param>
        /// <returns></returns>
        public static int[] DivideValueIntoMoneyStack(int value)
        {
            int[] stacks = new int[4];
            int denomination;
            int index;
            while (value > 0)
            {
                if (DEBUG) Main.NewText("    value is now: " + value, 255, 255, 100);
                if (value >= 1000000) //platinum
                {
                    index = 0;
                    denomination = 1000000;
                }
                else if (value >= 10000) //gold
                {
                    index = 1;
                    denomination = 10000;
                }
                else if (value >= 100) //silver
                {
                    index = 2;
                    denomination = 100;
                }
                else            //copper
                {
                    index = 3;
                    denomination = 1;
                }

                // Get rounded down stacks as value/denomination
                stacks[index] = value / denomination;
                // Reduce value by denomination * calculated stack
                value -= stacks[index] * denomination;
            }
            if (DEBUG) Main.NewText("    value is now: " + value, 255, 255, 100);
            return stacks;
        }
        
    }
}