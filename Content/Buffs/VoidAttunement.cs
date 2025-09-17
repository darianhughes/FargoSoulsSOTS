using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargoSoulsSOTS.Content.Items.ForceofVoid;
using FargowiltasSouls;
using SOTS.Void;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Buffs
{
    public class VoidAttunement : ModBuff
    {
        public override string Texture => "SOTS/Buffs/VoidAccess";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
