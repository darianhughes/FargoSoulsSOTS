using FargoSoulsSOTS.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    public class SpaceForceHeader : SoulHeader
    {
        public override float Priority => 0.92f;
        public override int Item => ModContent.ItemType<SpaceForce>();
    }
}
