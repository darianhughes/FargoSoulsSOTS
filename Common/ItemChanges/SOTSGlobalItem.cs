using System.Collections.Generic;
using Fargowiltas.Common.Configs;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using SOTS.Items;
using SOTS.Items.Earth.Glowmoth;
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
        }

        public static readonly int[] ALwyasUsableVanillaSummons = [ModContent.ItemType<SuspiciousLookingCandle>()];

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
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FlashsparkEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SubspaceBoosters>();
    }
}
