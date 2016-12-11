using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    class PlayerExplorer : ModPlayer
    {
        private static int _version = _versionCurrent;
        private const int _versionCurrent = 1;
        public static string svmsg;
        public static string dbgmsg;

        public int[] tileOpened = new int[2];

        /// <summary>
        /// Stores "empty" expeditions that serve to store only progress information
        /// </summary>
        private List<Expedition> _localExpeditionList;
        /// <summary>
        /// Stores expeditions which were loaded with no match found.
        /// </summary>
        private List<Expedition> _unusedExpeditionData;

        public override void SaveCustomData(BinaryWriter writer)
        {
            writer.Write(_versionCurrent);
            int byteCount = 4;
            if (Expeditions.DEBUG) { svmsg = "\nSAVE v: " + _versionCurrent; dbgmsg = ""; }

            // Expeditions
            List<ModExpedition> expeditions = Expeditions.GetExpeditionsList();

            // Save the counts
            int count = expeditions.Count;
            int unknownCount = 0;
            
            // If 'O-K-0' is held
            bool resetProgress =
                 Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.O) &&
                 Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.K) &&
                 Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D0);
            // If '#' is held
            bool discardUnknowns = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemQuotes);

            if (_unusedExpeditionData != null) unknownCount = _unusedExpeditionData.Count;
            if (Expeditions.DEBUG) svmsg += "| c:" + count + "?" + unknownCount;

            // Apply resets where applicable
            if (resetProgress) { count = 0; unknownCount = 0; }
            if (discardUnknowns) unknownCount = 0;

            writer.Write(count); byteCount += 4;
            writer.Write(unknownCount); byteCount += 4;



            for (int i = 0; i < count; i++)
            {
                // Save the ID as 4 bytes, and its booleans as another byte
                if (Expeditions.DEBUG) svmsg += "  [" + expeditions[i].expedition.name + " : " + Expedition.GetHashID(expeditions[i].expedition).ToString("X") + " : " + expeditions[i].expedition.completed + " & " + expeditions[i].expedition.trackingActive + "] ";
                writer.Write(Expedition.GetHashID(expeditions[i].expedition)); byteCount += 4;
                writer.Write(new BitsByte(
                    expeditions[i].expedition.completed,
                    expeditions[i].expedition.trackingActive,
                    expeditions[i].expedition.condition1Met,
                    expeditions[i].expedition.condition2Met,
                    expeditions[i].expedition.condition3Met
                    )); byteCount += 1;
            }


            // Attempt to keep unknown data
            if (unknownCount > 0)
            {
                for (int i = 0; i < unknownCount; i++)
                {
                    // Save the ID as 4 bytes, and its booleans as another byte
                    int hashID = Convert.ToInt32(_unusedExpeditionData[i].name);
                    if (Expeditions.DEBUG) svmsg += "  [" + _unusedExpeditionData[i].name + " : ?" + hashID.ToString("X") + "? : " + _unusedExpeditionData[i].completed + " & " + _unusedExpeditionData[i].trackingActive + "] ";
                    writer.Write(hashID); byteCount += 4;
                    writer.Write(new BitsByte(
                        _unusedExpeditionData[i].completed,
                        _unusedExpeditionData[i].trackingActive,
                        _unusedExpeditionData[i].condition1Met,
                        _unusedExpeditionData[i].condition2Met,
                        _unusedExpeditionData[i].condition3Met
                        )); byteCount += 1;
                }
            }
            // Bin this now, we don't need it
            _unusedExpeditionData = new List<Expedition>();

            if (Expeditions.DEBUG)
            {
                svmsg += " Filesize = " + byteCount + "B";

                if (resetProgress)
                {
                    svmsg += "\n[cmd OK0]: FORCE DELETE ALL PROGRESS!";
                }
                else if (discardUnknowns)
                {
                    svmsg += "\n[cmd #]: FORCE DELETE UNKNOWNS!";
                }
            }

            // Reset this
            _localExpeditionList = new List<Expedition>();
        }

        public override void LoadCustomData(BinaryReader reader)
        {
            _version = reader.ReadInt32();
            if (_version == _versionCurrent)
            {
                if (Expeditions.DEBUG) dbgmsg += "\nLOAD v:" + _version + "/" + _versionCurrent;

                // Create a new progress storage
                _localExpeditionList = new List<Expedition>();

                // Create a dictionary of keys -> index
                Dictionary<int, int> hashToIndex = new Dictionary<int, int>();
                for(int i = 0; i < Expeditions.GetExpeditionsList().Count; i++)
                {
                    Expedition e = Expeditions.GetExpeditionsList()[i].expedition;
                    // Populate both lists
                    try
                    {
                        hashToIndex.Add(
                            Expedition.GetHashID(e), // The hashcode
                            i               // The index
                            );
                        _localExpeditionList.Add(new Expedition());
                    }
                    catch
                    {
                        string errorList = "";
                        foreach(ModExpedition me in Expeditions.GetExpeditionsList())
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
                if (Expeditions.DEBUG) dbgmsg += "|c:" + count + "?" + unknownCount;

                // Iterating
                for (int i = 0; i < count + unknownCount; i++)
                {
                    int expeditionID = reader.ReadInt32();
                    BitsByte flags = reader.ReadByte();
                    int index;
                    if (hashToIndex.TryGetValue(expeditionID, out index))
                    {
                        // Write match found at index
                        Expedition e = _localExpeditionList[index];
                        e.completed = flags[0];
                        e.trackingActive = flags[1];
                        e.condition1Met = flags[2];
                        e.condition2Met = flags[3];
                        e.condition3Met = flags[4];
                        if (Expeditions.DEBUG) dbgmsg += " [" + expeditionID.ToString("X") + ";" + (e.completed ? "`" : ".") + (e.trackingActive ? "`" : ".") + "]";
                    }
                    else 
                    {
                        // Make a new one if not initialised
                        if (_unusedExpeditionData == null) _unusedExpeditionData = new List<Expedition>();
                        // No match found
                        Expedition e = new Expedition();
                        e.completed = flags[0];
                        e.trackingActive = flags[1];
                        e.condition1Met = flags[2];
                        e.condition2Met = flags[3];
                        e.condition3Met = flags[4];
                        e.name = "" + expeditionID;
                        _unusedExpeditionData.Add(e);
                        if (Expeditions.DEBUG) dbgmsg += " [" + expeditionID.ToString("X") + ";-]";
                    }
                }
            }
            else
            {
                if (_version < 0) return;
                if (Expeditions.DEBUG) dbgmsg += "(( LOAD v: " + _version + " / " + _versionCurrent + ")) ";
                ErrorLogger.Log("Expeditions: Player save file v" + _version + " is not a valid version number, somehow (You must've done goofed).");
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
                    for(int i = 0;i < _localExpeditionList.Count;i++)
                    {
                        Expeditions.GetExpeditionsList()[i].expedition.CopyProgress(
                            _localExpeditionList[i]);
                        dbgmsg += "(" + Expeditions.GetExpeditionsList()[i].expedition.name
                            + (_localExpeditionList[i].trackingActive ? "T-" : "n-")
                            + (Expeditions.GetExpeditionsList()[i].expedition.trackingActive ? ">T" : ">n") + ")";
                    }
                }
            }
        }

        public override void PostUpdate()
        {
            if(ExpeditionUI.visible && ExpeditionUI.viewMode == ExpeditionUI.viewMode_Tile)
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
                if(!tileRange.Intersects(boardRect))
                {
                    Expeditions.CloseExpeditionMenu();
                }
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                HitNPC(target);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                HitNPC(target);
            }
        }
        private void HitNPC(NPC target)
        {
            if (!target.active) return;
            if (target.realLife >= 0 && target.realLife != target.whoAmI) return;
            Expeditions.lastHitNPC = target;
            if( target.life <= 0)
            {
                Expeditions.lastKilledNPC = target;
            }
        }
    }
}
