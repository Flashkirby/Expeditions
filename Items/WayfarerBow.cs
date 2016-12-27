using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class WayfarerBow : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.GoldBow);
            item.name = "Wayfarer's Bow";

            item.damage = 10;
            item.useAnimation = 25;
            item.knockBack = 2f;
            item.shootSpeed = 6.6f;

            item.value = Item.sellPrice(0, 0, 25, 0);
        }
    }
}