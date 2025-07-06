using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions144
{
    public class NPCExplore : GlobalNPC
    {
        #region Shop
        public override void ModifyActiveShop(NPC npc, String shopName, Item[] items)
        {
			int nextSlot = 0;
			for (; nextSlot < items.Length && items[nextSlot] != null && !items[nextSlot].IsAir; nextSlot++);
			nextSlot++;

			if (npc.type == NPCID.Merchant) MerchantShop(items, ref nextSlot);
            if (npc.type == NPCID.SkeletonMerchant) SkeletonMerchantShop(items, ref nextSlot);
        }

        public void MerchantShop(Item[] items, ref int nextSlot)
        {
			if (nextSlot < items.Length)
			{
				items[nextSlot] = new Item(API.ItemIDExpeditionBook);
				nextSlot++;
			}
        }
        public void SkeletonMerchantShop(Item[] items, ref int nextSlot)
        {
            if (Main.moonPhase % 2 == 0) //Alternate between selling the box and board
            { API.AddShopItemVoucher(items, ref nextSlot, API.ItemIDRustedBox, 1); }
            else
            {
				if (nextSlot < items.Length)
				{
					items[nextSlot] = new Item(API.ItemIDExpeditionBoard);
					nextSlot++;
				}
			}
        }

        internal static void AddVoucherPricedItem(Item[] shop, ref int nextSlot, int itemID, int price)
        {
            price = Math.Min(999,Math.Max(0, price));

            shop[nextSlot] = new Item(itemID);
            shop[nextSlot].shopCustomPrice = new int?(price);
            shop[nextSlot].shopSpecialCurrency = Expeditions144.currencyVoucherID;
            nextSlot++;
        }

		#endregion

		public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			if (player.whoAmI != Main.myPlayer) return;
			foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
			{
				if (npc.life <= 0 || !npc.active)
				{ expKillNPC(me, npc); }
				expCombatWithNPC(me, npc);
			}
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (projectile.owner != Main.myPlayer) return;
			foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
			{
				if (npc.life <= 0 || !npc.active)
				{ expKillNPC(me, npc); }
				expCombatWithNPC(me, npc);
			}
		}

		public override void OnKill(NPC npc)
		{
			foreach (ModExpedition me in Expeditions144.GetExpeditionsList())
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
