using FargowiltasSouls.Core.Toggler.Content;
using SecretsOfTheSouls.Content.Items.Accessories.Souls.SOTSSoul;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.SoulToggles.SOTSToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SigiloftheShadowsHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<SigiloftheShadows>();
    }
}
