using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Items
{
    public class StockBox2 : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Relic Box";
            item.width = 20;
            item.height = 24;
            item.toolTip = "Right click to open";
            item.toolTip2 = "'Its contents, an enigma...'";
            item.maxStack = 30;
            item.rare = 4;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int rare = ItemRewardPool.GetRewardRare(player);
            if (rare < 3) rare = 3;
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
                player.QuickSpawnItem(ItemID.GoldenCrate);
            }
        }
    }
}

/*
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

        // See StockBox
        int minRare = 1;
        if (maxRare < minRare) maxRare = minRare;

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
*/
