using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier7Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Placeholder7";
            expedition.difficulty = 7;
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
