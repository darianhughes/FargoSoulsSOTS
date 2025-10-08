using Consolaria.Content.Items.Weapons.Ammo;
using Fargowiltas.Content.Items.Ammos;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Conduit;
using SOTS.Items.Earth;
using ssm.Core;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Ammos.Arrows
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ScrapMetalQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<AncientSteelArrow>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VibrantQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<VibrantArrow>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class WormholeQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SkipArrow>();
    }

    [ExtendsFromMod(ModCompatibility.Consolaria.Name)]
    [JITWhenModsEnabled(ModCompatibility.Consolaria.Name)]
    public class HeartQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<HeartArrow>();
    }

    [ExtendsFromMod(ModCompatibility.Consolaria.Name)]
    [JITWhenModsEnabled(ModCompatibility.Consolaria.Name)]
    public class SpectralQuiver : BaseAmmo
    {
        public override int AmmunitionItem => ModContent.ItemType<SpectralArrow>();
    }
}
