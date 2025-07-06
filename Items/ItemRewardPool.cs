using System;
using System.Collections.Generic;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Expeditions144.Items
{
    public static class ItemRewardPool
    {
        public static List<int> mainRewards;
        public static List<int> ammos;
        public static List<int> resourceRewards;
        public const int resourceReducedRarity = 1;
        public static void GenerateRewardPool()
        {
            mainRewards = new List<int>();
            ammos = new List<int>();
            resourceRewards = new List<int>();

            Item item;
            for (int i = 0; i < TextureAssets.Item.Length; i++)
            {
                try
                {
                    item = new Item();
                    item.SetDefaults(i);
                    try { item.ModItem.SetDefaults(); } catch { }
                    if (item.pick > 0 || // No picks
                        (item.Name.Contains("Key") || Lang.GetItemName(i).Value.Contains("Key")) || // No "keys"
                        item.expert // No "experts" since boss only
                        )
                    { continue; }

                    if (item.ammo > 0)
                    { ammos.Add(item.type); }

                    if (
                            ( // Is a weapon
                            /****(item.melee || item.ranged || item.magic || item.summon || item.thrown)*/ !item.accessory
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
                        // No Tiles
                        if (item.createTile > 0) continue;
                        if (item.createWall > 0) continue;
                        // Non Weapons
                        if (item.material || //materials
                            item.potion || //potions
                            item.bait > 0 || //baits
                            item.vanity //vanity equips
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
            if (NPC.downedBoss1 || NPC.downedGoblins)
            {
                rare = Math.Max(rare, 1);
            }
            if (NPC.downedBoss2)
            {
                rare = Math.Max(rare, 2);
            }
            if (NPC.downedBoss3 || NPC.downedQueenBee)
            {
                rare = Math.Max(rare, 3);
            }
            if (Main.hardMode)
            {
                rare = Math.Max(rare, 4);
            }
            if (NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3 || NPC.downedFrost || NPC.downedPirates)
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
            #region Main Reward
            List<ItemRewardData> rewards = new List<ItemRewardData>();
            Item item;
            if (mainRewards.Count > 0)
            {
                int mainReward = ItemID.CopperShortsword;
				/****item = new Item();
                item.SetDefaults(mainReward);
                try { item.ModItem.SetDefaults(); } catch { }*/
				item = new Item(mainReward);

				// Limit by rare
				bool goForTopTier = Main.rand.Next(3) == 0; // 33% chance of going for top rare
                bool goForHighTier = Main.rand.Next(2) == 0; // 50% chance of going for good rare

                // Try random 511 times
                for (int i = 0; i < 511; i++)
                {
                    mainReward = mainRewards[Main.rand.Next(mainRewards.Count)];
					item = new Item(mainReward);
					/****item = new Item();
                    item.SetDefaults(mainReward);
                    try { item.modItem.SetDefaults(); } catch { }*/

					int lowRare = 0;
                    if (goForTopTier) lowRare = rare; // Only the best
                    if (goForHighTier) lowRare = rare - 2; // The good stuff
                    if (goForTopTier && goForHighTier) lowRare = rare - 1; //If both, give leeway

                    //Prevent custom and quest items maybe? (Thorium Blood Orange rarity)
                    if (rare < 0) rare = 0;
                    if (lowRare < -1) rare = -1;

                    if (item.rare <= rare && item.rare >= lowRare)
                    {
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
                        // Check ammo items
                        if (item.useAmmo > 0)
                        {
                            //Main.NewText(item.name + " uses ammo " + item.useAmmo);
                            Item ammunition;
                            foreach (int j in ammos)
                            {
                                ammunition = new Item();
                                ammunition.SetDefaults(j);
                                if (ammunition.ammo == item.useAmmo)
                                {
                                    stack = ammunition.maxStack;
                                    stack += Main.rand.Next(stack * 2);
                                    stack /= 15;
                                    stack = Math.Min(stack, ammunition.maxStack);
                                    stack = Math.Max(stack, 1);

                                    rewards.Add(new ItemRewardData(ammunition.type, stack));
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            #endregion

            #region Side Resource
            rare -= resourceReducedRarity;
            if (resourceRewards.Count > 0)
            {
                int sideReward = 0;
                int sideRareCount = Main.rand.Next(1, 2 + rare / 3); // Varying amounts of items
                if (rare <= 0)
                {
                    rare = 0;
                    sideRareCount = 1; // 1 extra item
                }
                int i = 0;
                while (i < sideRareCount)
                {
					/***item = new Item();
                    item.SetDefaults(sideReward);
                    try { item.modItem.SetDefaults(); } catch { }*/
					item = new Item(sideReward);
                    
                    // Try random 511 times
                    for (int j = 0; j < 511; j++)
                    {
                        sideReward = resourceRewards[Main.rand.Next(resourceRewards.Count)];
						/****item = new Item();
                        item.SetDefaults(sideReward);
                        try { item.modItem.SetDefaults(); } catch { }*/
						item = new Item(sideReward);

						// No items above this value in prehard (eg. wizard and pirate items)
						if (item.value > Item.buyPrice(0, 4) && !Main.hardMode) continue;

                        // No super rares, or hardmode (3) in prehard
                        if (item.rare <= rare && rare >= 0)
                        {
                            int stack = item.maxStack;
                            int maxCostStack = stack;
                            stack += Main.rand.Next(stack * 4);
                            if (item.value > 0)
                            { stack /= 16; }
                            else
                            { stack /= 8; }

                            if (item.value > 0)
                            {
                                // Stop costs going over 5-15 gold
                                maxCostStack = Item.buyPrice(0, Main.rand.Next(5, 16), 0, 0);
                                maxCostStack /= item.value;
                                maxCostStack = Math.Min(maxCostStack, item.maxStack);
                                maxCostStack = Math.Max(maxCostStack, 1);
                            }

                            stack = Math.Min(stack, maxCostStack);
                            stack = Math.Max(stack, 1);

                            //Main.NewText(i + " granted " + item.name + ":" + stack);
                            rewards.Add(new ItemRewardData(sideReward, stack));
                            i += 1 + item.rare;
                            break;
                        }
                    }
                }
            }
            #endregion

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
