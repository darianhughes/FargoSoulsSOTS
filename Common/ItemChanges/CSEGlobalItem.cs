using System.Collections.Generic;
using FargoSoulsSOTS.Content.Items.Accessories.Forces;
using ssm.Content.Items.Accessories;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

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

            /*
            if (item.type == ModContent.ItemType<StargateSoul>())
            {
                ++dp.doorPants;
 
                if (player.AddEffect<FlashsparkEffect>(item))
                {
                    ModItem sb = sots.Find<ModItem>("SubspaceBoosters");

                    sb.UpdateAccessory(player, hideVisual);

                    //remove extra things added
                    player.lavaMax -= 600;
                    if (player.HasEffect<SupersonicRocketBoots>())
                        player.rocketBoots = player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
                    else
                    {
                        player.rocketBoots = 0;
                    }
                    player.moveSpeed -= 0.2f;
                    player.accRunSpeed = player.HasEffect<RunSpeed>() ? 15.6f : 6.75f;
                }

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
            */
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
                tooltips[3].Text += ", ";
                tooltips[3].Text += Language.GetTextValue("Mods.FargoSoulsSOTS.ActiveSkills.BloomStrike.DisplayName");

                if (FargoSOTSConfig.Instance.UnfinishedContent)
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.ChaosForce.SoulTooltip"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.SpcaeForce.SoulTooltip"));
                }
                else
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.VoidForce.SoulTooltip"));
                }
            }
        }
    }
}
