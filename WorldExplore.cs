using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public class WorldExplore : ModWorld
    {
        private static int _version = 0;
        public static bool savedClerk = false;

        public override void Initialize()
        {
            savedClerk = false;
        }

        public override void SaveCustomData(BinaryWriter writer)
        {
            writer.Write(_version);

            // Booleans
            BitsByte flags = new BitsByte(
                savedClerk);
            writer.Write(flags);
        }
        public override void LoadCustomData(BinaryReader reader)
        {
            _version = reader.ReadInt32();
            if (_version == 0)
            {
                // Booleans
                BitsByte flags = reader.ReadByte();
                savedClerk = flags[0];
            }
            else
            {
                ErrorLogger.Log("Expeditions: Save file v" + _version + " is not a valid version number, somehow (You must've done goofed).");
            }
        }
    }
}
