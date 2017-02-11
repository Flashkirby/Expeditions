using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class StockBox : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Rusted Box";
            item.width = 20;
            item.height = 20;
            item.toolTip = "Right click to open";
            item.toolTip2 = "'Its contents, a mystery...'";
            item.maxStack = 30;
            item.rare = 1;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int rare = ItemRewardPool.GetRewardRare(player);
            if (rare > 3) rare = 3;
            try
            {
                foreach (ItemRewardData i in ItemRewardPool.GenerateFullRewards(rare))
                {
                    player.QuickSpawnItem(i.itemID, i.stack);
                }
            }
            catch (System.Exception e)
            {
                //Main.NewTextMultiline(e.ToString());
                player.QuickSpawnItem(ItemID.IronCrate);
            }
        }
    }
}
