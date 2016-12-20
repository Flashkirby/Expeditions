using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Expeditions.Buffs
{
    class FamiliarMinion : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffName[Type] = "Wayfarer's Familiars";
            Main.buffTip[Type] = "The familiar will fight for you";
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        
        public override void Update(Player player, ref int buffIndex)
        {
            PlayerExplorer modPlayer = player.GetModPlayer<PlayerExplorer>(mod);
            if (player.ownedProjectileCounts[mod.ProjectileType("MinionFox")] > 0)
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
