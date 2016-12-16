using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.NPCs
{
    /// <summary>
    /// The sole executive in charge of the Ministry of Cartography and 
    /// Taxonomy's foreign branch. Her employers rarely actually 
    /// communicate, but they send plenty of shipments over everytime a 
    /// new report rolls in, which she is more than happy to divulge.
    /// 
    /// Enjoys discovering and documenting new things, but both annoyed 
    /// and concerned with effectively being stuck at the bottom of the
    /// corporate ladder.
    /// 
    /// Uses magic gratuitously in everyday life, akin to the Wizard.
    /// </summary>
    class Clerk : ModNPC
    {
        public override void SetDefaults()
        {
            npc.name = "Clerk";
            npc.width = 18;
            npc.height = 40;
            npc.townNPC = true;
            npc.friendly = true;
            Main.npcFrameCount[npc.type] = 21;

            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.soundHit = 1;
            npc.soundKilled = 1;
            npc.knockBackResist = 0.5f;

            NPCID.Sets.ExtraTextureCount[npc.type] = 1;
            NPCID.Sets.ExtraFramesCount[npc.type] = 7; // Extra frames after walk animation
            NPCID.Sets.AttackFrameCount[npc.type] = 2; // Attack frames at the end
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.MagicAuraColor[npc.type] = new Color(238, 82, 255);
            NPCID.Sets.AttackType[npc.type] = 2;
            NPCID.Sets.AttackTime[npc.type] = 30; // time to execute 1 attack
            NPCID.Sets.AttackAverageChance[npc.type] = 40; // 1/chance to attack per frame
            NPCID.Sets.HatOffsetY[npc.type] = 2;

            animationType = NPCID.Stylist;
        }
        public override bool Autoload(ref string name, ref string texture, ref string[] altTextures)
        {
            altTextures = new string[] { "Expeditions/NPCs/Clerk_Party" };
            return mod.Properties.Autoload;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                // Not Dead
                for (int i = 0; i < damage / npc.lifeMax * 100.0; i++)
                {
                    Dust.NewDust(npc.position, npc.height, npc.width,
                        DustID.Blood, hitDirection, -1f);
                }
            }
            else
            {
                // Probably Dead
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.height, npc.width,
                        DustID.Blood, hitDirection * 2.5f, -2.5f);
                }
                // Spawn gores á la vanilla
                int goreHead = mod.GetGoreSlot("Gores/Clerk1");
                int goreArms = mod.GetGoreSlot("Gores/Clerk2");
                int goreLegs = mod.GetGoreSlot("Gores/Clerk3");
                Gore.NewGore(npc.position, npc.velocity, goreHead, 1f);
                Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 20f), npc.velocity, goreArms, 1f);
                Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 20f), npc.velocity, goreArms, 1f);
                Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 34f), npc.velocity, goreLegs, 1f);
                Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 34f), npc.velocity, goreLegs, 1f);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return WorldExplore.savedClerk;
        }
        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(9))
            {
                case 0:
                    return "Nastasia"; //Super Paper Mario
                case 1:
                    return "Jolene"; //Paper Mario TTYD
                case 2:
                    return "Susan"; //Planet Robobot
                case 3:
                    return "Rachel"; //Hotel Dusk: Room 215
                case 4:
                    return "Eva"; //Grim Fandango
                case 5:
                    return "Carol"; //Dilbert
                case 6:
                    return "Jen"; //IT Crowd
                case 7:
                    return "Donna"; //Dr. Who and Suits
                default:
                    return "Isabelle"; //Animal Crossing
            }
        }

        #region AI Behaviours

        private bool danger = false;
        private float wasSittingTimer = 0f;
        public override void PostAI()
        {
            Player player = Main.player[Main.myPlayer];
            // You 'saved' me!
            WorldExplore.savedClerk = true;
            
            danger = CheckDangered();

            // Overwrite getting off chair due to type != 15
            StayOnChairOverwriteVanilla();

            // Track if leaving npc whilst window is open
            bool anotherNPC = player.talkNPC > 0 && Main.npc[player.talkNPC].type == npc.type;
            if (ExpeditionUI.viewMode == ExpeditionUI.viewMode_NPC && ExpeditionUI.visible &&
                (player.talkNPC != npc.whoAmI && !anotherNPC)
                )
            {
                Expeditions.CloseExpeditionMenu();
                if (Expeditions.DEBUG) Main.NewText("Closing via talknpc change");
            }

            SitAtBountyBoard();
        }

        private bool CheckDangered()
        {
            bool danger = false;
            float num389 = (float)NPCID.Sets.DangerDetectRange[npc.type];
            for (int num395 = 0; num395 < 200; num395++)
            {
                bool? flag45 = NPCLoader.CanHitNPC(Main.npc[num395], npc);
                if (!flag45.HasValue || flag45.Value)
                {
                    bool flag46 = flag45.HasValue && flag45.Value;
                    if (Main.npc[num395].active && !Main.npc[num395].friendly && Main.npc[num395].damage > 0 && Main.npc[num395].Distance(npc.Center) < num389 && (!NPCID.Sets.Skeletons.Contains(Main.npc[num395].netID) | flag46))
                    {
                        danger = true;
                        break;
                    }
                }
            }

            return danger;
        }

        private void StayOnChairOverwriteVanilla()
        {

            // If standing around when I could still be sitting
            if (npc.ai[0] == 0f && !danger && wasSittingTimer > 0)
            {
                Point point = npc.Center.ToTileCoordinates();
                Tile tile = Main.tile[point.X, point.Y];
                if (tile.type == mod.TileType("BountyBoard"))
                {
                    TakeSeat(point, tile);
                    npc.ai[1] = 2; //normally sit around all day
                }
                else
                {
                    wasSittingTimer = 0f;
                }
            }
            else
            {
                wasSittingTimer = 0f;
            }
        }
        private void SitAtBountyBoard()
        {
            // Sit down on expedition boards
            if (npc.ai[0] == 1f && !danger && npc.velocity.Y == 0f && Main.dayTime)
            {
                // Get my tile
                Point point2 = npc.Center.ToTileCoordinates();
                bool flag61 = WorldGen.InWorld(point2.X, point2.Y, 1);

                // Check first if anyone else is sitting here
                if (flag61)
                {
                    for (int num471 = 0; num471 < 200; num471++)
                    {
                        if (Main.npc[num471].active && Main.npc[num471].aiStyle == 7 && Main.npc[num471].townNPC && Main.npc[num471].ai[0] == 5f)
                        {
                            Point a = Main.npc[num471].Center.ToTileCoordinates();
                            if (a.Equals(point2))
                            {
                                flag61 = false;
                                break;
                            }
                        }
                    }
                }
                if (flag61)
                {
                    Tile tile2 = Main.tile[point2.X, point2.Y];
                    flag61 = (tile2.type == mod.TileType("BountyBoard"));
                    // disregard parts with no seat
                    if (tile2.frameY <= 52)
                    {
                        if (tile2.frameX < 54) flag61 = false;
                    }
                    else
                    {
                        if (tile2.frameX >= 16) flag61 = false;
                    }
                    if (flag61)
                    {
                        TakeSeat(point2, tile2);
                        npc.ai[1] = (float)(900 + Main.rand.Next(10800));
                    }
                }
            }

            wasSittingTimer = (npc.ai[0] == 5f && Main.dayTime) ? npc.ai[1] : 0;
        }
        private void TakeSeat(Point point2, Tile tile2)
        {
            npc.ai[0] = 5f;
            bool flag = true;
            foreach(Player p in Main.player)
            {
                if(p.talkNPC == npc.whoAmI)
                {
                    flag = false;
                }
            }
            if(flag) npc.direction = ((tile2.frameY <= 52) ? -1 : 1);
            npc.Bottom = new Vector2((float)(point2.X * 16 + 8 + 2 * npc.direction), (float)(point2.Y * 16 + 32));
            npc.velocity = Vector2.Zero;
            npc.localAI[3] = 0f;
            if (Main.netMode != 1)
            {
                npc.netUpdate = true;
            }
        }

        #endregion
        #region AI Attacks

        private int getWeaponType()
        {
            if (Main.hardMode)
            { return 2; }
            else if (NPC.downedBoss1)
            { return 1; }
            return 0;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            switch (getWeaponType())
            {
                case 2:
                    damage = 40;
                    knockback = 5f;
                    break;
                case 1:
                    damage = 21;
                    knockback = 5f;
                    break;
                default:
                    damage = 10;
                    knockback = 0f;
                    break;
            }
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            switch (getWeaponType())
            {
                case 2:
                    cooldown = 32; //min cooldown from START of attack (no inc. attack time)
                    randExtraCooldown = 33;
                    break;
                case 1:
                    cooldown = 36; //min cooldown from START of attack (no inc. attack time)
                    randExtraCooldown = 26;
                    break;
                default:
                    cooldown = 40; //min cooldown from START of attack (no inc. attack time)
                    randExtraCooldown = 20;
                    break;
            }
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            switch (getWeaponType())
            {
                case 2:
                    projType = ProjectileID.CrystalPulse;
                    attackDelay = 30; //how many frames before projectile spawns
                    if (npc.localAI[3] == attackDelay - 1) Main.PlaySound(2, npc.Center, 109);
                    break;
                case 1:
                    projType = ProjectileID.AmethystBolt; //ruby stats though
                    attackDelay = 30; //how many frames before projectile spawns
                    if (npc.localAI[3] == attackDelay - 1) Main.PlaySound(2, npc.Center, 43);
                    break;
                default:
                    projType = ProjectileID.Spark;
                    attackDelay = 30; //how many frames before projectile spawns
                    if (npc.localAI[3] == attackDelay - 1) Main.PlaySound(2, npc.Center, 8);
                    break;
            }
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            gravityCorrection = 0f;
            switch (getWeaponType())
            {
                case 2:
                    multiplier = 13f;
                    randomOffset = 0.6f;
                    break;
                case 1:
                    multiplier = 9f;
                    randomOffset = 0.3f;
                    break;
                default:
                    multiplier = 9f;
                    randomOffset = 0.5f;
                    gravityCorrection = 1f;
                    break;
            }
        }
        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 0.5f;
        }

        #endregion

        #region Talk Shop

        public override string GetChat()
        {
            Expeditions.CloseExpeditionMenu(true); // Stop conflict caused by Bounty Book
            //"Ok, here's the deal. I'm really not cut out for adventuring, but my employers are constantly demanding information about WORLDNAME. They also send me goodies whenever I document something new, which I'm more than willing to share... catch my drift? But we'll need a base camp first, and I've got all this stuff lying around..."
            if(npc.homeless)
            {
                return "Hmm... a tent over here, a campfire over there...";
            }
            return "Oh... um... ignore me. I'm waiting for my assignment. ";

            //bloodmoon
            //  paper work
            //  go really far away
            //bountyboard in underworld
            //stapler
            //how does X you ask? Magic.

        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Lang.inter[28];
            button2 = "Expeditions";
        }
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                Expeditions.OpenExpeditionMenu(ExpeditionUI.viewMode_NPC);
            }
        }
        
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            if (!Expeditions.CompletedWelcomeQuest())
            {
                // Until completing quest, has the bare basics
                if (WorldGen.CopperTierOre == TileID.Copper)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.CopperShortsword); nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.CopperPickaxe); nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.CopperAxe); nextSlot++;
                }
                else
                {
                    shop.item[nextSlot].SetDefaults(ItemID.TinShortsword); nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.TinPickaxe); nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.TinAxe); nextSlot++;
                }
                return;
            }

            // Sell default items
            shop.item[nextSlot].SetDefaults(mod.ItemType("BountyBook")); nextSlot++;
            if (WorldGen.GoldTierOre == TileID.Gold)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoldPickaxe); nextSlot++;
                shop.item[nextSlot].SetDefaults(ItemID.GoldAxe); nextSlot++;
            }
            else
            {
                shop.item[nextSlot].SetDefaults(ItemID.PlatinumPickaxe); nextSlot++;
                shop.item[nextSlot].SetDefaults(ItemID.PlatinumAxe); nextSlot++;
            }

            // Gold tier expensive weapons
            shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerSword")); nextSlot++;
            shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerPike")); nextSlot++;
            shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerBow")); nextSlot++;

            // Biome gun
            if (!WorldGen.crimson)
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerCarbine")); nextSlot++;
            }
            else
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerRepeater")); nextSlot++;
            }

            // Give some magic from eye of cthulu
            if(NPC.downedBoss1)
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerBook")); nextSlot++;
                shop.item[nextSlot].SetDefaults(mod.ItemType("WayfarerStaff")); nextSlot++;
            }
        }

        #endregion

    }
}
