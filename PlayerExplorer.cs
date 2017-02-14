using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Expeditions
{
    public class PlayerExplorer : ModPlayer
    {
        public int[] tileOpened = new int[2];
        public static bool[] itemContains;

        #region Save/Load
        private static int _version = 3;
        internal static string svmsg;
        internal static string dbgmsg;

        /// <summary>
        /// Stores expedition progress data.
        /// When loaded, copies progress to the expeditions list.
        /// When saved, copies progress from the expeditions list.
        /// </summary>
        private List<ProgressData> _savedProgressList;
        /// <summary>
        /// Stores expeditions which were loaded with no match found.
        /// </summary>
        private List<ProgressData> _orphanData;

        // v9.0.1 bug calls this method in multiplayer
        public override void Initialize()
        {
            itemContains = new bool[Main.itemTexture.Length];
        }

        // Called on exiting world and on death
        public override TagCompound Save()
        {
            if (Expeditions.DEBUG) Main.NewText("Expeditions PE: Saving...");

            //the tag to save
            TagCompound tag = new TagCompound();

            // Get the current expedition list
            List<ModExpedition> expeditions = Expeditions.GetExpeditionsList();

            // If 'O-K-0' is held
            bool resetProgress =
                 Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.O) &&
                 Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.K) &&
                 Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D0);
            // If '#' is held
            bool discardUnknowns = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemQuotes);

            List<ProgressData> saveData = new List<ProgressData>();
            
            if (Expeditions.DEBUG) svmsg += "\n" + player.name + " : Save";
            if (!resetProgress && expeditions != null)
            {
                // Save expedition progress
                foreach (ModExpedition me in expeditions)
                {
                    saveData.Add(new ProgressData(me.expedition));
                }

                // Carry on unknown progress
                if (!discardUnknowns && _orphanData != null && _orphanData.Count > 0)
                {
                    foreach (ProgressData pd in _orphanData)
                    {
                        saveData.Add(pd);
                    }
                }

                try
                {
                    ConvertProgressToTag(tag, saveData);
                }catch(Exception e)
                {
                    ErrorLogger.Log("Expeditions: " + e.ToString());
                }
            }

            if (Main.gameMenu)
            {
                // If leaving the game, reset the expeditions list 
                // so it isn't copied when new players are created. 
                //I don't even know how this works...
                Expeditions.ResetExpeditions();
            }

            if (Expeditions.DEBUG) Main.NewText("Expeditions PE: Save complete, expeditions cleared");
            return tag;
        }

        private static void ConvertProgressToTag(TagCompound tag, List<ProgressData> saveData)
        {
            // Generate save data arrays for NBT
            IList<int> h = new List<int>();
            IList<byte> c = new List<byte>();
            IList<int> cc = new List<int>();

            // Add all values to arrays
            foreach (ProgressData pd in saveData)
            {
                if (Expeditions.DEBUG) svmsg += pd.completed ? ":" : ".";
                h.Add(pd.hash);
                c.Add(new BitsByte(
                    pd.completed,
                    pd.trackingActive,
                    pd.condition1Met,
                    pd.condition2Met,
                    pd.condition3Met
                    ));
                cc.Add(pd.conditionCounted);
            }
            tag.Add("ProgressData.hash", h);
            tag.Add("ProgressData.bools", c);
            tag.Add("ProgressData.condCount", cc);
        }

        // Called at player select screen
        public override void Load(TagCompound tag)
        {
            if (Expeditions.DEBUG) Main.NewText("Expeditions PE: Loading...");
            if (Expeditions.DEBUG) svmsg += "\n" + player.name + " : Load";

            List<ProgressData> progress = ConvertTagToProgress(tag);

            if (progress.Count > 0)
            {
                //Initialise the progress list for this
                _savedProgressList = new List<ProgressData>();
                _orphanData = new List<ProgressData>();

                // Create carbon copy of loaded expedition list
                for (int i = 0; i < Expeditions.GetExpeditionsList().Count; i++)
                {
                    _savedProgressList.Add(new ProgressData(0, false, false, false, false, false, 0));
                }

                // Find hash matches and add progress
                for (int i = 0; i < Expeditions.GetExpeditionsList().Count; i++)
                {
                    Expedition e = Expeditions.GetExpeditionsList()[i].expedition;
                    foreach (ProgressData pd in progress)
                    {
                        if(pd.hash == Expedition.GetHashID(e))
                        {
                            if (Expeditions.DEBUG) svmsg += e.completed ? ":" : ".";
                            _savedProgressList[i] = pd;

                            // Remove from the list after use
                            progress.Remove(pd);
                            break;
                        }
                    }
                }

                // Still remaining? Put them in the unknown pile
                if(progress.Count > 0)
                {
                    foreach(ProgressData pd in progress)
                    {
                        if (Expeditions.DEBUG) svmsg += pd.completed ? "`" : "`";
                        _orphanData.Add(pd);
                    }
                }
            }
            if (Expeditions.DEBUG) svmsg += " c:" + progress.Count;
            if (Expeditions.DEBUG) Main.NewText("Expeditions PE: Load complete, expeditions set");
        }

        private static List<ProgressData> ConvertTagToProgress(TagCompound tag)
        {
            List<ProgressData> progress = new List<ProgressData>();
            IList<int> h = tag.GetList<int>("ProgressData.hash");
            IList<byte> c = tag.GetList<byte>("ProgressData.bools");
            IList<int> cc = tag.GetList<int>("ProgressData.condCount");
            if (Expeditions.DEBUG) svmsg += " L:" + h.Count + "'" + c.Count + "'" + cc.Count;
            for (int i = 0; i < h.Count; i++)
            {
                BitsByte bb = c[i];
                progress.Add(new ProgressData(
                    h[i], bb[0], bb[1],
                    bb[2], bb[3], bb[4], cc[i]
                    ));
            }
            return progress;
        }
        
        internal void CopyLocalExpeditionsToMain()
        {
            if (Main.netMode != 2 && player.whoAmI == Main.myPlayer)
            {
                if (Expeditions.DEBUG) { dbgmsg += "\n" + player.name + " set Expeditions"; }
                Expeditions.ResetExpeditions();
                if (_savedProgressList != null)
                {
                    if (Expeditions.DEBUG) Main.NewText("Expeditions: Loading progress to list, counted " + _savedProgressList.Count);
                    // Set the expeditions to use this list
                    for (int i = 0; i < _savedProgressList.Count; i++)
                    {
                        //Expeditions.GetExpeditionsList()[i].expedition.CopyProgress(
                        //    _localExpeditionList[i]);
                        Expeditions.GetExpeditionsList()[i].expedition.CopyProgress(
                            _savedProgressList[i].ToExpedition());

                        dbgmsg += "(" + Expeditions.GetExpeditionsList()[i].expedition.name
                            + (_savedProgressList[i].trackingActive ? "T-" : "n-")
                            + (Expeditions.GetExpeditionsList()[i].expedition.trackingActive ? ">T" : ">n") + ")";
                    }
                }
                else
                {
                    if (Expeditions.DEBUG) Main.NewText("Expeditions: New player - null list");
                }
            }
        }

        public override void OnEnterWorld(Player player)
        {
            if (Expeditions.DEBUG) Main.NewText("Expeditions: Enter World");
            if (Main.netMode != 2)
            {
                // Set main list to loaded
                CopyLocalExpeditionsToMain();
            }

            if(Main.netMode == 1)
            {
                RequestDailyQuest();
            }

            // Reset list items
            Expeditions.WorldInit();
        }

        internal void RequestDailyQuest()
        {
            Expeditions.SendNet_GetDaily(mod, player.whoAmI);
        }

        #endregion

        public override void ResetEffects()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // Reset item contains
                itemContains = new bool[Main.itemTexture.Length];
                foreach (Item item in player.inventory)
                {
                    if (item == null) continue;
                    itemContains[item.type] = true;
                }
                foreach (Item item in player.armor)
                {
                    if (item == null) continue;
                    itemContains[item.type] = true;
                }
                foreach (Item item in player.miscEquips)
                {
                    if (item == null) continue;
                    itemContains[item.type] = true;
                }
            }
        }

        public override void PostUpdate()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (ExpeditionUI.visible && ExpeditionUI.viewMode == ExpeditionUI.viewMode_Tile)
                {
                    Rectangle tileRange = new Rectangle(
                        (int)(player.Center.X - (float)(Player.tileRangeX * 16)),
                        (int)(player.Center.Y - (float)(Player.tileRangeY * 16)),
                        Player.tileRangeX * 16 * 2,
                        Player.tileRangeY * 16 * 2);
                    Rectangle boardRect = new Rectangle(
                        tileOpened[0] * 16,
                        tileOpened[1] * 16,
                        4 * 16,
                        3 * 16
                        );
                    if (Expeditions.DEBUG) Dust.NewDust(boardRect.TopLeft(), boardRect.Width, boardRect.Height, 175);
                    if (!tileRange.Intersects(boardRect))
                    {
                        Expeditions.CloseExpeditionMenu();
                    }
                }
            }
            /*
            if (player.controlDown)
            {
                Main.NewText(player.HeldItem.name + " is rarity: " + player.HeldItem.rare);
            }
            */
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (player.whoAmI != Main.myPlayer) return;
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                me.OnCombatWithNPC(npc, true, Main.LocalPlayer,
                              ref me.expedition.condition1Met,
                              ref me.expedition.condition2Met,
                              ref me.expedition.condition3Met,
                              me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                              );
            }
        }
    }

    /// <summary>
    /// Trimmed down expedition class that only stores important data. To be refactored eventually.
    /// </summary>
    class ProgressData
    {
        public int hash;
        public bool completed;
        public bool trackingActive;
        public bool condition1Met;
        public bool condition2Met;
        public bool condition3Met;
        public int conditionCounted;

        public ProgressData(int hash, bool completed, bool tracking, bool cond1, bool cond2, bool cond3, int condC)
        {
            this.hash = hash;
            this.completed = completed;
            this.trackingActive = tracking;
            this.condition1Met = cond1;
            this.condition2Met = cond2;
            this.condition3Met = cond3;
            this.conditionCounted = condC;
        }

        public ProgressData(Expedition expedition)
        {
            this.hash = Expedition.GetHashID(expedition);
            this.completed = expedition.completed;
            this.trackingActive = expedition.trackingActive;
            this.condition1Met = expedition.condition1Met;
            this.condition2Met = expedition.condition2Met;
            this.condition3Met = expedition.condition3Met;
            this.conditionCounted = expedition.conditionCounted;
        }

        /// <summary>
        /// Create an empty expedition with the set progress
        /// </summary>
        /// <returns></returns>
        public Expedition ToExpedition()
        {
            Expedition e = new Expedition();
            e.name = "" + hash;
            e.completed = completed;
            e.trackingActive = trackingActive;
            e.condition1Met = condition1Met;
            e.condition2Met = condition2Met;
            e.condition3Met = condition3Met;
            e.conditionCounted = conditionCounted;
            return e;
        }
    }
}
