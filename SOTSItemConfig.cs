using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SecretsOfTheSouls
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSItemConfig : ModConfig
    {
        public static SOTSItemConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("ItemReworks")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool FlashsparkBootsRework { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool FortressGeneratorRework { get; set; }
    }
}
