using System.Collections.Generic;
using ssm.Content.Items.Accessories;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Name)]
    public class CSEGlobalItem : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
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
                CrossmodAdditions.UpdateEternitySoul(item, player, hideVisual);
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
                            tooltips[i].Text = $"{Language.GetTextValue("Mods.SecretsOfTheSouls.ActiveSkills.GrantsSkillsPlural")} {Language.GetTextValue("Mods.SecretsOfTheSouls.ActiveSkills.BloomStrike.DisplayName")}";
                            tooltips[i].OverrideColor = Color.Lerp(Color.Blue, Color.LightBlue, 0.7f);
                        }
                    }

                    if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
                    {
                        AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.ChaosForce.SoulTooltip"));
                        AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.SpaceForce.SoulTooltip"));
                    }
                    else
                    {
                        AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.VoidForce.SoulTooltip"));
                    }
                }
            }
        }
    }
}
