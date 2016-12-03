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
    public class Clerk : ModNPC
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
            
            NPCID.Sets.ExtraFramesCount[npc.type] = 7; // Extra frames after walk animation
            NPCID.Sets.AttackFrameCount[npc.type] = 2; // Attack frames at the end
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.MagicAuraColor[npc.type] = new Color(238, 82, 255);
            NPCID.Sets.AttackType[npc.type] = 2;
            NPCID.Sets.AttackTime[npc.type] = 30; // time to execute 1 attack
            NPCID.Sets.AttackAverageChance[npc.type] = 40; // 1/chance to attack per frame
            NPCID.Sets.HatOffsetY[npc.type] = 4;

            animationType = NPCID.Steampunker;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int num;
            if (npc.life > 0) num = (int)Math.Min(damage, npc.life);
            else num = npc.lifeMax;
            num = num / 2 + 1;
            for (int i = 0; i < num; i++)
            {
                Dust.NewDust(npc.position, npc.height, npc.width,
                    DustID.Blood, hitDirection * 2, -1);
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

        public override void PostAI()
        {
            Player player = Main.player[Main.myPlayer];
            WorldExplore.savedClerk = true;

            // Track if leaving npc whilst window is open
            if (ExpeditionUI.viewMode == ExpeditionUI.viewMode_NPC && ExpeditionUI.visible &&
                player.talkNPC != npc.whoAmI
                )
            {
                Expeditions.CloseExpeditionMenu();
                if (Expeditions.DEBUG) Main.NewText("Closing via talknpc change");
            }
        }

        public override string GetChat()
        {
            //"Ok, here's the deal. I'm really not cut out for adventuring, but my employers are constantly demanding information about WORLDNAME. They also send me goodies whenever I document something new, which I'm more than willing to share... catch my drift? But we'll need a base camp first, and I've got all this stuff lying around..."
            if(npc.homeless)
            {
                return "Hmm... a tent over here, a campfire over there...";
            }
            return "Oh... um... ignore me. I'm waiting for my assignment. ";
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
            shop.item[nextSlot].SetDefaults(ItemID.CopperShortsword);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.CopperPickaxe);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.CopperAxe);
            nextSlot++;
        }


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
            switch(getWeaponType())
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

    }
}
