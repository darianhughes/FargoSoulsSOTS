using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.SoulToggles.SOTSToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ChaosForceHeader : SoulHeader
    {
        public override float Priority => 0.91f;
        public override int Item => ModContent.ItemType<ChaosForce>();
    }
}
