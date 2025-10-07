using Fargowiltas.Content.Items.Ammos;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Conduit;
using SOTS.Items.Earth;
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
}
