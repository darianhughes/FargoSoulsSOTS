using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace FargoSoulsSOTS
{
    public class FargoSOTSConfig : ModConfig
    {
        public static FargoSOTSConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool UnfinishedContent { get; set; }
    }
}
