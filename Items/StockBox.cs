using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions144.Items
{
    public class StockBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Rusted Box");
            Tooltip.SetDefault("Right click to open\n"
              + "'Its contents, a mystery...'");*/
        }

        public override void SetDefaults()
        {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 30;
			Item.rare = 1;
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
                    player.QuickSpawnItem(player.GetSource_OpenItem(Type), i.itemID, i.stack);
                }
            }
            catch (System.Exception e)
            {
                //Main.NewTextMultiline(e.ToString());
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.IronCrate);
            }
        }
    }
}
