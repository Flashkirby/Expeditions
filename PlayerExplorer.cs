using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public class PlayerExplorer : ModPlayer
    {
        private static int _version = _versionCurrent;
        private const int _versionCurrent = 0;
        public static string message;

        private List<ModExpedition> _localExpeditionList;

        public override void SaveCustomData(BinaryWriter writer)
        {
            writer.Write(_versionCurrent);
            if (Expeditions.DEBUG) message = "\nSAVE v: " + _versionCurrent;

            // Expeditions
            List<ModExpedition> expeditions = Expeditions.expeditionList;
            // Save the counts
            int count = expeditions.Count;
            if (Expeditions.DEBUG) message += "| c:" + count;
            writer.Write(count);
            for (int i = 0; i < count; i++)
            {
                // Save the ID as 4 bytes, and its booleans as another byte
                //if (Expeditions.DEBUG) message += "\n" + expeditions[i].expedition.title + " : " + expeditions[i].expedition.GetHashID().ToString("X") + " : " + expeditions[i].expedition.completed;
                writer.Write(expeditions[i].expedition.GetHashID());
                writer.Write(new BitsByte(
                    expeditions[i].expedition.completed,
                    expeditions[i].expedition.trackingActive,
                    expeditions[i].expedition.condition1Met,
                    expeditions[i].expedition.condition2Met,
                    expeditions[i].expedition.condition3Met
                    ));
            }
        }

        public override void LoadCustomData(BinaryReader reader)
        {
            _version = reader.ReadInt32();

            if (Expeditions.DEBUG) message += "\nLOAD v: " + _version + " / " + _versionCurrent;
            if (_version == _versionCurrent)
            {
                // Expeditions
                _localExpeditionList = Expeditions.GetExpeditionsListCopy();
                List<ModExpedition> myExpeditions = _localExpeditionList;
                // Create a dictionary of keys -> index
                Dictionary<int, int> hashToIndex = new Dictionary<int, int>();
                for(int i = 0; i < myExpeditions.Count; i++)
                {
                    hashToIndex.Add(
                        myExpeditions[i].expedition.GetHashID(), // The hashcode
                        i               // The index
                        );
                }

                // Reset local expeditions
                initialiseExpeditions();
                // Read expeditions
                int count = reader.ReadInt32();
                if (Expeditions.DEBUG) message += " | LOAD c:" + count;
                for (int i = 0; i < count; i++)
                {
                    int expeditionID = reader.ReadInt32();
                    BitsByte flags = reader.ReadByte();
                    int index;
                    if (hashToIndex.TryGetValue(expeditionID, out index))
                    {
                        Expedition e = myExpeditions[index].expedition;
                        e.completed = flags[0];
                        e.trackingActive = flags[1];
                        e.condition1Met = flags[2];
                        e.condition2Met = flags[3];
                        e.condition3Met = flags[4];
                        if (Expeditions.DEBUG) message += "\n" + e.title + " : " + expeditionID.ToString("X") + " : " + e.completed + " & " + e.trackingActive;
                    }
                    else
                    {
                        if (Expeditions.DEBUG) message += "\n" + expeditionID.ToString("X") + " : Not Found";
                    }
                }
            }
            else
            {
                if (_version < 0) return;
                ErrorLogger.Log("Expeditions: Player save file v" + _version + " is not a valid version number, somehow (You must've done goofed).");
            }
        }
        private void initialiseExpeditions()
        {
            if (Expeditions.DEBUG) message += "(rinit)";
            //initiliase all the expeditions
            foreach (ModExpedition mex in _localExpeditionList)
            {
                mex.expedition.ResetProgress();
            }
        }

        public void CopyLocalExpeditionsToMain()
        {
            if (Main.netMode != 2 && player.whoAmI == Main.myPlayer)
            {
                if (Expeditions.DEBUG) { Main.NewText(player.name + " copying to Expeditions", 200, 150, 255); }
                if (_localExpeditionList == null)
                {
                    // Reset local expeditions
                    _localExpeditionList = Expeditions.GetExpeditionsListCopy();
                    initialiseExpeditions();
                }
                // Set the expeditions to use this list
                Expeditions.expeditionList = _localExpeditionList;
            }
        }
    }
}
