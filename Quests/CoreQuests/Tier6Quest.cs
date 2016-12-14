using System;
using Terraria;
using Terraria.ID;
using Expeditions;

namespace Expeditions.Quests
{
    class Tier6Quest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Mecha Revenge";
            SetNPCHead(API.NPCIDClerk);
            expedition.difficulty = 6;
            expedition.ctgSlay = true;
            expedition.ctgCollect = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Face against a mechanical boss";

        }
        public override void AddItemsOnLoad()
        {
            expedition.AddDeliverable(ItemID.HallowedBar, 3);

            AddRewardItem(ItemID.GoldCoin, 8);

            // Give a taster of life fruits if not obtained yet
            AddRewardItem(ItemID.LifeFruit, 1);

            // Standard useful boss items
            if (WorldGen.crimson)    AddRewardItem(ItemID.IchorCampfire);
            else                    AddRewardItem(ItemID.CursedCampfire);
            AddRewardItem(ItemID.HeartStatue, 1);
            AddRewardItem(ItemID.HeartLantern, 3);
            AddRewardItem(ItemID.StarinaBottle, 3);
            AddRewardItem(ItemID.BuilderPotion, 5);
            AddRewardItem(ItemID.GreaterHealingPotion, 10);
            AddRewardItem(ItemID.GreaterManaPotion, 10);

        }
        public override string Description(bool complete)
        {
            if (complete) return "A mechanical what now!? Well, you know the drill. Hey, if you haven't done it yet, you should totally build an arena and fight on your terms! That'll show 'em. ";
            return "I have a theory that there's some kind of hallowed equivalent to the " + (WorldGen.crimson ? "crimtane" : "demonite") + " metals that we found in said biome. Thing is, they dropped from bosses... so I wonder if there are counterparts to things like the Eye of Cthulu we haven't seen yet. ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if(!cond1 && !Main.dayTime)
            {
                foreach(NPC npc in Main.npc)
                {
                    if (npc.active && npc.boss)
                    {
                        cond1 = npc.type == NPCID.Spazmatism
                            || npc.type == NPCID.Retinazer
                            || npc.type == NPCID.TheDestroyer
                            || npc.type == NPCID.SkeletronPrime;
                        break;
                    }
                }
            }
            return cond1;
        }
    }
}
