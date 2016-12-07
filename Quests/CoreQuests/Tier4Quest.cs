using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier4Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Underworld Mystery";
            expedition.difficulty = 4;
            expedition.defeat = true;
            expedition.important = true;

            expedition.conditionDescription1 = "Enter the Underworld";
            expedition.conditionDescription2 = "Investigate voodoo doll ritual";
        }
        public override string Description(bool complete)
        {
            return "So I've been trying to decipher these dungeon books, but so far not much luck. However there are a couple that seem to illustrate some kind of underworld ritual using... voodoo dolls? Mind taking a look - it could be something big! ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (!condition1) condition1 = player.ZoneUnderworldHeight;
            if (condition1 && !condition2) condition1 = Main.hardMode;
            return condition1 && condition2;
        }
    }
}
