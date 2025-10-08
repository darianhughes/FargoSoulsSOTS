using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SecretsOfTheSouls
{
    public class SecretsOfTheSoulsConfig : ModConfig
    {
        public static SecretsOfTheSoulsConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool UnfinishedContent { get; set; }
    }
}
