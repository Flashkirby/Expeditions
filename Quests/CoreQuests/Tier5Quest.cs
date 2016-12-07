using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier5Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Placeholder5";
            expedition.difficulty = 5;
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
