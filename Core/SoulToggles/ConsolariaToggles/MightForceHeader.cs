using FargowiltasSouls.Core.Toggler.Content;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.ConsolariaForce;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.SoulToggles.ConsolariaToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class MightForceHeader : SoulHeader
    {
        public override float Priority => 0.93f;
        public override int Item => ModContent.ItemType<MightForce>();
    }
}
