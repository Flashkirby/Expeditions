using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier3Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Placeholder3";
            expedition.difficulty = 3;
            expedition.important = true;
        }
        public override string Description(bool complete)
        {
            return "";
        }
        public override void AddItemsOnLoad()
        {

        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
    }
}
