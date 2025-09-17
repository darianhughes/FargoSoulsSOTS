using FargoSoulsSOTS.Content.Items.ForceofChaos;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    public class ChaosForceHeader : SoulHeader
    {
        public override float Priority => 0.91f;
        public override int Item => ModContent.ItemType<ChaosForce>();
    }
}
