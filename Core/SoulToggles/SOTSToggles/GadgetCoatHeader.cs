using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;
using SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity;

namespace SecretsOfTheSouls.Core.SoulToggles.SOTSToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class GadgetCoatHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<GadgetCoat>();
    }
}
