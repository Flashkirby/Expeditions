using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
	class BountyBoard : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Expeditions Board";
			item.toolTip = "Manage tracking on your expeditions";
            //item.toolTip2 = "Allows the Clerk to move in";
            item.width = 30;
            item.height = 36;
            item.maxStack = 99;

            item.consumable = true;
            item.createTile = mod.TileType("BountyBoard");

            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.value = 100;
        }
        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wood, 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
