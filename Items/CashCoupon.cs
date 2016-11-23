using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class CashCoupon : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Cash Coupon";
            item.toolTip = "Exchange it for coins";
            item.width = 30;
            item.height = 36;
            item.maxStack = 100000000;
            
            item.value = 5;
        }

        public override void UpdateInventory(Player player)
        {
            string priceString = "";

            int value = item.stack;
            int denomination;
            int coinLang;
            int stack;
            while (value > 0)
            {
                if (value >= 1000000) //platinum
                {
                    denomination = 1000000;
                    coinLang = 15;
                }
                if (value >= 10000) //gold
                {
                    denomination = 10000;
                    coinLang = 16;
                }
                if (value >= 100) //silver
                {
                    denomination = 100;
                    coinLang = 17;
                }
                else
                {
                    denomination = 1;
                    coinLang = 18;
                }

                stack = value / denomination;
                value -= stack * denomination;

                priceString += stack + " " + Lang.inter[coinLang] + " ";
            }
            item.toolTip2 = priceString;

            setRarity();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            setRarity();
        }

        private void setRarity()
        {
            if (item.stack < 100) { item.rare = 0; }
            else if (item.stack < 1000) { item.rare = 1; }
            else if (item.stack < 10000) { item.rare = 2; }
            else if (item.stack < 50000) { item.rare = 3; }
            else if (item.stack < 100000) { item.rare = 4; }
            else if (item.stack < 250000) { item.rare = 5; }
            else if (item.stack < 500000) { item.rare = 6; }
            else if (item.stack < 750000) { item.rare = 7; }
            else if (item.stack < 1000000) { item.rare = 8; }
            else if (item.stack < 3333333) { item.rare = 9; }
            else if (item.stack < 6666667) { item.rare = 10; }
            else { item.rare = 11; }
        }
    }
}
