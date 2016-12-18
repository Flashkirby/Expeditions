using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    class WayfarerSummon : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.HornetStaff);
            item.name = "Wayfarer's Bell";
            item.toolTip = "Summons a familiar to fight for you";
            item.UseSound = SoundID.Item34;
            
            item.damage = 11;
            item.knockBack = 3f;
            item.shoot = mod.ProjectileType("MinionFox");

            item.value = Item.buyPrice(0, 5, 0, 0);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }
    }
}