using System;
using Terraria;
using Terraria.ID;
using Expeditions;
using System.Collections.Generic;

namespace Expeditions.Quests
{
    class ExampleExpedition : ModExpedition
    {
        public override void SetDefaults()
        {
            expedition.name = "Hello World";
            SetNPCHead(NPCID.Guide);
            expedition.difficulty = 1;
            expedition.ctgCollect = true;
            expedition.ctgImportant = false;
            expedition.repeatable = true;
            expedition.partyShare = true;
            expedition.conditionDescription1 = "Stand on the ground";
            expedition.conditionDescription2 = "Run at 30mph";
            expedition.conditionCountedMax = 120;
            expedition.conditionDescriptionCountable = "Jump in the air for " + expedition.conditionCountedMax + " ticks";
        }
        public override void AddItemsOnLoad()
        {
            expedition.AddDeliverable(ItemID.DirtBlock, 1);

            AddRewardItem(ItemID.DirtBlock, 2);
            AddRewardPrefix(ItemID.Shackle, 65);
            Item item = new Item();
            item.SetDefaults(ItemID.CobaltBar);
            item.name = "Cobalt or Palladium Bar";
            expedition.AddReward(item);
            AddRewardItem(WorldGen.copperBar);
            AddRewardItem(WorldGen.ironBar);
            AddRewardItem(WorldGen.silverBar);
            AddRewardItem(WorldGen.goldBar);
        }
        public override string Description(bool complete)
        {
            if (complete) return "You've completed this repeatable expedition. Good for you! ";
            return "This is a sample quest, with a lot of random text to help build unnessecary amounts of space to create lines. ";
        }

        public override void CheckConditionCountable(Player player, ref int count, int max)
        {
            if(player.velocity.Y != 0f)
            {
                count++;
            }
        }

        public override bool CheckConditions(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            cond1 = player.velocity.Y == 0f;
            if (!cond2 && cond1) 
            {
                //if(Expeditions.DEBUG) Main.NewText(Math.Abs(player.velocity.X) + " is < 6");
                cond2 = Math.Abs(player.velocity.X) > 6;
            }
            return cond1 && cond2;
        }

        public override bool CheckPrerequisites(Player player)
        {
            if (Expeditions.DEBUG)
            {
                foreach (Item item in player.inventory)
                {
                    if (item.rare == -1) return true;
                }
            }
            return false;
        }

        public override void PreCompleteExpedition(List<Item> rewards)
        {
            rewards[2].SetDefaults(ItemID.PalladiumBar);
            rewards[2].stack = 2;
        }

        public override void OnNewDay()
        {
            expedition.ResetProgress(true);
            if (Expeditions.DEBUG) Main.NewText("Expedition was reset.", 
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }
    }
}
