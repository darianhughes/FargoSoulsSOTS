using System.Collections.Generic;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using SOTS.Items.Planetarium;
using SOTS.Items.Planetarium.FromChests;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using SOTS.Items.DoorItems;
using System.Runtime.Intrinsics.Arm;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    public class FargoGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<SupersonicSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateSupersonic(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    player.AddEffect<EarthenEffect>(item);
                }
            }

            if (item.type == ModContent.ItemType<FlightMasterySoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateFlightMastery(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<TrawlerSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    //player.AddEffect<TwilightFishingEffect>(item);
                }
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                //Supersonic
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateSupersonic(item, player, hideVisual);
                }

                //Flight Mastery
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateFlightMastery(item, player, hideVisual);
                }

                //Trawler
                if (ModLoader.HasMod("SOTS"))
                {
                    //player.AddEffect<TwilightFishingEffect>(item);
                }

                //World Shaper
                if (ModLoader.HasMod("SOTS"))
                {
                    player.AddEffect<EarthenEffect>(item);
                }
            }

            if (item.type == ModContent.ItemType<TerrariaSoul>())
            {
                /*
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateTerrariaSoul(item, player, hideVisual);
                }
                */
            }

            if (item.type == ModContent.ItemType<EternitySoul>())
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
                int insertIndex = maxTooltipIndex;
                TooltipLine customLine = new TooltipLine(Mod, "StealthTooltip", stealthTooltip);
                tooltips.Insert(insertIndex, customLine);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<SupersonicSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SubspaceDash"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSSupersonicEffects"));
                }
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.EarthenEnchant.SimpleTooltip"));
            }

            if (item.type == ModContent.ItemType<FlightMasterySoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    //AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.GravityAnchor"));
                    ApplySpecialTooltips.ModifyTooltips(tooltips, "Tooltip0");
                }
            }

            if (item.type == ModContent.ItemType<TrawlerSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.TwilightFishing"));
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, ApplySpecialTooltips.GetBladewingTooltip());
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSSupersonicEffects"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SubspaceDash"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.TwilightFishing"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.EarthenEnchant.SimpleTooltip"));
                }
            }

            if (item.type == ModContent.ItemType<TerrariaSoul>())
            {
                /*
                if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
                {
                    if (ModLoader.HasMod("SOTS"))
                    {
                        if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
                        {
                            foreach (TooltipLine tooltip in tooltips)
                            {
                                if (tooltip.Mod == "FargowiltasSouls" && tooltip.Name == "FargowiltasSouls:ActiveSkills")
                                {
                                    tooltip.Text += ", ";
                                    tooltip.Text += Language.GetTextValue("Mods.SecretsOfTheSouls.ActiveSkills.BloomStrike.DisplayName");
                                }
                            }
                            AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.ChaosForce.SoulTooltip"));
                            AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.SpaceForce.SoulTooltip"));
                        }
                    }
                }
                else
                {
                    if (ModLoader.HasMod("SOTS"))
                    {
                        tooltips[3].Text += ", ";
                        tooltips[3].Text += Language.GetTextValue("Mods.SecretsOfTheSouls.ActiveSkills.BloomStrike.DisplayName");

                        AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.VoidForce.SoulTooltip"));
                    }
                }
                */
            }
        }
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

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ApplySpecialTooltips
    {
        public static void ModifyTooltips(List<TooltipLine> tooltips, string tooltipName)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                if (tooltip.Mod == "Terraria" && tooltip.Name == tooltipName)
                {
                    string bwt = GetBladewingTooltip();
                    tooltip.Text = $"{tooltip.Text}\n{bwt}";
                }
            }
        }

        public static string GetBladewingTooltip()
        {
            string str1 = Language.GetTextValue("Mods.SOTS.Common.Unbound");
            string str2 = str1;
            using (List<string>.Enumerator enumerator = SOTS.SOTS.MachinaBoosterHotKey.GetAssignedKeys((InputMode)0).GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str1 = enumerator.Current;
            }
            using (List<string>.Enumerator enumerator = SOTS.SOTS.SlowFlightHotKey.GetAssignedKeys((InputMode)0).GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str2 = enumerator.Current;
            }
            return Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.GildedBladeWings", str1, str2);
        }
    }
}
