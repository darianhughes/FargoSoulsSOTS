using FargoSoulsSOTS.Content.Items.Accessories.Forces.SOTSForce;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class ChaosForceHeader : SoulHeader
    {
        public override float Priority => 0.91f;
        public override int Item => ModContent.ItemType<ChaosForce>();
    }
}
