using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOTS.Void;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Buffs
{
    public class VoidEmpowerment : ModBuff
    {
        public override string Texture => "SOTS/Buffs/VoidAccess";
        public override void Update(Player player, ref int buffIndex)
        {
            VoidPlayer mp = player.GetModPlayer<VoidPlayer>();
            mp.voidMeterMax2 += 15;
            player.GetDamage<VoidGeneric>() += 0.15f;
        }
    }
}
