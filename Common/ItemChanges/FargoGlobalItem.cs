using System.Collections.Generic;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargoSoulsSOTS.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
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
            Mod sots = ModLoader.GetMod("SOTS");

            if (item.type == ModContent.ItemType<AeolusBoots>())
            {
                /*
                player.accRunSpeed = 7f;

                player.lavaMax += 180; //total of 600

                player.AddEffect<FlashsparkEffect>(item);
                ModItem flashsparkBoots = sots.Find<ModItem>("FlashsparkBoots");
                if (player.HasEffect<FlashsparkEffect>())
                {
                    flashsparkBoots.UpdateAccessory(player, hideVisual);
                    player.lavaMax -= 600;
                    player.moveSpeed -= 0.2f;
                }
                if (ItemConfig.Instance.FlashsparkBootsRework)
                    player.hellfireTreads = true;
                */
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                player.AddEffect<EarthenEffect>(item);
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                player.AddEffect<EarthenEffect>(item);
            }

            if (item.type == ModContent.ItemType<TerrariaSoul>())
            {
                if (FargoSOTSConfig.Instance.UnfinishedContent)
                {
                    if (!FargoSOTSCrossmod.CommunitySoulsExpansion.Loaded)
                    {
                        ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                        ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
                    }
                }
                else
                {
                    ModContent.GetInstance<VoidForce>().UpdateAccessory(player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<EternitySoul>())
            {
                player.AddEffect<EarthenEffect>(item);

                if (FargoSOTSConfig.Instance.UnfinishedContent)
                {
                    ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
                }
                else
                {
                    ModContent.GetInstance<VoidForce>().UpdateAccessory(player, hideVisual);
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
                int insertIndex = maxTooltipIndex;
                TooltipLine customLine = new TooltipLine(Mod, "StealthTooltip", stealthTooltip);
                tooltips.Insert(insertIndex, customLine);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<AeolusBoots>())
            {
                if (ItemConfig.Instance.FlashsparkBootsRework)
                    AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.HellfireTreads"));

                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.FlashsparkBoots"));
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.EarthenEnchant.SimpleTooltip"));
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.EarthenEnchant.SimpleTooltip"));
            }

            if (item.type == ModContent.ItemType<TerrariaSoul>())
            {
                if (FargoSOTSConfig.Instance.UnfinishedContent)
                {
                    if (!FargoSOTSCrossmod.CommunitySoulsExpansion.Loaded)
                    {
                        tooltips[3].Text += ", ";
                        tooltips[3].Text += Language.GetTextValue("Mods.FargoSoulsSOTS.ActiveSkills.BloomStrike.DisplayName");

                        AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.ChaosForce.SoulTooltip"));
                        AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.SpcaeForce.SoulTooltip"));
                    }
                }
                else
                {
                    tooltips[3].Text += ", ";
                    tooltips[3].Text += Language.GetTextValue("Mods.FargoSoulsSOTS.ActiveSkills.BloomStrike.DisplayName");

                    AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.VoidForce.SoulTooltip"));
                }
            }
        }
    }
}
