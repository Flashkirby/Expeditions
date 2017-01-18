using Terraria.ModLoader;

namespace Expeditions.Items
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
        public override void SetDefaults()
        {
            item.name = itemName;
            item.toolTip = "Trade in for exclusive items at certain stores";
            item.toolTip2 = "'Proof of achievement'";
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.value = 0;
            item.rare = -11;
        }
    }
}
