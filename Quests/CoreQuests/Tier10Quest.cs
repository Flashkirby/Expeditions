using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier10Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "The Temple Conspiracy";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 10;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Investigate the lihzahrd altar";
            expedition.conditionDescription2 = "Investigate the dungeon's coven";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 25);
            AddRewardItem(ItemID.IronskinPotion, 10);
            AddRewardItem(ItemID.EndurancePotion, 10);
        }
        public override string Description(bool complete)
        {
            if(expedition.condition1Met) return "Some strange people have moved into the dungeon ever since that golem was destroyed. They look real suspicious worshipping that tablet, what looks like it was plundered from the temple. But I would've thought sun worshippers would look, I dunno, more grossly incandescent?";
            return "Have you explored much of that jungle temple yet? According to the artifacts so far, there seems to be some kind of lihzahrd fascination with, and worship of the sun. Maybe there's an altar buried deep? ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (!condition1) { condition1 = NPC.downedGolemBoss; }
            else
            {
                if (!condition2) condition2 = Expeditions.LastHitNPC.type == NPCID.CultistBoss;
            }
            return condition1 && condition2;
        }
    }
}
