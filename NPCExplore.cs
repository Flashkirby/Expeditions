using System;

using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public class NPCExplore : GlobalNPC
    {
        internal static void AddVoucherPricedItem(Chest shop, ref int nextSlot, int itemID, int price)
        {
            price = Math.Min(999,Math.Max(0, price));

            shop.item[nextSlot].SetDefaults(itemID);
            shop.item[nextSlot].shopCustomPrice = new int?(price);
            shop.item[nextSlot].shopSpecialCurrency = Expeditions.currencyVoucherID;
            nextSlot++;
        }
    }
}
