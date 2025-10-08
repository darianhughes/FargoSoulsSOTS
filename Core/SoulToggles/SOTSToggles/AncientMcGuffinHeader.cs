using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.SoulToggles.SOTSToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class AncientMcGuffinHeader : SoulHeader
    {
        public override int Item => -1;
    }
}
