using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Content.Items.ForceofVoid;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Void;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Players
{
    public class FargoSOTSPlayer : ModPlayer
    {
        //ENCHANTMENTS
        //public bool VesperaEnchantment;
        public override void PostUpdate()
        {
            VoidPlayer mp = VoidPlayer.ModPlayer(Player);

            if (Player.HasBuff(BuffID.PotionSickness))
            {
                if (Player.HasEffect<VesperaEffect>())
                {
                    float voidRegenBonus = Player.ForceEffect<VesperaEffect>() ? 0.05f : 0.15f;
                    mp.voidRegenSpeed += voidRegenBonus;
                }
            }
            if (Player.HasBuff<VoidAttunement>())
            {
                int voidBonus = Player.ForceEffect<VesperaEffect>() ? 50 : 25;
                mp.voidMeterMax2 += voidBonus;
            }
        }
    }
}
