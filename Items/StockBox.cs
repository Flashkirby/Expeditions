using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    class StockBox : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Rusted Box";
            item.width = 20;
            item.height = 20;
            item.toolTip = "Right click to open";
            item.toolTip2 = "'Its contents, a mystery...'";
            item.rare = 1;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int failCount = 0;
            while (failCount < 200)
            {
                try
                {
                    int maxRare = 1;
                    foreach (Item i in player.inventory)
                    {
                        Item defs = new Item();
                        defs.SetDefaults(i.type);
                        if (defs.rare > maxRare) maxRare = defs.rare;
                    }
                    maxRare--;

                    // See StockBox2
                    int minRare = 0;
                    if (minRare > maxRare) minRare = maxRare;

                    Item item = new Item();
                    item.SetDefaults(Main.rand.Next(Main.itemTexture.Length));
                    if (item.rare > maxRare ||
                        item.rare < minRare ||
                        (item.expert && !Main.expertMode))
                    {
                        failCount++;
                    }
                    else
                    {
                        if (Expeditions.DEBUG) Main.NewText("<Box> (" + item.rare + "/" + maxRare + ") " + item.name + " : " + item.type,
                            UI.UIColour.GetColourFromRarity(item.rare).R,
                            UI.UIColour.GetColourFromRarity(item.rare).G,
                            UI.UIColour.GetColourFromRarity(item.rare).B);
                        player.QuickSpawnItem(item.type);
                        failCount = 0;
                        break;
                    }
                }
                catch { failCount++; }
            }
            if (failCount > 0)
            {
                player.QuickSpawnItem(ItemID.GoldCoin, Main.rand.Next(5,11));
                player.QuickSpawnItem(ItemID.WoodenCrate, 1);
            }
        }
    }
}
