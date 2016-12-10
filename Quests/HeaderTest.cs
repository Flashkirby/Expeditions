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
            return "This mod adds Expeditions, which are basically quests that have conditions and give rewards. Neato! ";
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            condition1 = true;
            return condition1 && condition2;
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.DEBUG;
        }
    }
}
