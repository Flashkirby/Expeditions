using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Buffs
{
    class FamiliarMinion : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffName[Type] = "Familiar";
            Main.buffTip[Type] = "The familiar will fight for you";
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        
        public override void Update(Player player, ref int buffIndex)
        {
            PlayerExplorer modPlayer = player.GetModPlayer<PlayerExplorer>(mod);
            int minionCount = 0;
            minionCount += player.ownedProjectileCounts[mod.ProjectileType("MinionFox")];
            minionCount += player.ownedProjectileCounts[mod.ProjectileType("MinionChicken")];
            minionCount += player.ownedProjectileCounts[mod.ProjectileType("MinionCat")];
            //minionCount += player.ownedProjectileCounts[mod.ProjectileType("MinionDeer")];
            if (minionCount > 0)
            {
                modPlayer.familiarMinion = true;
            }
            if (!modPlayer.familiarMinion)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
