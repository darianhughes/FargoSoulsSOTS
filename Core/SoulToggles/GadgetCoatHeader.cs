using FargoSoulsSOTS.Content.Items.Accessories.Masomode.SOTSEternity;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.SoulToggles
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class GadgetCoatHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<GadgetCoat>();
    }
}
