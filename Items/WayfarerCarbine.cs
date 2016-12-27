using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class WayfarerCarbine : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheUndertaker);
            item.name = "Wayfarer's Carbine";
            item.width = 36;
            item.height = 18;

            item.damage = 16;
            item.useAnimation = 24;
            item.useTime = 24;
            item.knockBack = 2f;
            item.shootSpeed = 7f;

            item.value = Item.buyPrice(0, 3, 0, 0);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }
    }
}