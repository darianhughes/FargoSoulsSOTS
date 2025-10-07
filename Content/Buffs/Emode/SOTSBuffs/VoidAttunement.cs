using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Buffs.Emode.SOTSBuffs
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class VoidAttunement : ModBuff
    {
        public override string Texture => "SOTS/Buffs/VoidAccess";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
