using FargoSoulsSOTS.Content.Items.ForceofVoid;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    public class VoidForceHeader : SoulHeader
    {
        public override float Priority => 0.92f;
        public override int Item => ModContent.ItemType<VoidForce>();
    }
}
