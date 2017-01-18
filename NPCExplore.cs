using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions
{
    public class NPCExplore : GlobalNPC
    {
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
            API.AddShopItemVoucher(shop, ref nextSlot, API.ItemIDRelicBox, 1);
        }

        internal static void AddVoucherPricedItem(Chest shop, ref int nextSlot, int itemID, int price)
        {
            price = Math.Min(999,Math.Max(0, price));

            shop.item[nextSlot].SetDefaults(itemID);
            shop.item[nextSlot].shopCustomPrice = new int?(price);
            shop.item[nextSlot].shopSpecialCurrency = Expeditions.currencyVoucherID;
            nextSlot++;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            if (player.whoAmI != Main.myPlayer) return;
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                if (npc.life <= 0 || !npc.active)
                { me.OnKillNPC(npc); }
                me.OnCombatWithNPC(npc, false);
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (projectile.owner != Main.myPlayer) return;
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                if (npc.life <= 0 || !npc.active)
                { me.OnKillNPC(npc); }
                me.OnCombatWithNPC(npc, false);
            }
        }
        public override void NPCLoot(NPC npc)
        {
            foreach (ModExpedition me in Expeditions.GetExpeditionsList())
            {
                me.OnAnyNPCDeath(npc);
            }
        }
    }
}
