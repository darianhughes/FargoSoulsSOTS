using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Nature;
using SOTS.Items.Slime;
using System.Collections.Generic;
using Terraria.Audio;
using SOTS.Projectiles.Nature;
using FargoSoulsSOTS.Core.Players;
using FargowiltasSouls;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Core.SoulToggles;
using FargoSoulsSOTS.Common.ProjectileChanges;

namespace FargoSoulsSOTS.Content.Items.Accessories.Enchantments
{
    public class WormwoodEnchant : BaseEnchant
    {
        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<BloomStrike>()];
        public override Color nameColor => new(100, 173, 255);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3, 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<WormwoodEffect>(Item);
            player.AddEffect<BloomStrike>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NatureWreath>()
                .AddIngredient<NatureShirt>()
                .AddIngredient<NatureLeggings>()
                .AddIngredient<BotanicalSymbiote>()
                .AddIngredient<WormWoodScepter>()
                .AddIngredient<WormWoodHook>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class WormwoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<WormwoodEnchant>();
        public override void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.minion)
            {
                target.AddBuff(BuffID.Slimed, 60 * 3);
            }
        }
    }

    public class BloomStrike : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;

        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (!stunned)
                ActivateBloomStrike(player);
        }

        public static void ActivateBloomStrike(Player player)
        {
            var mp = player.GetModPlayer<FargoSOTSPlayer>();
            if (player.HasBuff<BloomStrikeCooldown>() || mp.BloomTimeLeft > 0)
                return;

            // Must have at least one summon.
            var ownedMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.minion && p.minionSlots > 0f)
                    ownedMinions.Add(p);
            }
            if (ownedMinions.Count == 0)
                return;

            // Duration & post-ability cull rules.
            int baseDuration = 60 * 5;
            int duration = baseDuration + (player.ForceEffect<WormwoodEffect>() ? 60 * 10 : 0);
            int cull = 0;
            if (ownedMinions.Count > 2)
                cull = player.ForceEffect<WormwoodEffect>() ? 1 : 2;

            // Start ability state.
            mp.BloomTimeLeft = duration;
            mp.BloomReduced = false;
            mp.CullCountPending = cull;
            player.AddBuff(ModContent.BuffType<BloomStrikeCooldown>(), 60 * 25);

            // Spawn a BloomingHook on each minion (positioned at minion center).
            int hookType = ModContent.ProjectileType<BloomingHook>();
            int damage = 11;
            var source = player.GetSource_Misc("Wormwood:BloomStrike");
            foreach (var minion in ownedMinions)
            {
                int idx = Projectile.NewProjectile(
                    source,
                    minion.Center,
                    Vector2.Zero,
                    hookType,
                    damage,
                    1f,
                    player.whoAmI);

                if (idx >= 0 && idx < Main.maxProjectiles)
                {
                    var hook = Main.projectile[idx];
                    hook.damage = damage;
                    hook.originalDamage = damage;
                    hook.DamageType = DamageClass.Summon;
                    hook.timeLeft = 300;
                    hook.netUpdate = true;
                    hook.localAI[0] = minion.whoAmI + 1; // anchor to this minion
                    hook.GetGlobalProjectile<BloomHookMinionAnchor>().MarkAsWormwood();
                    hook.netUpdate = true;
                }
            }

            //CombatText.NewText(player.getRect(), new Color(140, 220, 140), "Bloom Strike!", dramatic: false);
            SoundEngine.PlaySound(SoundID.Item29 with { PitchVariance = 0.2f }, player.Center);
        }
    }
}
