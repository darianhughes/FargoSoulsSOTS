using System.Collections.Generic;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.ItemChanges
{
    public class FargoGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                player.AddEffect<EarthenEffect>(item);
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                player.AddEffect<EarthenEffect>(item);
            }

            if (item.type == ModContent.ItemType<EternitySoul>())
            {
                player.AddEffect<EarthenEffect>(item);
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
            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.EarthenEnchant.SimpleTooltip"));
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.EarthenEnchant.SimpleTooltip"));
            }
        }
    }
}
