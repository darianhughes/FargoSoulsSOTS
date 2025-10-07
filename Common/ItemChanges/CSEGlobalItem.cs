using System.Collections.Generic;
using ssm.Content.Items.Accessories;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargoSoulsSOTS.Common.ItemChanges
{
    [ExtendsFromMod(FargoSOTSCrossmod.CommunitySoulsExpansion.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.CommunitySoulsExpansion.Name)]
    public class CSEGlobalItem : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoSOTSConfig.Instance.UnfinishedContent;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<MicroverseSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateMicroverseSoul(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<StargateSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateEternitySoul(item, player, hideVisual);
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
            if (item.type == ModContent.ItemType<MicroverseSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    for (int i = 0; i < tooltips.Count; i++)
                    {
                        if (tooltips[i].Mod == "Terraria" && tooltips[i].Name.Contains("Tooltip0"))
                        {
                            tooltips[i].Text = $"{Language.GetTextValue("Mods.FargowiltasSouls.ActiveSkills.GrantsSkillsPlural")} {Language.GetTextValue("Mods.FargoSoulsSOTS.ActiveSkills.BloomStrike.DisplayName")}";
                            tooltips[i].OverrideColor = Color.Lerp(Color.Blue, Color.LightBlue, 0.7f);
                        }
                    }

                    if (FargoSOTSConfig.Instance.UnfinishedContent)
                    {
                        AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.ChaosForce.SoulTooltip"));
                        AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.SpaceForce.SoulTooltip"));
                    }
                    else
                    {
                        AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.VoidForce.SoulTooltip"));
                    }
                }
            }
        }
    }
}
