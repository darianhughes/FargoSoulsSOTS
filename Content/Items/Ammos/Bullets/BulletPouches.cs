using Fargowiltas.Items.Ammos;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Conduit;
using SOTS.Items.Earth;
using SOTS.Items.SoldStuff;
using SOTS.Items.Temple;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Ammos.Bullets
{
    public class BorePouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<BoreBullet>();
    }

    public class ScrapMetalPouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<AncientSteelBullet>();
    }

    public class SolarPouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SolarBullet>();
    }

    public class VibrantPouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<VibrantBullet>();
    }

    public class WormholePouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SkipBullet>();
    }
}
