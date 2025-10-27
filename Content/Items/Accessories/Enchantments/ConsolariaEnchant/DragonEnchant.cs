using Consolaria.Content.Items.Armor.Melee;
using Consolaria.Content.Items.Pets;
using Consolaria.Content.Items.Weapons.Melee;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SecretsOfTheSouls.Content.Projectiles.Eternity.ConsolariaEternity;
using SecretsOfTheSouls.Core.SoulToggles.ConsolariaToggles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.ConsolariaEnchant
{
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class DragonEnchant : BaseEnchant
    {
        public override Color nameColor => new(100, 76, 140);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DragonEffect>(Item);
        }

        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Melee;
            tooltipColor = null;
            scaling = null;
            return (int)(125 * Main.LocalPlayer.ActualClassDamage(DamageClass.Melee));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DragonMask>()
                .AddIngredient<DragonBreastplate>()
                .AddIngredient<DragonGreaves>()
                .AddIngredient<AlbinoMandible>()
                .AddIngredient<Tizona>()
                .AddIngredient<Tonbogiri>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class DragonEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MightForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<DragonEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dmg = (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(125);
            ShootTripleShadowflames(player, dmg, item.knockBack);
        }

        public override void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.type == ModContent.ProjectileType<ShadowflameApparitionProj>())
                return;

            if (proj.CountsAsClass(DamageClass.SummonMeleeSpeed))
            {
                int dmg = (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(125);
                ShootTripleShadowflames(player, dmg, proj.knockBack);
            }
            else if (player.ForceEffect<DragonEffect>())
            {
                int dmg = (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(75);
                ShootTripleShadowflames(player, dmg, proj.knockBack);
            }
        }

        public static void ShootTripleShadowflames(Player player, int damage, float knockback, float speed = 12f, float spreadDeg = 12f)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ShadowflameApparitionProj>()] >= 3)
                return;

            Vector2 dir = (Main.MouseWorld - player.Center);
            if (dir.LengthSquared() < 0.001f) dir = Vector2.UnitX;
            dir.Normalize();

            float spread = MathHelper.ToRadians(spreadDeg);
            int type = ModContent.ProjectileType<ShadowflameApparitionProj>();

            for (int i = -1; i <= 1; i++)
            {
                Vector2 vel = dir.RotatedBy(spread * i) * speed;
                Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    player.Center,
                    vel,
                    type,
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            SoundEngine.PlaySound(SoundID.NPCDeath55, player.Center);
        }
    }
}
