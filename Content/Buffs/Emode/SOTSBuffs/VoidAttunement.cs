using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VoidAttunement : ModBuff
    {
        public override string Texture => "SOTS/Buffs/VoidAccess";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
