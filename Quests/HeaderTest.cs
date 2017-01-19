using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class HeaderTest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Hello World";
            expedition.difficulty = 9;
            expedition.conditionDescription1 = "Stay cool";
            expedition.conditionDescription2 = "Install the mod";
        }
        public override void AddItemsOnLoad()
        {
            expedition.AddDeliverable(ItemID.GoldenKey, 1);

            AddRewardMoney(Item.buyPrice(1, 50, 25));
            AddRewardItem(ItemID.GoldenToilet);
        }
        public override string Description(bool complete)
        {
            if (WorldExplore.IsCurrentDaily(expedition)) return "I'm the daily quest! ";
            return "This mod adds Expeditions, which are basically quests that have conditions and give rewards. Neato! ";
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            cond1 = true;
            return cond1 && cond2;
        }

        public override bool CheckPrerequisites(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            return Expeditions.DEBUG;
        }

        public override bool IncludeAsDaily()
        {
            return true;
        }
    }
}
