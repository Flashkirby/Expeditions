using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Expeditions
{
    class WorldExplore : ModWorld
    {
        public static bool savedClerk = false;

        public override void Initialize()
        {
            if (Main.netMode == 2)
            {
                Console.WriteLine("Expeditions: World Initialising");
            }

            // Reset bools
            savedClerk = false;
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "savedClerk", savedClerk }
            };
        }

        public override void Load(TagCompound tag)
        {
            savedClerk = tag.GetBool("savedClerk");
        }

        public override void LoadLegacy(BinaryReader reader)
        {
            int _version = reader.ReadInt32();
            // Booleans
            BitsByte flags = reader.ReadByte();
            savedClerk = flags[0];
        }
    }
}
