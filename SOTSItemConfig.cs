using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargoSoulsSOTS
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class SOTSItemConfig : ModConfig
    {
        public static SOTSItemConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("ItemReworks")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool FlashsparkBootsRework { get; set; }
    }
}
