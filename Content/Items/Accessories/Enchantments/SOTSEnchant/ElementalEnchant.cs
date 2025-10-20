using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Chaos;
using SOTS.Items.Fragments;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls;
using SecretsOfTheSouls.Core.Players;
using Fargowiltas.Content.Items.Tiles;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using System.Collections.Generic;
using SOTS.Void;
using SOTS.Projectiles.Chaos;
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ElementalEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }
        //public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<ChaosTeleport>()];
        public override Color nameColor => new(231, 95, 203);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.AddEffect<ChaosTeleport>(Item);
            player.AddEffect<ElementalEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ElementalHelmet>()
                .AddIngredient<ElementalBreastplate>()
                .AddIngredient<ElementalLeggings>()
                .AddIngredient<SOTS.Items.Chaos.HyperlightGeyser>()
                .AddIngredient<SOTS.Items.Chaos.RealityShatter>()
                .AddIngredient<EtherealScepter>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ChaosTeleport : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override bool ActiveSkill => true;
        public override Header ToggleHeader => null;
        private int Cooldown;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            var FargoSOTSPlayer = player.GetModPlayer<SOTSEffectsPlayer>();
            bool flag = !player.mount.Active && player.grappling[0] < 0 && !player.frozen && !player.CCed && !player.dead;

            if (!stunned && FargoSOTSPlayer.ChaosCharge == 0 && (flag ? 1 : 0) != 0 && player.whoAmI == Main.myPlayer)
            {
                Vector2 mousePos = Main.MouseWorld - new Vector2(0f, player.height * 0.5f);
                Vector2 dir = mousePos - player.Center;

                int dmg = (int)player.GetTotalDamage(ModContent.GetInstance<VoidGeneric>()).ApplyTo(80);

                Projectile.NewProjectile(player.GetSource_Misc("SotSouls:Blink"), player.Center, Utils.SafeNormalize(dir, Vector2.Zero), ModContent.ProjectileType<ElementalRelocatorBeam>(), dmg, 0f, player.whoAmI, mousePos.X, mousePos.Y, player.ForceEffect<ChaosTeleport>() ? 25 : 15);

                FargoSOTSPlayer.ChaosCharge = Cooldown;
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            var FargoSOTSPlayer = player.GetModPlayer<SOTSEffectsPlayer>();
            Cooldown = player.ForceEffect<ChaosTeleport>() ? 60 * 40 : 60 * 25;
            CooldownBarManager.Activate("ChaosTeleport", ModContent.Request<Texture2D>("SecretsOfTheSouls/Assets/Textures/Content/Items/Accessories/Enchantments/ElementalEnchant").Value, new(231, 95, 203),
                () => 1 - (float)FargoSOTSPlayer.ChaosCharge / Cooldown, displayAtFull: false, activeFunction: player.HasEffect<ChaosTeleport>);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ElementalEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override void PostUpdateEquips(Player player)
        {
            var dissolvingPlayer = player.GetModPlayer<DissolvingElementsPlayer>();
            dissolvingPlayer.DissolvingNature = 0;
            dissolvingPlayer.DissolvingEarth = 0;
            dissolvingPlayer.DissolvingAurora = 0;
            dissolvingPlayer.DissolvingAether = 0;
            dissolvingPlayer.DissolvingDeluge = 0;
            dissolvingPlayer.DissolvingAurora = 0;
            dissolvingPlayer.DissolvingUmbra = 0;
            dissolvingPlayer.DissolvingNether = 0;
            dissolvingPlayer.DissolvingBrilliance = 0;
        }
    }
}