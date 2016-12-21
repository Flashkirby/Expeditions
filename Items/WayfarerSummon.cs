using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    /// <summary>
    /// Summons 3 types of familiars:
    /// Fox guards the player's space
    /// Chicken chases enemies normally
    /// Cat attacks away from the player
    /// </summary>
    class WayfarerSummon : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.HornetStaff);
            item.name = "Wayfarer's Bell";
            item.toolTip = "Summons a familiar to fight for you";
            item.UseSound = SoundID.Item25;
            
            item.damage = 11;
            item.knockBack = 3f;
            item.shoot = mod.ProjectileType("MinionFox");

            // Create buff that manages the modPlayer's minion bool
            item.buffType = mod.BuffType("FamiliarMinion");
            item.buffTime = 3600;

            item.value = Item.buyPrice(0, 5, 0, 0);

            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 1;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int foxes = player.ownedProjectileCounts[mod.ProjectileType("MinionFox")];
            int chickens = player.ownedProjectileCounts[mod.ProjectileType("MinionChicken")];
            int cats = player.ownedProjectileCounts[mod.ProjectileType("MinionCat")];
            if (foxes > chickens)
            {
                type = mod.ProjectileType("MinionChicken");
            }
            else if (chickens > cats)
            {
                type = mod.ProjectileType("MinionCat");
            }
            position = Main.MouseWorld - new Vector2(12, 10);
            speedX = 0f;
            speedY = 0f;
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2();
        }
    }
}