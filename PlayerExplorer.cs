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


        #region Save/Load
        private static int _version = 2;
        internal static string svmsg;
        internal static string dbgmsg;

        /// <summary>
        /// Stores expedition progress data.
        /// </summary>
        private List<ProgressData> _localExpeditionList;
        /// <summary>
        /// Stores expeditions which were loaded with no match found.
        /// </summary>
        private List<ProgressData> _orphanData;

        public override TagCompound Save()
        {
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

        public override void Load(TagCompound tag)
        {
            if (Expeditions.DEBUG) svmsg += "\n" + player.name + " : Load";

            List<ProgressData> progress = ConvertTagToProgress(tag);

            _localExpeditionList = new List<ProgressData>();
            _orphanData = new List<ProgressData>();

            if (progress != null)
            {
                // Create carbon copy of loaded expedition list
                for (int i = 0; i < Expeditions.GetExpeditionsList().Count; i++)
                {
                    _localExpeditionList.Add(new ProgressData(0, false, false, false, false, false, 0));
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
                            _localExpeditionList[i] = pd;

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

        public override void LoadLegacy(BinaryReader reader)
        {
            _version = reader.ReadInt32();
            if (_version == _versionCurrent)
            {
                //!/ if (Expeditions.DEBUG) dbgmsg += "\nLOAD v:" + _version + "/" + _versionCurrent;

                // Create a new progress storage
                //_localExpeditionList = new List<Expedition>();
                _localExpeditionList = new List<ProgressData>();

                // Create a dictionary of keys -> index
                Dictionary<int, int> hashToIndex = new Dictionary<int, int>();
                for (int i = 0; i < Expeditions.GetExpeditionsList().Count; i++)
                {
                    Expedition e = Expeditions.GetExpeditionsList()[i].expedition;
                    // Populate both lists
                    try
                    {
                        hashToIndex.Add(
                            Expedition.GetHashID(e), // The hashcode
                            i               // The index
                            );
                        //_localExpeditionList.Add(new Expedition());
                        _localExpeditionList.Add(new ProgressData(0, false, false, false, false, false, 0));
                    }
                    catch
                    {
                        string errorList = "";
                        foreach (ModExpedition me in Expeditions.GetExpeditionsList())
                        {
                            errorList += "\n" + Expedition.GetHashID(me.expedition).ToString("X") + " -> " + me.expedition.name + " (" + (object)(me).GetType().Name + ")";
                        }
                        throw new System.Exception("{!CONFLICT: " + e.name + ":#" + Expedition.GetHashID(e).ToString("X") + " duplicate!}" + errorList);
                        //dbgmsg += "{!CONFLICT: " + e.name + ":#" + e.GetHashID().ToString("X") + " duplicate!}";
                    }
                }

                // Read expeditions
                int count = reader.ReadInt32();
                int unknownCount = reader.ReadInt32();
                //!/ if (Expeditions.DEBUG) dbgmsg += "|c:" + count + "?" + unknownCount;

                // Iterating
                for (int i = 0; i < count + unknownCount; i++)
                {
                    int expeditionID = reader.ReadInt32();
                    BitsByte flags = reader.ReadByte();
                    ushort countedCond = reader.ReadUInt16();
                    int index;
                    if (hashToIndex.TryGetValue(expeditionID, out index))
                    {
                        // Write match found at index
                        //Expedition e = _localExpeditionList[index];
                        ProgressData e = _localExpeditionList[index];
                        e.completed = flags[0];
                        e.trackingActive = flags[1];
                        e.condition1Met = flags[2];
                        e.condition2Met = flags[3];
                        e.condition3Met = flags[4];
                        e.conditionCounted = countedCond;
                        //!/ if (Expeditions.DEBUG) dbgmsg += " [" + expeditionID.ToString("X") + ";" + (e.completed ? "`" : ".") + (e.trackingActive ? "`" : ".") + "]";
                    }
                    else
                    {
                        // Make a new one if not initialised
                        //if (_unusedExpeditionData == null) _unusedExpeditionData = new List<Expedition>();
                        if (_orphanData == null) _orphanData = new List<ProgressData>();
                        // No match found
                        //Expedition e = new Expedition();
                        ProgressData e = new ProgressData(0, false, false, false, false, false, 0);
                        e.completed = flags[0];
                        e.trackingActive = flags[1];
                        e.condition1Met = flags[2];
                        e.condition2Met = flags[3];
                        e.condition3Met = flags[4];
                        e.conditionCounted = countedCond;
                        //e.name = "" + expeditionID;
                        e.hash = expeditionID;
                        //_unusedExpeditionData.Add(e);
                        _orphanData.Add(e);
                        //!/ if (Expeditions.DEBUG) dbgmsg += " [" + expeditionID.ToString("X") + ";-]";
                    }
                }
            }
            else
            {
                if (_version < 0) return;
                if (Expeditions.DEBUG)
                {
                    dbgmsg += "(( LOAD v: " + _version + " / " + _versionCurrent + ")) ";
                }
                else
                {
                    ErrorLogger.Log("Expeditions: Player save file v" + _version + " is not a valid version number, somehow (You must've done goofed).");
                }
            }
        }

        public void CopyLocalExpeditionsToMain()
        {
            if (Main.netMode != 2 && player.whoAmI == Main.myPlayer)
            {
                if (Expeditions.DEBUG) { dbgmsg += "\n" + player.name + " set Expeditions"; }
                Expeditions.ResetExpeditions();
                if (_localExpeditionList != null)
                {
                    // Set the expeditions to use this list
                    for (int i = 0; i < _localExpeditionList.Count; i++)
                    {
                        //Expeditions.GetExpeditionsList()[i].expedition.CopyProgress(
                        //    _localExpeditionList[i]);
                        Expeditions.GetExpeditionsList()[i].expedition.CopyProgress(
                            _localExpeditionList[i].ToExpedition());

                        dbgmsg += "(" + Expeditions.GetExpeditionsList()[i].expedition.name
                            + (_localExpeditionList[i].trackingActive ? "T-" : "n-")
                            + (Expeditions.GetExpeditionsList()[i].expedition.trackingActive ? ">T" : ">n") + ")";
                    }
                }
            }
        }

        #endregion

        public bool familiarMinion;
        public override void ResetEffects()
        {
            familiarMinion = false;
        }

        public override void PostUpdate()
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

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            HitNPC(target);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!proj.npcProj) HitNPC(target);
        }

        private void HitNPC(NPC target)
        {
            // Only record NPCs the client hit
            if (player.whoAmI != Main.myPlayer) return;

            if (Expeditions.DEBUG && Expeditions.lastHitNPC != target) Main.NewText("EXP Hit: " + target.name, 119, 119, 255);
            Expeditions.lastHitNPC = target;

            // Record NPCs killed
            if (target.life <= 0 || !target.active)
            {
                if (Expeditions.DEBUG) Main.NewText("EXP Kill: " + target.name, 255, 119, 119);
                Expeditions.lastKilledNPC = target;
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
