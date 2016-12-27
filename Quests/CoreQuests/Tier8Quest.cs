using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier8Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Temple Raid";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 8;
            expedition.ctgSlay = true;
            expedition.ctgExplore = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Gain access to the Jungle Temple";
        }
        public override void AddItemsOnLoad()
        {
            AddRewardItem(ItemID.GoldCoin, 15);
            AddRewardItem(mod.ItemType("StockBox2"), 1);

            // Boss summons!
            AddRewardItem(ItemID.LihzahrdPowerCell, 3);

            // Dungeon prep
            AddRewardItem(ItemID.EndurancePotion, 3);
            AddRewardItem(ItemID.GravitationPotion, 3);
            AddRewardItem(ItemID.InfernoPotion, 1);

            // Always need chests
            AddRewardItem(ItemID.LihzahrdChest);
        }
        public override string Description(bool complete)
        {
            if (complete) return "You had to fight a huge man-eating plant to recover the keys to the Temple? That sounds so cool! Like one of those action movies, y'know? Anyway, while you were down there, I posted another urgent expedition for you to look at.";
            return "I WANT YOU!... to investigate that jungle temple. But how can we get in I wonder. Knowing this place you'll probably need to defeat a bigger, scarier jungle boss, like the dungeon's Skeletron hijinks.";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }
        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (NPC.downedPlantBoss && !cond1)
            {
                try
                {
                    cond1 = Main.tile[
                        (int)(player.Center.X / 16f),
                        (int)(player.Center.Y / 16f)
                        ].wall == WallID.LihzahrdBrickUnsafe;
                }
                catch { }
            }
            return cond1;
        }
    }
}
