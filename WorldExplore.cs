using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    class WorldExplore : ModWorld
    {
        private static int _version = _versionCurrent;
        private const int _versionCurrent = 0;
        public static bool savedClerk = false;

        public override void Initialize()
        {
            if (Main.netMode == 2)
            {
                Console.WriteLine("Expeditions: World Initialising");
            }
            else
            {
                // Set main list to loaded
                Main.player[Main.myPlayer].GetModPlayer<PlayerExplorer>(mod).CopyLocalExpeditionsToMain();
            }

            // Reset list items
            Expeditions.WorldInit();

            // Reset bools
            savedClerk = false;
        }

        public override void SaveCustomData(BinaryWriter writer)
        {
            writer.Write(_version);

            // Booleans
            writer.Write(new BitsByte(
                savedClerk
                ));
        }
        public override void LoadCustomData(BinaryReader reader)
        {
            _version = reader.ReadInt32();
            if (_version == _versionCurrent)
            {
                // Booleans
                BitsByte flags = reader.ReadByte();
                savedClerk = flags[0];
            }
            else
            {
                if (_version < 0) return;
                ErrorLogger.Log("Expeditions: World save file v" + _version + " is not a valid version number, somehow (You must've done goofed).");
            }
        }
    }
}
