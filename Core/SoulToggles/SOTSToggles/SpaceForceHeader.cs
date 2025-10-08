using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.SoulToggles.SOTSToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SpaceForceHeader : SoulHeader
    {
        public override float Priority => 0.92f;
        public override int Item => ModContent.ItemType<SpaceForce>();
    }
}
