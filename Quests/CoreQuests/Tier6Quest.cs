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
        }
        public override string Description(bool complete)
        {
            return "I have a theory that there's some kind of hallowed equivalent to the " + (WorldGen.crimson ? "crimtane" : "demonite") + " metals that we found in said biome. Thing is, they dropped from bosses... so I wonder if there are counterparts to things like the Eye of Cthulu we haven't seen yet. ";
        }

        public override bool CheckPrerequisites(Player player)
        {
            return Expeditions.GetCurrentExpeditionTier() >= expedition.difficulty - 1;
        }

        public override bool CheckConditions(Player player, ref bool condition1, ref bool condition2, ref bool condition3)
        {
            if(!condition1 && !Main.dayTime)
            {
                foreach(NPC npc in Main.npc)
                {
                    if (npc.active && npc.boss)
                    {
                        condition1 = npc.type == NPCID.Spazmatism
                            || npc.type == NPCID.Retinazer
                            || npc.type == NPCID.TheDestroyer
                            || npc.type == NPCID.SkeletronPrime;
                        break;
                    }
                }
            }
            return condition1;
        }
    }
}
