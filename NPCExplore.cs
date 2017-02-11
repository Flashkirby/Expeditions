using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions
{
    public class NPCExplore : GlobalNPC
    {
        #region Shop
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Merchant) MerchantShop(shop, ref nextSlot);
            if (type == NPCID.SkeletonMerchant) SkeletonMerchantShop(shop, ref nextSlot);
        }

        public void MerchantShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(API.ItemIDExpeditionBook); nextSlot++;
        }
        public void SkeletonMerchantShop(Chest shop, ref int nextSlot)
        {
            if(Main.hardMode)
            {
                API.AddShopItemVoucher(shop, ref nextSlot, API.ItemIDRelicBox, 2);
            }
            else
            {
                API.AddShopItemVoucher(shop, ref nextSlot, API.ItemIDRustedBox, 1);
            }
        }

        internal static void AddVoucherPricedItem(Chest shop, ref int nextSlot, int itemID, int price)
        {
            price = Math.Min(999,Math.Max(0, price));

            shop.item[nextSlot].SetDefaults(itemID);
            shop.item[nextSlot].shopCustomPrice = new int?(price);
            shop.item[nextSlot].shopSpecialCurrency = Expeditions.currencyVoucherID;
            nextSlot++;
        }

        #endregion

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            if (player.whoAmI != Main.myPlayer) return;
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                if (npc.life <= 0 || !npc.active)
                { expKillNPC(me, npc); }
                expCombatWithNPC(me, npc);
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (projectile.owner != Main.myPlayer) return;
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                if (npc.life <= 0 || !npc.active)
                { expKillNPC(me, npc); }
                expCombatWithNPC(me, npc);
            }
        }
        public override void NPCLoot(NPC npc)
        {
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                expAnyNPCDeath(me, npc);
            }
        }

        private void expCombatWithNPC(ModExpedition me, NPC npc)
        {
            me.OnCombatWithNPC(npc, false, Main.LocalPlayer,
                          ref me.expedition.condition1Met,
                          ref me.expedition.condition2Met,
                          ref me.expedition.condition3Met,
                          me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                          );
        }
        private void expKillNPC(ModExpedition me, NPC npc)
        {
            me.OnKillNPC(npc, Main.LocalPlayer,
                          ref me.expedition.condition1Met,
                          ref me.expedition.condition2Met,
                          ref me.expedition.condition3Met,
                          me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                          );
        }
        private void expAnyNPCDeath(ModExpedition me, NPC npc)
        {
            me.OnAnyNPCDeath(npc, Main.LocalPlayer,
                          ref me.expedition.condition1Met,
                          ref me.expedition.condition2Met,
                          ref me.expedition.condition3Met,
                          me.expedition.conditionCounted >= me.expedition.conditionCountedMax
                          );
        }
    }
}
