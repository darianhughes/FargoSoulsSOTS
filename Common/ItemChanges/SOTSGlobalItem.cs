using System.Collections.Generic;
using Fargowiltas.Common.Configs;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using SOTS;
using SOTS.Items;
using SOTS.Items.DoorItems;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Fragments;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.Planetarium;
using SOTS.Items.Slime;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ModContent.ItemType<SubspaceBoosters>())
            {
                //FargoSets.Items.SquirrelSellsDirectly[item.type] = true;
            }
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if ((item.type == ModContent.ItemType<SuspiciousLookingCandle>() && !SOTSWorld.downedGlowmoth) ||
                ((item.type == ModContent.ItemType<JarOfPeanuts>() || item.type == ModContent.ItemType<OffbrandPeanuts>()) && !SOTSWorld.downedPinky) ||
                (item.type == ModContent.ItemType<OldCRTTV>() && !SOTSWorld.downedAdvisor) ||
                (item.type == ModContent.ItemType<PolarKey>() && !SOTSWorld.downedAmalgamation) ||
                (item.type == ModContent.ItemType<ChaosLure>() && !SOTSWorld.downedLux) ||
                (item.type == ModContent.ItemType<CatalystDynamite>() && !SOTSWorld.downedSubspace)
                )
            {
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            var dp = player.GetModPlayer<DissolvingElementsPlayer>();

            if (dp.DissolvingDeluge != 0 && dp.PolarizeDeluge && player.HasEffect<ElementalEffect>() && item.CountsAsClass(DamageClass.Ranged))
            {
                velocity *= dp.DissolvingDeluge * 0.01f;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<FlashsparkBoots>() && SOTSItemConfig.Instance.FlashsparkBootsRework)
            {
                player.waterWalk = false;
                player.fireWalk = false;
                player.iceSkate = false;
                player.moveSpeed -= 0.2f;

                player.hellfireTreads = true;

                //togglable flashspark effect
                //player.AddEffect<FlashsparkEffect>(item);
            }

            if (item.type == ModContent.ItemType<SubspaceBoosters>())
            {
                //amph boot
                player.AddEffect<MasoAeolusFrog>(item);

                player.AddEffect<MasoAeolusFlower>(item);
                player.AddEffect<ZephyrJump>(item);

                //dunerider boot
                player.desertBoots = true;

                player.jumpBoost = true;
                player.noFallDmg = true;

                //hellfire treads
                if (SOTSItemConfig.Instance.FlashsparkBootsRework)
                    player.hellfireTreads = true;
            }

            if (item.type == ModContent.ItemType<FortressGenerator>() && SOTSItemConfig.Instance.FortressGeneratorRework)
            {
                player.noKnockback = false;
                player.hasPaladinShield = false;
            }
        }

        public static readonly int[] AlwyasUsableSOTSSummons = [ModContent.ItemType<SuspiciousLookingCandle>()];

        public void FullTooltipOveride(List<TooltipLine> tooltips, string stealthTooltip)
        {
            for (int index = 0; index < tooltips.Count; ++index)
            {
                if (tooltips[index].Mod == "Terraria")
                {
                    if (tooltips[index].Name == "Tooltip0")
                    {
                        TooltipLine tooltip = tooltips[index];
                        tooltip.Text = $"{stealthTooltip}";
                    }
                    else if (tooltips[index].Name.Contains("Tooltip"))
                    {
                        tooltips[index].Hide();
                    }
                }
            }
        }

        public void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip)
        {
            int maxTooltipIndex = -1;
            int maxNumber = -1;

            // Find the TooltipLine with the highest TooltipX name
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.StartsWith("Tooltip"))
                {
                    if (int.TryParse(tooltips[i].Name.Substring(7), out int num) && num > maxNumber)
                    {
                        maxNumber = num;
                        maxTooltipIndex = i;
                    }
                }
            }

            // If found, insert a new TooltipLine right after it with the desired color
            if (maxTooltipIndex != -1)
            {
                int insertIndex = maxTooltipIndex + 1;
                TooltipLine customLine = new TooltipLine(Mod, "StealthTooltip", stealthTooltip);
                tooltips.Insert(insertIndex, customLine);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<SuspiciousLookingCandle>() && ModContent.GetInstance<FargoServerConfig>().EasySummons)
            {
                FullTooltipOveride(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.TooltipOverride.SuspiciousLookingCandle"));
            }

            if (item.type == ModContent.ItemType<FlashsparkBoots>() && SOTSItemConfig.Instance.FlashsparkBootsRework)
            {
                FullTooltipOveride(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.TooltipOverride.FlashsparkBoots"));
            }

            if (item.type == ModContent.ItemType<SubspaceBoosters>())
            {
                if (SOTSItemConfig.Instance.FlashsparkBootsRework)
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.HellfireTreads"));

                AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.AeolusBoots"));
            }

            if (item.type == ModContent.ItemType<FortressGenerator>() && SOTSItemConfig.Instance.FortressGeneratorRework)
            {
                FullTooltipOveride(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.TooltipOverride.FortressGenerator"));
            }
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FlashsparkEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SubspaceBoosters>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class GravityAnchorEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<FlightMasteryHeader>();
        public override int ToggleItemType => ModContent.ItemType<GravityAnchor>();

        public override void PostUpdateEquips(Player player)
        {
            if (player.gravity < Player.defaultGravity)
                player.gravity = Player.defaultGravity;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class TwilightFishingEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TrawlerHeader>();
        public override int ToggleItemType => ModContent.ItemType<TwilightFishingPole>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class BandofDoorEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ModContent.ItemType<BandOfDoor>();

        public override void PostUpdateEquips(Player player)
        {
            DoorPlayer dp = player.GetModPlayer<DoorPlayer>();
            ++dp.doorPants;
        }
    }
}
