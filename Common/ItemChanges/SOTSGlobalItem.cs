using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using SOTS;
using SOTS.Items;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.ItemChanges
{
    public class SOTSGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<FlashsparkBoots>() && ItemConfig.Instance.FlashsparkBootsRework)
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
                if (ItemConfig.Instance.FlashsparkBootsRework)
                    player.hellfireTreads = true;

            }
        }

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
            if (item.type == ModContent.ItemType<FlashsparkBoots>() && ItemConfig.Instance.FlashsparkBootsRework)
            {
                FullTooltipOveride(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.TooltipOverride.FlashsparkBoots"));
            }

            if (item.type == ModContent.ItemType<SubspaceBoosters>())
            {
                if (ItemConfig.Instance.FlashsparkBootsRework)
                    AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.HellfireTreads"));

                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.AeolusBoots"));
            }
        }
    }

    public class FlashsparkEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SubspaceBoosters>();
    }
}
