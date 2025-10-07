using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using SecretsOfTheSouls.Core.Players;

namespace SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
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
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            mp.debuffCorrosion = true;
        }
    }
}
