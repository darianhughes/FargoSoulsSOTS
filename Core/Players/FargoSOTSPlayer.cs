using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Content.Items.ForceofVoid;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Projectiles.Nature;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Players
{
    public class FargoSOTSPlayer : ModPlayer
    {
        public int BloomTimeLeft;
        public bool BloomReduced;
        public int CullCountPending;

        public override void PostUpdate()
        {
            if (BloomTimeLeft > 0)
            {
                BloomTimeLeft--;
                if (BloomTimeLeft == 0)
                {
                    // Ability ended: cull minions now.
                    if (CullCountPending > 0)
                        DespawnOwnedMinions(CullCountPending);
                    CullCountPending = 0;
                }
            }

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

        private void DespawnOwnedMinions(int countToCull)
        {
            // Prefer to remove newest minions first.
            var ownedMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Player.whoAmI)
                {
                    if (p.type == ModContent.ProjectileType<BloomingHook>())
                        p.Kill();
                    if (p.minion && p.minionSlots > 0f)
                        ownedMinions.Add(p);
                }
            }

            if (ownedMinions.Count <= 2)
                return;

            foreach (var p in ownedMinions
                .OrderByDescending(p => p.timeLeft) // “newest” heuristic
                .Take(Math.Min(countToCull, ownedMinions.Count - 2)))
            {
                p.Kill();
            }
        }
    }
}
