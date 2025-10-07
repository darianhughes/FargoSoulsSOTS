using Fargowiltas.Content.Items.Ammos;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Conduit;
using SOTS.Items.Earth;
using SOTS.Items.SoldStuff;
using SOTS.Items.Temple;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Ammos.Bullets
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class BorePouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<BoreBullet>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]

    public class ScrapMetalPouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<AncientSteelBullet>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SolarPouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SolarBullet>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VibrantPouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<VibrantBullet>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class WormholePouch : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SkipBullet>();
    }
}
