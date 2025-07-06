using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions144.Items
{
    public class BountyBoard : ModItem
	{
        public override void SetStaticDefaults()
        {
            /****DisplayName.SetDefault("Expeditions Board");
            Tooltip.SetDefault("View, track and complete expeditions");*/
        }

        public override void SetDefaults()
		{
            Item.width = 30;
			Item.height = 36;
			Item.maxStack = 99;

			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.BountyBoard>();

			Item.useStyle = 1;
            Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.value = Item.sellPrice(0, 0, 0, 20);
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(RecipeGroupID.Wood, 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
