﻿using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;

namespace Expeditions.Items
{
    public static class ItemRewardPool
    {
        public static List<int> mainRewards;
        public static List<int> ammos;
        public static List<int> resourceRewards;
        public static void GenerateRewardPool()
        {
            mainRewards = new List<int>();
            ammos = new List<int>();
            resourceRewards = new List<int>();

            Item item;
            for (int i = 0; i < Main.itemTexture.Length; i++)
            {
                try
                {
                    item = new Item();
                    item.SetDefaults(i);
                    try { item.modItem.SetDefaults(); } catch { }
                    if (item.pick > 0 || // No picks
                        (item.name.Contains("Key") || Lang.itemName(i).Contains("Key")) || // No "keys"
                        item.expert // No "experts" since boss only
                        )
                    { continue; }

                    if (item.ammo > 0)
                    { ammos.Add(item.type); }

                    if (
                            ( // Is a weapon
                            (item.melee || item.ranged || item.magic || item.summon || item.thrown)
                            && item.damage > 0
                            && item.ammo == AmmoID.None
                            )
                        ||
                            ( // Is an actual accessory
                            item.accessory
                            && !item.vanity
                            )
                        )
                    {
                        mainRewards.Add(item.type);
                    }
                    else
                    {
                        // Non Weapons
                        if (item.material || //materials
                            item.potion || //potions
                            item.bait > 0 || //baits
                            item.vanity || //vanity equips
                            item.createTile > 0 // tiles
                            )
                        {
                            resourceRewards.Add(item.type);
                        }
                    }
                }
                catch { }
            }
        }

        public static int GetRewardRare(Player player)
        {
            int rare = 0;
            if (player.statLife >= 200 || NPC.downedBoss1 || NPC.downedGoblins)
            {
                rare = Math.Max(rare, 1);
            }
            if ((player.statLife >= 300 && NPC.downedBoss1) || NPC.downedBoss2)
            {
                rare = Math.Max(rare, 2);
            }
            if ((player.statLife >= 400 && NPC.downedBoss2) || NPC.downedBoss3 || NPC.downedQueenBee)
            {
                rare = Math.Max(rare, 3);
            }
            if (Main.hardMode)
            {
                rare = Math.Max(rare, 4);
            }
            if (player.statLife >= 500 || NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3 || NPC.downedFrost || NPC.downedPirates)
            {
                rare = Math.Max(rare, 5);
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                rare = Math.Max(rare, 6);
            }
            if (NPC.downedPlantBoss)
            {
                rare = Math.Max(rare, 7);
            }
            if (NPC.downedFishron || NPC.downedHalloweenKing || NPC.downedChristmasIceQueen || NPC.downedMartians || NPC.downedAncientCultist)
            {
                rare = Math.Max(rare, 8);
            }
            if (NPC.downedMoonlord)
            {
                rare = Math.Max(rare, 10);
            }
            //Main.NewText("Rare check is " + rare);
            return rare;
        }

        public static List<ItemRewardData> GenerateFullRewards(int rare)
        {
            List<ItemRewardData> rewards = new List<ItemRewardData>();
            Item item;
            if (mainRewards.Count > 0)
            {
                int mainReward = ItemID.CopperShortsword;
                item = new Item();
                item.SetDefaults(mainReward);
                try { item.modItem.SetDefaults(); } catch { }

                // Try random 255 times
                for (int i = 0; i < 255; i++)
                {
                    mainReward = mainRewards[Main.rand.Next(mainRewards.Count)];
                    item = new Item();
                    item.SetDefaults(mainReward);
                    try { item.modItem.SetDefaults(); } catch { }

                    // No super rares
                    if (item.rare > rare) continue;
                }

                int stack = item.maxStack;
                if (stack > 1) // For multi stack weapons like throwing
                {
                    stack += Main.rand.Next(stack * 2);
                    stack /= 8;

                    stack = Math.Min(stack, item.maxStack);
                    stack = Math.Max(stack, 1);
                }
                rewards.Add(new ItemRewardData(mainReward, stack));

                //Main.NewText("granted " + item.name);

                if (item.useAmmo > 0)
                {
                    //Main.NewText(item.name + " uses ammo " + item.useAmmo);
                    Item ammunition;
                    foreach (int i in ammos)
                    {
                        ammunition = new Item();
                        ammunition.SetDefaults(i);
                        if (ammunition.ammo == item.useAmmo)
                        {
                            stack = ammunition.maxStack;
                            stack += Main.rand.Next(stack * 2);
                            stack /= 8;
                            stack = Math.Min(stack, ammunition.maxStack);
                            stack = Math.Max(stack, 1);

                            rewards.Add(new ItemRewardData(ammunition.type, stack));
                            break;
                        }
                    }
                }
            }

            if (resourceRewards.Count > 0)
            {
                int sideReward = 0;
                int sideCount = Main.rand.Next(2, 5);
                for (int i = 0; i < sideCount; i++)
                {
                    sideReward = ItemID.Wood;
                    item = new Item();
                    item.SetDefaults(sideReward);
                    try { item.modItem.SetDefaults(); } catch { }

                    // Try random 255 times
                    for (int j = 0; j < 255; j++)
                    {
                        sideReward = resourceRewards[Main.rand.Next(mainRewards.Count)];
                        item = new Item();
                        item.SetDefaults(sideReward);
                        try { item.modItem.SetDefaults(); } catch { }

                        // No super rares
                        if (item.rare > rare) continue;
                    }

                    int stack = item.maxStack;
                    int maxCostStack = stack;
                    stack += Main.rand.Next(stack * 4);
                    if (item.value > 0)
                    { stack /= 16; }
                    else
                    { stack /= 8; }

                    if(item.value > 0)
                    {
                        // Stop costs going over 5-15 gold
                        maxCostStack = Item.buyPrice(0, Main.rand.Next(5, 16), 0, 0);
                        maxCostStack /= item.value;
                        maxCostStack = Math.Min(maxCostStack, item.maxStack);
                        maxCostStack = Math.Max(maxCostStack, 1);

                        //get lesser
                        stack = Math.Min(stack, maxCostStack);
                    }
                    
                    stack = Math.Min(stack, item.maxStack);
                    stack = Math.Max(maxCostStack, 1);

                    rewards.Add(new ItemRewardData(sideReward, stack));

                    //Main.NewText(i + " granted " + item.name + ":" + stack);
                }
            }

            return rewards;
        }
    }

    public class ItemRewardData
    {
        public int itemID = 0;
        public int stack = 1;

        public ItemRewardData(int id)
        {
            itemID = id;
            stack = 1;
        }
        public ItemRewardData(int id, int stacks)
        {
            itemID = id;
            stack = stacks;
        }
    }
}