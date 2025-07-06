using System;
using System.Collections.Generic;
using System.Linq; // For OrderBy
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using ReLogic.Content;
using Expeditions144.Items;
using Terraria.ModLoader.Config;
using System.ComponentModel;
using Terraria.Audio;

namespace Expeditions144
{
	/// <summary>
	/// See the API class. Do not use unless you know precisely what you're doing.
	/// </summary>
	public class Expeditions144 : Mod
    {
        internal const bool DEBUG = false;
        // Use a boolean to check if the appropriate mod is loaded
        public bool LoadedFKTModSettings = false;

        internal UserInterface expeditionUserInterface;
        internal static ExpeditionUI expeditionUI;

        /*private*/ internal static UserInterface trackerInterface; // *** added "static"
        internal static TrackerUI trackerUI;

        internal static bool ShowTrackingText = true;

        /// <summary> REMINDER: INTERNAL ONLY USE GetExpeditionsList() FOR SAFETY. DO NOT CHANGE. </summary>
        private static List<ModExpedition> expeditionTemplateList;
        /// <summary> The list used by the player. Can be modified. </summary>
        private static List<ModExpedition> expeditionActiveList;

        internal static Dictionary<int, byte> checkedState;

        internal static Asset<Texture2D> sortingTexture;
        internal static Asset<Texture2D> bountyBoardTexture;

        internal static int bookID;
        internal static int boardID;
        internal static int voucherID;
        internal static int stockBox1;
        internal static int stockBox2;

        internal static int currencyVoucherID;

        public Expeditions144()
        {
			this.GoreAutoloadingEnabled = true;
            /***Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };*/
            // Reset list every time we reload
            expeditionTemplateList = new List<ModExpedition>();
            expeditionActiveList = new List<ModExpedition>();
        }

        public override void Load()
        {
            checkedState = new Dictionary<int, byte>();

            // Load textures
            if (Main.netMode != 2)
            {
                sortingTexture = this.Assets.Request<Texture2D>("UI/Sorting_Categories");
                bountyBoardTexture = this.Assets.Request< Texture2D >("Items/BountyBoard");
            }

            if (Main.netMode != 2)
            {
                expeditionUI = new ExpeditionUI();
                expeditionUI.Activate();
                expeditionUserInterface = new UserInterface();
                expeditionUserInterface.SetState(expeditionUI);

                trackerUI = new TrackerUI();
                trackerUI.Activate();
                trackerInterface = new UserInterface();
                trackerInterface.SetState(trackerUI);
            }

            bookID = ModContent.ItemType<BountyBook>();
            boardID = ModContent.ItemType<BountyBoard>();
            voucherID = ModContent.ItemType<BountyVoucher>();
            stockBox1 = ModContent.ItemType<StockBox>();
            stockBox2 = ModContent.ItemType<StockBox2>();

            // Register the voucher as a new currency
            CustomCurrencySingleCoin c = new CustomCurrencySingleCoin(voucherID, 999L);
            c.CurrencyTextColor = new Color(226, 51, 240); // Purple
            c.CurrencyTextKey = Items.BountyVoucher.itemName;
            currencyVoucherID = CustomCurrencyManager.RegisterCurrency(c);


            // Add test quests
            if (DEBUG)
            {
                AutoLoadExpeditions(this);
                //AddExpeditionToList(new ExampleExpedition(), this);
                //AddExpeditionToList(new HeaderTest(), this);
            }

            /***LoadedFKTModSettings = ModLoader.GetMod("FKTModSettings") != null;
            if (LoadedFKTModSettings)
            {
                // Needs to be in a method otherwise it throws a namespace error
                try { LoadModSettings(); }
                catch { }
            }*/
        }
        
        #region Autoload support

        /// <summary>
        /// Tries to autoload all expeditions where available.
        /// </summary>
        /// <param name="mod">Your mod, which is probably just 'this'</param>
        public static void AutoLoadExpeditions(Mod mod)
        {
            // Should be a publicly accessible readonly Assembly object
			if (mod.Code == null) return;
            
            // Shameless copy from tModLoader/patches/tModLoader/Terraria.ModLoader, internal void Autoload()
            // Appears to get all classes from the assembly ordered specificially irrelevant of culture (eg. turkish, chinese)
            foreach (Type type in mod.Code.GetTypes().OrderBy(type => type.FullName, StringComparer.InvariantCulture))
			{
				if (type.IsAbstract)
				{
					continue; // Obviously, ignore abstract types since they cannot be initialised on their own
				}
				if (type.IsSubclassOf(typeof(ModExpedition)))
				{
                    // We want to load classes extended from ModExpedition
					AutoloadExpedition(type, mod);
				}
            }
        }
        
        private static void AutoloadExpedition(Type type, Mod mod)
        {
            // Activator is a handy mscorlib class that:
            // "Creates an instance of the specified type using the constructor that best matches the specified parameters."
            // Basically, it calls a ModExpedition's ModExpedition() constructor. Can specify extra params since it uses
            // params object[], though not particularly relevant here.
            ModExpedition modExpedition = (ModExpedition)Activator.CreateInstance(type);
            if (modExpedition.AutoLoad())
            {
                AddExpeditionToList(modExpedition, mod);
            }
        }
        
        #endregion

        #region External Expedition Support

        /// <summary>
        /// Adds the designated expedition to the list of active expeditions.
        /// </summary>
        /// <param name="modExpedition">The expedition you are adding, just initialise as a new one.</param>
        /// <param name="mod">Your mod, which is probably just 'this'</param>
        public static void AddExpeditionToList(ModExpedition modExpedition, Mod mod)
        {
            // Make a new set
            if (expeditionTemplateList == null) expeditionTemplateList = new List<ModExpedition>();

            modExpedition.mod = mod;
            expeditionTemplateList.Add(modExpedition);
            if (Main.netMode == 2) Console.WriteLine("  > Adding Expedition: " + modExpedition.GetType().ToString());
        }
        
        /// <summary>
        /// Returns the expedition list
        /// </summary>
        /// <returns></returns>
        public static List<ModExpedition> GetExpeditionsList()
        {
            // Make a new set
            if (expeditionActiveList == null)
            {
                expeditionActiveList = new List<ModExpedition>();
                ResetExpeditions();
            }
            return expeditionActiveList.ToList<ModExpedition>(); // Prevent issues with modified enumeration?
        }
        /// <summary>
        /// Finds the specified mod expedition or an empty ModExpedition
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
            return new ModExpedition();
        }
        /// <summary>
        /// Finds the specified mod expedition or an empty ModExpedition
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

		#region ModSettings Support
		/****private void LoadModSettings()
        {
            FKTModSettings.ModSetting setting = 
                FKTModSettings.ModSettingsAPI.CreateModSettingConfig(this);
            setting.EnableAutoConfig();

            setting.AddBool("newQuestPopup", "Show New Expeditions", false);
            setting.AddBool("chatTrackEnabled", "Chat Tracker Enabled", false);
            setting.AddBool("trackerEnabled", "HUD Tracker Enabled", false);
            setting.AddByte("trackerAlphaByte", "HUD Transparency", 0, 255, false);
            setting.AddBool("trackerDescriptions", "HUD Descriptions", false);
            setting.AddFloat("trackerScale", "HUD Scale", 0.5f, 1f, false);
            setting.AddBool("autoShowEnabled", "Contextual HUD Enabled", false);
            setting.AddInt("autoShowHoldTime", "HUD Time (seconds/60)", 0, 300, false);
		}*/


		public static bool port_lastTrackerEnabled = false; //**** So you can toggle off the thing
		internal void UpdateModSettings() // *** changed from "private" to "internal"
        {
			/*FKTModSettings.ModSetting setting;
            if (FKTModSettings.ModSettingsAPI.TryGetModSetting(this, out setting))
            {
                setting.Get("newQuestPopup", ref enableUnlockDisplay);
                setting.Get("chatTrackEnabled", ref ShowTrackingText);
                setting.Get("trackerEnabled", ref TrackerUI.visible);
                setting.Get("trackerAlphaByte", ref TrackerUI.permaVisAlpha);
                setting.Get("trackerDescriptions", ref TrackerUI.showDescription);
                setting.Get("trackerScale", ref TrackerUI.textScale);
                setting.Get("autoShowEnabled", ref TrackerUI.allowUpdateVisible);
                setting.Get("autoShowHoldTime", ref TrackerUI.ChangeTickMax);
            }*/
			Expeditions144Config config = (Expeditions144Config)this.GetConfig("Expeditions144Config");

			enableUnlockDisplay = config.newQuestPopup;
			ShowTrackingText = config.chatTrackEnabled;
			if (port_lastTrackerEnabled != config.trackerEnabled)
				TrackerUI.visible = port_lastTrackerEnabled = config.trackerEnabled;
			TrackerUI.permaVisAlpha = config.trackerAlphaByte;
			TrackerUI.showDescription = config.trackerDescriptions;
			TrackerUI.textScale = config.trackerScale;
			TrackerUI.allowUpdateVisible = config.autoShowEnabled;
			TrackerUI.ChangeTickMax = config.autoShowHoldTime;
		}
        #endregion

        /// <summary> Reset progress and detach references </summary>
        internal static void ResetExpeditions()
        {
            if (Main.netMode == 2) Console.WriteLine("  > Resetting Expeditions");
            if (Expeditions144.DEBUG) Main.NewText("Expeditions: Resetting Expeditions");

            // reinitiliase all the expeditions
            expeditionActiveList = new List<ModExpedition>(expeditionTemplateList.Count);
            for(int i = 0; i < expeditionTemplateList.Count; i++)
            {
                ModExpedition mex = expeditionTemplateList[i];
                ModExpedition mexAct = mex.Clone();
                mexAct.SetDefaults();
                expeditionActiveList.Add(mexAct);
                PlayerExplorer.dbgmsg += "(" + mexAct.expedition.name
                    + (mexAct.expedition.trackingActive ? "T" : "n") + ")";
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
            foreach (ModExpedition mex in expeditionTemplateList)
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
            ResetExpeditions();

            Items.ItemRewardPool.GenerateRewardPool();
        }


        #region Netcode

        internal const int packetID_test = 0;
        internal const int packetID_partyComplete = 1;
        internal const int packetID_dailyExpedition = 2;
        internal const int packetID_dailyCheck = 3;
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.netMode == 0) return;

            //get my packet type
            int packetID = reader.ReadInt32();
            if (DEBUG)
            {
                if (Main.netMode == 2)
                { Console.WriteLine("Received a Packet with id " + packetID + " for " + whoAmI); }
                else
                { Main.NewText("Received a Packet with id " + packetID + " for " + whoAmI, 50, 50, 100); }
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
                        if (DEBUG) Console.WriteLine("Packet for team " + senderTeam + ", expi " + expIndex);
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
                    int dailyHashID = reader.ReadInt32();

                    if (DEBUG) Main.NewText("Expedition index received: " + dailyHashID);
                    Receive_NewDaily(dailyHashID);
                    break;
                case packetID_dailyCheck:
                    // Get variables
                    int playerRequester = reader.ReadInt32();

                    if (Main.netMode == 2)
                    {
                        if (DEBUG)
                            try
                            {
                                Console.WriteLine("Packet request from " + playerRequester + " to " + whoAmI
                                     + ", writing " + Expedition.GetHashID(WorldExplore.syncedDailyExpedition));
                            } catch (Exception e) { Console.WriteLine(e.ToString()); }
                        SendNet_NewDaily(this, playerRequester);
                    }
                    break;
                default:
                    break;
            }
        }

        internal void SendTestModPacket(int senderWhoAmI, int message)
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

        internal static void SendNet_NewDaily(Mod mod, int sendToPlayer = -1)
        {
            if(Main.netMode == 2)
            {
                if (WorldExplore.syncedDailyExpedition != null)
                {
                    Console.WriteLine("Sending daily expedition: " + Expedition.GetHashID(WorldExplore.syncedDailyExpedition));
                    // Generate a new packet
                    ModPacket packet = mod.GetPacket();
                    // Add the variables
                    packet.Write(packetID_dailyExpedition);
                    packet.Write(Expedition.GetHashID(WorldExplore.syncedDailyExpedition));
                    // Send to the clients
                    packet.Send(sendToPlayer);

                    Console.WriteLine("SENT INDEX IS " + Expedition.GetHashID(WorldExplore.syncedDailyExpedition));
                }
            }
        }
        private static void Receive_NewDaily(int hasID)
        {
            Expedition expedition = FindExpedition(hasID);
            if (expedition == null) return;
            try
            {
                if (DEBUG) Main.NewText("id matches " + expedition.name);
                WorldExplore.NetSyncDaily(expedition);
            }
            catch
            {
                if (DEBUG) Main.NewText("id matches nothing");
                WorldExplore.NetSyncDaily(null);
            }
        }

        // Client send request to daily quest on connect
        internal static void SendNet_GetDaily(Mod mod, int playerWhoAmI)
        {
            // Generate a new packet
            ModPacket packet = mod.GetPacket();
            // Add the variables
            packet.Write(packetID_dailyCheck);
            packet.Write(playerWhoAmI);
            // Send to the server
            packet.Send();
        }

        #endregion Netcode

        /// <summary>
        /// Opens the expediton menu
        /// </summary>
        /// <param name="viewMode"></param>
        public static void OpenExpeditionMenu(int viewMode)
        {
            if (Main.netMode == NetmodeID.Server) return;

            if (DEBUG) Main.NewText("OpenMethod UI : " + viewMode);
            Player player = Main.LocalPlayer;

            Main.playerInventory = false;
            player.sign = -1;
            Main.SetNPCShopIndex(0);
            Main.npcChatText = "";
            if (viewMode != ExpeditionUI.viewMode_NPC && 
                player.talkNPC > 0)
			{
				player.SetTalkNPC(0);

			}

			SoundEngine.PlaySound(SoundID.MenuOpen); //open menu
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

			if (!silent) SoundEngine.PlaySound(SoundID.MenuClose); //close menu
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
                Expeditions144.CloseExpeditionMenu();
            }
            else
            {
                Expeditions144.OpenExpeditionMenu(viewMode);
            }
        }

        internal static bool unlockedSoundFrame = false;
        internal static bool enableUnlockDisplay = true;

        /// <summary>
        /// Show the expedition as an item being "picked up". Called when an expedition meets
        /// the prerequisite for the first time.
        /// </summary>
        /// <param name="expedition"></param>
        public static void DisplayUnlockedExpedition(Expedition expedition, string customPrefix = "Expedition: ")
        {
            if (!enableUnlockDisplay) return;
            if (!API.InInventory[bookID]) return;

            Item exp = new Item();
            exp.SetNameOverride(customPrefix + expedition.name);
            exp.stack = 1;
            exp.active = true;
            exp.Center = Main.LocalPlayer.Center;
            exp.rare = expedition.difficulty;
            exp.expert = expedition.ctgImportant;

			//ItemText.NewText(exp, 1, true, true);
			Main.HoverItem = exp;


			if (!unlockedSoundFrame)
            {
				SoundEngine.PlaySound(SoundID.Chat, Main.LocalPlayer.Center);
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
            if (Main.netMode == NetmodeID.Server) return;

            int id = Item.NewItem(
				Main.LocalPlayer.GetSource_FromThis("Expeditions_ClientNetSpawnItem"),
				(int)Main.LocalPlayer.position.X,
                (int)Main.LocalPlayer.position.Y,
                Main.LocalPlayer.width,
                Main.LocalPlayer.height,
                itemType, stack, false, prefix, false, false);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(21, -1, -1, null, id, 1f);
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
                // if (DEBUG) Main.NewText("    value is at: " + value, 255, 255, 100);
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
            // if (DEBUG) Main.NewText("    value is now: " + value, 255, 255, 100);
            return stacks;
        }
        
    }


	public class Expeditions144System : ModSystem
	{
		// Moved from the main class
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			// **** changed to lambda operator function to get around nonsense involving use of local variables in local functions
			Func<bool> OtherInterfaceActive = () => (
				Main.playerInventory ||
				Main.LocalPlayer.chest != -1 ||
				Main.npcShop != 0 ||
				(
					Main.LocalPlayer.talkNPC > 0 &&
					ExpeditionUI.viewMode != ExpeditionUI.viewMode_NPC
				) ||
				Main.InReforgeMenu ||
				Main.InGuideCraftMenu ||
				Main.gameMenu);

			int HotBar = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
			if (HotBar != -1)
			{
				layers.Insert(HotBar, new LegacyGameInterfaceLayer(
					"ExpeditionsUITracker",
					delegate
					{
						if (TrackerUI.VisibleWithAlpha)
						{
							if (!OtherInterfaceActive())
							{
								Expeditions144.trackerInterface.Update(Main._drawInterfaceGameTime);
								Expeditions144.trackerUI.Draw(Main.spriteBatch);
							}
						}
						return true;
					}, InterfaceScaleType.UI) // **** added InterfaceScaleType.UI to make it not scale with zoom
				);
			}

			//All this stuff is jankyily adapted from ExampleMod
			//This is getting the mouse layer, and adding the UI just underneath it
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"ExpeditionsUIPanel",
					delegate
					{
						if (ExpeditionUI.visible)
						{
							if (OtherInterfaceActive())
							{
								//close this if other things are opened
								Expeditions144.CloseExpeditionMenu(true);
								if (Expeditions144.DEBUG) Main.NewText("Closing via obstruction");
							}
							else
							{
								//No idea what this does but the other one draws the UI
								((Expeditions144)Mod).expeditionUserInterface.Update(Main._drawInterfaceGameTime);
								Expeditions144.expeditionUI.Draw(Main.spriteBatch);
							}
						}
						return true;
					}, InterfaceScaleType.UI) // **** added InterfaceScaleType.UI to make it not scale with zoom
				);
			}
		}

		public override void PostUpdateInput()
		{
			/*if (LoadedFKTModSettings && !Main.gameMenu)
			{
				// Needs to be in a method otherwise it throws a namespace error
				try { UpdateModSettings(); }
				catch { }
			}*/
			Expeditions144 expMod = ((Expeditions144)this.Mod);
			expMod.UpdateModSettings();

			if (Main.netMode == 2) return;

			Player player = Main.LocalPlayer;
			// Keep track of active expeditions in-game
			if (!Main.gamePaused && !Main.gameMenu && Main.netMode != 2)
			{
				// RESET Expedition called values
				Expeditions144.unlockedSoundFrame = false;

				Expeditions144.checkedState.Clear();

				if (TrackerUI.recentChangeTick > 0) TrackerUI.recentChangeTick--;

				if (Main.time == 0.0)
				{
					foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
					{
						// Dawn of a new day
						if (Main.dayTime)
						{
							me.OnNewDay(player,
							  ref me.expedition.condition1Met,
							  ref me.expedition.condition2Met,
							  ref me.expedition.condition3Met,
							  me.expedition.conditionCounted >= me.expedition.conditionCountedMax
							  );
						}
						else
						{
							me.OnNewNight(player,
								ref me.expedition.condition1Met,
								ref me.expedition.condition2Met,
								ref me.expedition.condition3Met,
								me.expedition.conditionCounted >= me.expedition.conditionCountedMax
								);
						}

						// Check conditions as long as prerequisites are met
						if (me.expedition.PrerequisitesMet())
						{
							// As long as an expedition is not completed yet, or repeats, check this
							if (!me.expedition.completed || me.expedition.repeatable)
							{
								me.expedition.UpdateCountable();
								me.expedition.ConditionsMet();
								Expeditions144.checkedState.Add(me.expedition.GetHashID(), 1);
							}
							else
							{
								// Completed
								Expeditions144.checkedState.Add(me.expedition.GetHashID(), 2);
							}
						}
						else
						{
							// Prerequisite not met yet
							Expeditions144.checkedState.Add(me.expedition.GetHashID(), 0);
						}
					}
				}
				else
				{
					foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
					{
						// Check conditions as long as prerequisites are met
						if (me.expedition.PrerequisitesMet())
						{
							if (!me.expedition.completed || me.expedition.repeatable)
							{
								me.expedition.UpdateCountable();
								me.expedition.ConditionsMet();
								Expeditions144.checkedState.Add(me.expedition.GetHashID(), 1);
							}
							else
							{
								// Completed
								Expeditions144.checkedState.Add(me.expedition.GetHashID(), 2);
							}
						}
						else
						{
							// Prerequisite not met yet
							Expeditions144.checkedState.Add(me.expedition.GetHashID(), 0);
						}
					}
				}
			}

			#region Debug
			if (Expeditions144.DEBUG)
			{
				if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.L))
				{
					if (Main.time % 60 == 0)
					{
						Main.NewText(ExpeditionUI.visible + " : UIVisible mode? pre:" + ExpeditionUI.viewMode, 150, 200, 255);
						int[] stacks = Expeditions144.DivideValueIntoMoneyStack(1234567);
						Main.NewText("stacks: " +
							stacks[0] + "plat, " +
							stacks[1] + "gold, " +
							stacks[2] + "silv, " +
							stacks[3] + "copr, "
							);
						if (Main.netMode == 1) expMod.SendTestModPacket(Main.myPlayer, 1337);
					}
					/*
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
                    */
				}
				if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
				{
					if (Main.time % 60 == 0)
					{
						Main.NewText("Reset Progress");
						Expeditions144.ResetExpeditions();
					}
				}
			}
			#endregion
		}



	}

	public class Expeditions144Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(true)]
		public bool newQuestPopup;

		[DefaultValue(true)]
		public bool chatTrackEnabled;

		[DefaultValue(true)]
		public bool trackerEnabled;

		[Range(0, 255)]
		[DefaultValue(128)]
		public byte trackerAlphaByte;

		[DefaultValue(true)]
		public bool trackerDescriptions;

		[Range(0.5f, 1f)]
		[DefaultValue(1f)]
		public float trackerScale;

		[DefaultValue(true)]
		public bool autoShowEnabled;

		[Range(0, 300)]
		[DefaultValue(120)]
		public int autoShowHoldTime;
	}

}
