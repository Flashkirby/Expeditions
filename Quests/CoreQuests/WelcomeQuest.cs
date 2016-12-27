using Terraria;
using Terraria.ID;

namespace Expeditions.Quests
{
    class WelcomeQuest : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Welcome to Terraria";
            expedition.difficulty = 0;
            expedition.ctgExplore = true;
            expedition.ctgImportant = true;

            expedition.conditionDescription1 = "Build a house for an NPC to move in";
        }
        public override void AddItemsOnLoad()
        {
            AddRewardMoney(Item.buyPrice(0, 0, 1, 0));
            AddRewardItem(mod.ItemType("BountyBook"), 1);
        }
        public override string Description(bool complete)
        {
            if (complete) return "Thanks for setting up some shelter. I get tonnes of random shipments from my employer to help discover land and collect samples, but I'm no good at adventuring. Would you be interested in helping? In exchange, I'm sure you could use the stuff I have lying around somewhere... ";
            if (WorldExplore.savedClerk)
            {
                if(expedition.npcHead == 0) SetNPCHead(API.NPCIDClerk);
                return "Hey, you new here as well? I recently got assigned to " + Main.worldName + ", but this wasn't the cushy desk job I was expecting... You any good at house building? I can't focus at all with all these " + (Main.dayTime ? "slimes" : "zombies") + " around. ";
            }
            return "To whoever reads this,\nI'm out right now, but if you could find some housing I'll be super thankful when I get back! ";
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            // Check if an NPC has a house every second
            if (!cond1 && Main.time % 60 == 0)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (!Main.npc[i].active || Main.npc[i].type == NPCID.OldMan) continue;
                    if (Main.npc[i].townNPC && !Main.npc[i].homeless) cond1 = true;
                }
            }
            return cond1;
        }
    }
}
