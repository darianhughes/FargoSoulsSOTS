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
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;
using Terraria.Localization;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ElementalEnchant : BaseEnchant
    {
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
            player.AddEffect<PolarizerEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ElementalHelmet>()
                .AddIngredient<ElementalBreastplate>()
                .AddIngredient<ElementalLeggings>()
                .AddIngredient<SOTS.Items.Chaos.RealityShatter>()
                .AddIngredient<EtherealScepter>()
                .AddIngredient<UltimatePolarizer>()
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
        public override int ToggleItemType => ModContent.ItemType<ElementalEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            var dissolvingPlayer = player.GetModPlayer<DissolvingElementsPlayer>();
            if (!dissolvingPlayer.PolarizeNature)
                dissolvingPlayer.DissolvingNature = 0;
            if (!dissolvingPlayer.PolarizeEarth)
                dissolvingPlayer.DissolvingEarth = 0;
            if (!dissolvingPlayer.PolarizeAurora)
                dissolvingPlayer.DissolvingAurora = 0;
            if (!dissolvingPlayer.PolarizeAether)
                dissolvingPlayer.DissolvingAether = 0;
            if (!dissolvingPlayer.PolarizeDeluge)
                dissolvingPlayer.DissolvingDeluge = 0;
            if (!dissolvingPlayer.PolarizeUmbra)
                dissolvingPlayer.DissolvingUmbra = 0;
            if (!dissolvingPlayer.PolarizeNether)
                dissolvingPlayer.DissolvingNether = 0;
            if (!dissolvingPlayer.PolarizeBrilliance)
                dissolvingPlayer.DissolvingBrilliance = 0;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PolarizerEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<UltimatePolarizer>();

        public override void UpdateBadLifeRegen(Player player)
        {
            var dissolvingPlayer = player.GetModPlayer<DissolvingElementsPlayer>();
            bool hasForce = Main.LocalPlayer.ForceEffect<PolarizerEffect>();

            if (dissolvingPlayer.PolarizeNether)
            {
                player.GetAttackSpeed(DamageClass.Melee) += dissolvingPlayer.DissolvingNether * 0.02f;
                if (hasForce)
                {
                    ref StatModifier local = ref player.GetDamage(DamageClass.Melee);
                    local += dissolvingPlayer.DissolvingNether * 0.02f;
                }
            }
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class  ElementalPolarizerEnhancementTooltips : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            DissolvingElementsPlayer dissolvingElementsPlayer = DissolvingElementsPlayer.ModPlayer(Main.LocalPlayer);
            bool hasEffect = Main.LocalPlayer.HasEffect<PolarizerEffect>();
            bool hasForce = Main.LocalPlayer.ForceEffect<PolarizerEffect>();

            if (hasEffect)
            {
                if (item.type == ModContent.ItemType<DissolvingNether>() & dissolvingElementsPlayer.PolarizeNether)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingNetherFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingNetherElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingNetherElemental")}";
                        }
                    }
                }
                if (item.type == ModContent.ItemType<DissolvingAether>() & dissolvingElementsPlayer.PolarizeAether)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingAetherFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingAetherElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingAetherElemental")}";
                        }
                    }

                }
                if (item.type == ModContent.ItemType<DissolvingNature>() & dissolvingElementsPlayer.PolarizeNature)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingNatureFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingNatureElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingNatureElemental")}";
                        }
                    }

                }
                if (item.type == ModContent.ItemType<DissolvingEarth>() & dissolvingElementsPlayer.PolarizeEarth)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingEarthFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingEarthElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingEarthElemental")}";
                        }
                    }
                }
                if (item.type == ModContent.ItemType<DissolvingAurora>() & dissolvingElementsPlayer.PolarizeAurora)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingAuroraFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingAuroraElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingAuroraElemental")}";
                        }
                    }
                }
                if (item.type == ModContent.ItemType<DissolvingDeluge>() & dissolvingElementsPlayer.PolarizeDeluge)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingDelugeFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingDelugeElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingDelugeElemental")}";
                        }
                    }
                }
                if (item.type == ModContent.ItemType<DissolvingBrilliance>() & dissolvingElementsPlayer.PolarizeBrilliance)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingBrillianceFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingBrillianceElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingBrillianceElemental")}";
                        }
                    }
                }
                if (item.type == ModContent.ItemType<DissolvingUmbra>() & dissolvingElementsPlayer.PolarizeUmbra)
                {
                    foreach (TooltipLine tooltip in tooltips)
                    {
                        if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
                        {
                            if (hasForce)
                                tooltip.Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingUmbraFlipped")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingUmbraElemental")}";
                            else
                                tooltip.Text += $"\n{Language.GetTextValue("Mods.SecretsOfTheSouls.DissolvingElements.DissolvingUmbraElemental")}";
                        }
                    }
                }
            }
        }
    }
}