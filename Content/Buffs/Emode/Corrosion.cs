using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargoSoulsSOTS.Core.Players;

namespace FargoSoulsSOTS.Content.Buffs.Emode
{
    public class Corrosion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSOTSPlayer mp = player.GetModPlayer<FargoSOTSPlayer>();

            mp.debuffCorrosion = true;
        }
    }
}
