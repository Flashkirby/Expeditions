using Terraria.ModLoader;

namespace Expeditions.Items
{
    /// <summary>
    /// Used in shops that require this as a special currency. 
    /// Expeditions should only reward up to 3 of these,
	/// Daily quests typically give 1, think Angler rewards.
	/// Standard requests give 2, like easymode boss killing.
	/// Big quests grant 3, like hardmode boss activities etc.
	/// Remember also the exclusivity comes from the quests,
	/// Not just the coupons
    /// As a pricing guide:
    /// <para/>1: Small reward item, eg. Relic Box
    /// <para/>2: Main reward item, eg. A weapon
    /// <para/>5: Major reward item, eg. Armour set
    /// </summary>
    public class BountyVoucher : ModItem
    {
        public static string itemName = "Expedition Coupon";
        public override void SetDefaults()
        {
            item.name = itemName;
            item.toolTip = "Trade in for exclusive items at certain stores";
            item.toolTip2 = "'A proof of achievement'";
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.value = 0;
            item.rare = -11;
        }
    }
}
