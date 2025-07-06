using Terraria.ModLoader;

namespace Expeditions144.Items
{
    /// <summary>
    /// Used in shops that require this as a special currency. 
    /// Expeditions should only reward up to 3 of these.
	/// Remember also the exclusivity comes from the quests,
	/// Not just the coupons
    /// </summary>
    public class BountyVoucher : ModItem
    {
        public static string itemName = "Expedition Coupon";
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault(itemName);
            Tooltip.SetDefault("Trade in for exclusive items at certain stores\n"
              + "'Proof of achievement'");*/
        }

        public override void SetDefaults()
        {
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 999;
			Item.value = 0;
			Item.rare = -11; //quest tier
        }
    }
}
