using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.NPCs
{
    public class Clerk : ModNPC
    {
        public override void SetDefaults()
        {
            npc.name = "Clerk";
            npc.width = 18;
            npc.height = 40;
            npc.townNPC = true;
            npc.friendly = true;
            Main.npcFrameCount[npc.type] = 23;

            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.soundHit = 1;
            npc.soundKilled = 1;
            npc.knockBackResist = 0.5f;
            
            NPCID.Sets.ExtraFramesCount[npc.type] = 9; // Extra frames after walk animation
            NPCID.Sets.AttackFrameCount[npc.type] = 4; // Attack frames at the end
            NPCID.Sets.DangerDetectRange[npc.type] = 500;
            NPCID.Sets.AttackType[npc.type] = 2;
            NPCID.Sets.MagicAuraColor[npc.type] = Color.Yellow;
            NPCID.Sets.AttackTime[npc.type] = 20;
            NPCID.Sets.AttackAverageChance[npc.type] = 20;
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

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(7))
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
                default:
                    return "Isabelle"; //Animal Crossing
            }
        }

        public override string GetChat()
        {
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
                Expeditions.OpenExpeditionMenu(false);
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


        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 4;
            knockback = 5.75f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.Spark;
            attackDelay = 15;
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 5f;
            randomOffset = 1f;
            gravityCorrection = 0.5f;
        }
        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 0.5f;
        }
    }
}
