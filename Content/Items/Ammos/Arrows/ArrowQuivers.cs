using Fargowiltas.Items.Ammos;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Conduit;
using SOTS.Items.Earth;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Ammos.Arrows
{
    public class ScrapMetalQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<AncientSteelArrow>();
    }

    public class VibrantQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<VibrantArrow>();
    }

    public class WormholeQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SkipArrow>();
    }
}
