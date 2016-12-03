using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public class PlayerExplorer : ModPlayer
    {
        private static int _version = _versionCurrent;
        private const int _versionCurrent = -6;
        public static string message;

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
                if (Expeditions.DEBUG) message += "\n" + expeditions[i].expedition.title + " : " + expeditions[i].expedition.GetHashID().ToString("X") + " : " + expeditions[i].expedition.completed;
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
                List<ModExpedition> expeditions = Expeditions.expeditionList;
                // Create a dictionary of keys -> index
                Dictionary<int, int> hashToIndex = new Dictionary<int, int>();
                for(int i = 0; i < expeditions.Count; i++)
                {
                    hashToIndex.Add(
                        expeditions[i].expedition.GetHashID(), // The hashcode
                        i               // The index
                        );
                }

                // Reset expeditions
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
                        Expedition e = expeditions[index].expedition;
                        e.completed = flags[0];
                        e.trackingActive = flags[1];
                        e.condition1Met = flags[2];
                        e.condition2Met = flags[3];
                        e.condition3Met = flags[4];
                        if (Expeditions.DEBUG) message += "\n" + e.title + " : " + expeditionID.ToString("X") + " : " + e.completed;
                    }
                    else
                    {
                        if (Expeditions.DEBUG) message += "\n" + expeditionID.ToString("X") + " : Not Found";
                    }
                }
            }
            else
            {
                ErrorLogger.Log("Expeditions: Player save file v" + _version + " is not a valid version number, somehow (You must've done goofed).");
            }
        }
        private void initialiseExpeditions()
        {
            if (Expeditions.DEBUG) message += " rinit";
            //initiliase all the expeditions
            foreach (ModExpedition mex in Expeditions.expeditionList)
            {
                mex.expedition.ResetProgress();
            }
        }
    }
}
