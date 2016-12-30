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

            expedition.conditionDescription1 = "Equip some wings";

        }
        public override void AddItemsOnLoad()
        {
            AddRewardMoney(Item.buyPrice(0, 5, 0, 0));
            AddRewardItem(mod.ItemType("StockBox2"), 1);

            // Enough for mining tool
            AddRewardItem(ItemID.CobaltBar, 15); 
            AddRewardItem(ItemID.WoodenCrate, 2);

            // Weapon Buff potions are really useful
            if (WorldGen.crimson) { AddRewardItem(ItemID.FlaskofIchor, 2); }
            else { AddRewardItem(ItemID.FlaskofCursedFlames, 2); }
            AddRewardItem(ItemID.ArcheryPotion, 2);
            AddRewardItem(ItemID.MagicPowerPotion, 2);
            AddRewardItem(ItemID.SummoningPotion, 2);

            // Always need chests
            AddRewardItem(ItemID.SkywareChest);
        }
        public override string Description(bool complete)
        {
            if (complete) return "With the power of flight you can cover more ground, avoid enemy attacks and easily reach new places! What's not to love? Though, I'm deathly afraid of heights, goodness. ";
            return "Have you seen white, snake-like beasts flying in the skies as of late? I've heard reports that they have harbour powerful souls which could make surviving against these new creatures much easier, if you could find a use for them. What do you suppose? ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (!cond1)
            {
                cond1 = player.wingTimeMax > 0;
            }
            return cond1;
        }
    }
}
