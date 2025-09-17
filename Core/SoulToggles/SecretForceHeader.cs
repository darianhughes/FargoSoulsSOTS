using FargoSoulsSOTS.Content.Items.ForceofSecrets;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    public class SecretForceHeader : SoulHeader
    {
        public override float Priority => 0.91f;
        public override int Item => ModContent.ItemType<SecretForce>();
    }
}
