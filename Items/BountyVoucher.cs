using Terraria.ModLoader;

namespace Expeditions.Items
{
    /// <summary>
    /// Used in shops that require this as a special currency. 
    /// Expeditions should only reward up to 3 of these.
    /// As a pricing guide:
    /// <para/>1: Small reward item, eg. Relic Box
    /// <para/>2: Main reward item, eg. A weapon
    /// <para/>5: Major reward item, eg. Armour set
    /// </summary>
    public class BountyVoucher : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Expedition Coupon";
            item.toolTip = "'A proof of achievement'";
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.rare = 3;
        }
    }
}
