using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    class WayfarerRepeater : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Musket);
            item.name = "Wayfarer's Repeater";
            item.width = 46;
            item.height = 20;

            item.damage = 30;
            item.useAnimation = 40;
            item.useTime = 40;
            item.knockBack = 6.75f;
            item.shootSpeed = 11f;

            item.value = Item.sellPrice(0, 2, 0, 0);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }
    }
}