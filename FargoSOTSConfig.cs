using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
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
