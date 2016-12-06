using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier2QuestCrimson : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "An Ore against Nature";
            expedition.difficulty = 2;
            expedition.deliver = true;
            expedition.important = true;

            expedition.AddDeliverable(ItemID.CrimtaneOre, 10);

            AddRewardItem(ItemID.GoldCoin, 1);
            AddRewardItem(ItemID.SilverCoin, 20);
        }
        public override string Description(bool complete)
        {
            return "So I've been hearing rumours of a rare and powerful yet 'evil' ore found underground... a sizeable sample is a must! More expeditions should be available after this. ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.unlockedTier1Quests && WorldGen.crimson;
        }
    }
}
