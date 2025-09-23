using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace FargoSoulsSOTS
{
    public class ItemConfig : ModConfig
    {
        public static ItemConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("ItemReworks")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool FlashsparkBootsRework { get; set; }
    }
}
