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
            expedition.name = "Soaring the Skies";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 5;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Equip wings";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 5);
        }
        public override string Description(bool complete)
        {
            return "Have you seen white, snake-like beasts flying in the skies as of late? I've heard reports that they have harbour powerful souls which could make surviving against these new creatures much easier, if you could find a use for them. What do you suppose? ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (!condition1)
            {
                foreach(Item i in player.miscEquips)
                {
                    if (i.wingSlot > 0)
                    {
                        condition1 = true;
                        continue;
                    }
                }
            }
            return condition1;
        }
    }
}
