using Terraria;
using Terraria.ModLoader;

namespace Expeditions144.Items
{
    public class BountyBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            /***DisplayName.SetDefault("Expedition Log");
            Tooltip.SetDefault("Manage and track expeditions\n"
              + "'The joys of discovery!'");*/
        }

        public override void SetDefaults()
        {
            Item.width = 30;
			Item.height = 36;
			Item.maxStack = 1;

			Item.useStyle = 4; // holding up
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 15;

			Item.rare = 1;
			Item.value = Item.buyPrice(0, 0, 20, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return !ExpeditionUI.visible;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ItemAnimationJustStarted) // **** added "player.ItemAnimationJustStarted" because it was needed, for some reason
			{
                Expeditions144.OpenExpeditionMenu(ExpeditionUI.viewMode_Menu);
            }
            return null;
        }
    }
}
