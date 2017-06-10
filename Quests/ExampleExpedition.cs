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
            expedition.conditionDescription3 = "Craft an item";
            expedition.conditionCountedMax = 120;
            expedition.conditionDescriptionCountable = "Jump in the air for " + expedition.conditionCountedMax + " ticks";
        }
        public override void AddItemsOnLoad()
        {
            AddDeliverable(ItemID.PalmWood, 1);
            AddDeliverable(ItemID.PalladiumBar, 1);
            AddDeliverableAnyOf(new int[] {
                ItemID.WoodenSword,
                ItemID.CopperBroadsword,
                ItemID.TinBroadsword
            }, 1);

            AddRewardItem(ItemID.DirtBlock, 2);
            AddRewardPrefix(ItemID.Shackle, 65);
            Item item = new Item();
            item.SetDefaults(ItemID.CobaltBar);
            item.SetNameOverride("Cobalt or Palladium Bar");
            expedition.AddReward(item);
            AddRewardItem(WorldGen.copperBar);
            AddRewardItem(WorldGen.ironBar);
            AddRewardItem(WorldGen.silverBar);
            AddRewardItem(WorldGen.goldBar);
        }
        public override string Description(bool complete)
        {
            string message = "";
            if (WorldExplore.IsCurrentDaily(expedition)) message = "I'm the daily quest! ";
            if (complete) return "You've completed this repeatable expedition. Good for you! " + message;
            return "This is a sample quest, with a lot of random text to help build unnessecary amounts of space to create lines. " + message;
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
            return cond1 && cond2 && cond3;
        }

        public override bool CheckPrerequisites(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (WorldExplore.IsCurrentDaily(expedition)) return true;
            if (Expeditions.DEBUG)
            { return API.InInventory[ItemID.FishingSeaweed]; }
            return false;
        }

        public override void PreCompleteExpedition(List<Item> rewards, List<Item> deliveredItems)
        {
            Main.NewText("PreComplete");
            bool givePalladium = true;
            foreach(Item item in deliveredItems)
            {
                if(item.type == ItemID.WoodenSword)
                {
                    givePalladium = false;
                }
            }
            if(givePalladium)
            {
                rewards[2].SetDefaults(ItemID.PalladiumBar);
            }else
            {
                rewards[2].SetDefaults(ItemID.CobaltBar);
            }
            rewards[2].stack = 2;
        }

        public override bool IncludeAsDaily()
        {
            return true;
        }
        
        public override void OnNewDay(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            expedition.ResetProgress(true);
            if (Expeditions.DEBUG) Main.NewText("Expedition was reset for daytime.", 
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }
        public override void OnNewNight(Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            expedition.ResetProgress(true);
            if (Expeditions.DEBUG) Main.NewText("Expedition was reset for nighttime.",
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }

        public override void OnCombatWithNPC(NPC npc, bool playerGotHit, Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (Expeditions.DEBUG) Main.NewText(npc.GivenOrTypeName + " in combat, who hit? " + playerGotHit,
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }
        public override void OnKillNPC(NPC npc, Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (Expeditions.DEBUG) Main.NewText(npc.GivenOrTypeName + " got DEAD",
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }

        public override void OnCraftItem(Item item, Recipe recipe, Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            cond3 = true;
            if (!Expeditions.DEBUG) return;
            for (int i = 0; i < recipe.requiredItem.Length; i++)
            {
                if (recipe.requiredItem[i] == null || recipe.requiredItem[i].stack <= 0) continue;
                Main.NewText(item.Name + " from " + recipe.requiredItem[i].Name,
                    Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
            }
        }
        public override void OnPickupItem(Item item, Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (Expeditions.DEBUG) Main.NewText("Picked up " + item.Name,
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }

        public override void OnKillTile(int x, int y, int type, Player player, ref bool cond1, ref bool cond2, ref bool cond3, bool condCount)
        {
            if (Expeditions.DEBUG) Main.NewText("Break tile " + Main.tile[x, y].type,
                Expedition.textColour.R, Expedition.textColour.G, Expedition.textColour.B);
        }
    }
}
