using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class HardModeQuest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "The Wall";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 4;
            expedition.ctgSlay = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Defeat the Wall of Flesh";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 3);
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 65); //Warding
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 72); //Menacing
            AddRewardPrefix(mod.ItemType("PrefixApplicator"), 76); //Quick
            AddRewardItem(ItemID.PurificationPowder, 30);
        }
        public override string Description(bool complete)
        {
            if (complete) return "So the wall was holding back some crazy world-changing spirits? Well either way everything hurts more now, I'd suggest getting some new equipment, fast. Ooh, but isn't this exciting though! ";
            return "Want to find out what the Wall of Flesh drops? I do to, though it certainly seems tougher than anything you've fought before. I'd imagine something like a flat runway, some good defense and a reforged weapon will go a long way. ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if (!condition1) condition1 = player.ZoneUnderworldHeight;
            if (condition1 && player.ZoneUnderworldHeight && !condition2)
            {
                condition2 = Main.hardMode;
                if (!condition2) //check if we've seen the WoF
                {
                    condition2 = player.HasBuff(BuffID.Horrified) >= 0;
                }
            }
            return condition1 && condition2;
        }
    }
}
