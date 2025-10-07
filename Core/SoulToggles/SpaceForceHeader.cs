using FargoSoulsSOTS.Content.Items.Accessories.Forces.SOTSForce;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class SpaceForceHeader : SoulHeader
    {
        public override float Priority => 0.92f;
        public override int Item => ModContent.ItemType<SpaceForce>();
    }
}
