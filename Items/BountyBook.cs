using Terraria;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class BountyBook : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Expedition Log";
            item.toolTip = "Manage and track expeditions";
            item.toolTip2 = "'The joys of discovery!'";
            item.width = 30;
            item.height = 36;
            item.maxStack = 1;

            item.useStyle = 4; // holding up
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 15;

            item.rare = 1;
            item.value = Item.buyPrice(0, 0, 20, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return !ExpeditionUI.visible;
        }

        public override bool UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Expeditions.OpenExpeditionMenu(ExpeditionUI.viewMode_Menu);
            }
            return true;
        }
    }
}
