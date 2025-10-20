using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using SecretsOfTheSouls.Core.Players;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Buffs;
using SOTS.Projectiles.Earth;
using SOTS.Projectiles.Nature;
using SOTS.Void;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.Effects.SOTSEffects
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSGlobalProjectileEffects : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];

            if (projectile.type == ModContent.ProjectileType<VibrantBolt>() && projectile.ai[0] == 1)
            {
                if (player.HasEffect<VibrantEffect>())
                {
                    if (player.HeldItem.ModItem is not VoidItem)
                    {
                        damageDone = (int)(damageDone * (player.ForceEffect<VibrantEffect>() ? 0.8f : 0.5f));
                        player.AddBuff(ModContent.BuffType<VoidBurn>(), 60 * 5);
                    }
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<BloomingHook>())
            {
                Player owner = Main.player[projectile.owner];
                var mp = owner.GetModPlayer<SOTSEffectsPlayer>();

                if (mp.BloomTimeLeft > 0)
                    projectile.timeLeft = 300;

                projectile.damage = mp.BloomReduced ? 5 : 11;
                projectile.originalDamage = projectile.damage;
                projectile.DamageType = DamageClass.Summon;
            }
        }
    }
}
