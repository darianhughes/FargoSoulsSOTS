using System.Collections.Generic;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories;
using FargowiltasSouls.Content.Items;
using SOTS.Items.ChestItems;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    public class FargoGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ModContent.ItemType<BerserkerSoul>())
            {
                CrossmodAdditions.UpdateMeleeSoul(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<SnipersSoul>())
            {
                CrossmodAdditions.UpdateRangedSoul(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<ArchWizardsSoul>())
            {
                CrossmodAdditions.UpdateMageSoul(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<ConjuristsSoul>())
            {
                CrossmodAdditions.UpdateSummonSoul(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<UniverseSoul>())
            {
                CrossmodAdditions.UpdateUniverseSoul(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<SupersonicSoul>())
            {
                CrossmodAdditions.UpdateSupersonic(item, player, hideVisual);
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateWorldShaper(item, player, hideVisual);
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
                    SOTSAddtions.UpdateTrawlerSoul(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<Devilshield>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateDevilshield(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<ColossusSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateColossusSoul(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                //Supersonic
                CrossmodAdditions.UpdateSupersonic(item, player, hideVisual);

                //Flight Mastery
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateFlightMastery(item, player, hideVisual);
                }

                //Trawler
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateTrawlerSoul(item, player, hideVisual);
                }

                //World Shaper
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateWorldShaper(item, player, hideVisual);
                }

                //Colossus
                if (ModLoader.HasMod("SOTS"))
                {
                    SOTSAddtions.UpdateColossusSoul(item, player, hideVisual);
                }
            }

            if (item.type == ModContent.ItemType<MasochistSoul>())
            {
                if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded && !(SecretsOfTheSoulsCrossmod.Consolaria.Loaded))
                {
                    ModItem otherworldCore = SecretsOfTheSoulsCrossmod.Heartbeataria.Mod.Find<ModItem>("OtherworldCore");
                    otherworldCore.UpdateAccessory(player, hideVisual);
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
                CrossmodAdditions.UpdateEternitySoul(item, player, hideVisual);
            }
        }

        public void AddTooltip(List<TooltipLine> tooltips, string stealthTooltip, bool afterSplash = false, bool inRuminate = false)
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
                tooltips.Insert(insertIndex + (afterSplash ? 1 : 0), customLine);
            }
        }

        public void ModifyExistingTooltip(List<TooltipLine> tooltips, string itemString, string newTooltip, bool fullReplace = false, bool afterText = true, bool newLine = true)
        {
            int insert = tooltips.FindIndex(t => t.Text.Contains(itemString));
            if (fullReplace)
            {
                tooltips[insert].Text = tooltips[insert].Text.Replace(itemString, itemString + newTooltip);
            }
            else
            {
                if (!newLine)
                    tooltips[insert].Text = tooltips[insert].Text.Replace(itemString, afterText ? $"{newTooltip}, {itemString}" : $"{itemString} {newTooltip}");
                else
                    tooltips[insert].Text = tooltips[insert].Text.Replace(itemString, afterText ? $"{newTooltip}\n{itemString}" : $"{itemString}\n{newTooltip}");
                //tooltips.Insert(insert + (afterText ? 1 : 0), new TooltipLine(Mod, "DLCTooltip", newTooltip));
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<BerserkerSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSBerserker"));
                }
            }

            if (item.type == ModContent.ItemType<SnipersSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSSniper"));
                }
            }

            if (item.type == ModContent.ItemType<ArchWizardsSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.PlasmaShrimp"));
                    string wishingStarToolip = ApplySpecialTooltips.IsAlternate ? Language.GetTextValue("Mods.SOTS.Items.WishingStar.AltTooltip") : Language.GetTextValue("Mods.SOTS.Items.WishingStar.DefaultTooltip");
                    AddTooltip(tooltips, $"[i:SOTS/WishingStar] {wishingStarToolip}");
                }
            }

            if (item.type == ModContent.ItemType<ConjuristsSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.FortressGenerator"));
                }
            }

            if (item.type == ModContent.ItemType<UniverseSoul>())
            {
                if (!SoulsItem.IsNotRuminating(item))
                {
                    if (ModLoader.HasMod("SOTS"))
                    {
                        ModifyExistingTooltip(tooltips, "[i:1321]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSBerserker"));
                        ModifyExistingTooltip(tooltips, "[i:1595]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSSniper"));
                        ModifyExistingTooltip(tooltips, "[i:1595]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.FortressGenerator"));
                        string wishingStarToolip = ApplySpecialTooltips.IsAlternate ? Language.GetTextValue("Mods.SOTS.Items.WishingStar.AltTooltip") : Language.GetTextValue("Mods.SOTS.Items.WishingStar.DefaultTooltip");
                        ModifyExistingTooltip(tooltips, "[i:1595]", $"{Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.PlasmaShrimp")}\n[i:SOTS/WishingStar] {wishingStarToolip}");
                    }
                }
            }

            if (item.type == ModContent.ItemType<SupersonicSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SubspaceDash"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSSupersonicEffects"));
                }
                if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ShadowboundExo", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.DOWN" : "Key.UP")));
                }
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
               
                if (ModLoader.HasMod("SOTS"))
                {
                    ModifyExistingTooltip(tooltips, "[i:3624]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSWorldshaperEffects"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.MiningMode"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.Items.EarthenEnchant.SimpleTooltip"));
                }
            }

            if (item.type == ModContent.ItemType<FlightMasterySoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    //AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.GravityAnchor"));
                    ApplySpecialTooltips.ModifyTooltips(tooltips, "Tooltip0", 3);
                }
            }

            if (item.type == ModContent.ItemType<TrawlerSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/AnglerEnchant]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.TwilightFishing"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ZombieHand"));
                }
            }

            if (item.type == ModContent.ItemType<Devilshield>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ShardGaurd.Expanded"), true);
                }
            }

            if (item.type == ModContent.ItemType<ColossusSoul>())
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ShardGaurd.Short"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.Bulwark"));
                    AddTooltip(tooltips, Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.AlchemistsCharm"));
                }
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                if (SoulsItem.IsNotRuminating(item))
                {
                    if (ModLoader.HasMod("SOTS"))
                        ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/ColossusSoul]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.MiningModeShort"));
                }
                else
                {
                    if (ModLoader.HasMod("SOTS"))
                    {
                        ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/Devilshield]", $"{ApplySpecialTooltips.GetBladewingTooltip(5)}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SubspaceDash")}");
                    }
                    if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                    {
                        ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/Devilshield]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ShadowboundExo", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.DOWN" : "Key.UP")));
                    }
                    if (ModLoader.HasMod("SOTS"))
                    {
                        ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/Devilshield]", $"{Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ShardGaurd.Soul")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.AlchemistsCharm")}");
                        ModifyExistingTooltip(tooltips, "[i:PanicNecklace]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSSupersonicEffects"));
                        ModifyExistingTooltip(tooltips, "[i:RoyalGel]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.TwilightFishingSoD"));
                        ModifyExistingTooltip(tooltips, "[i:3624]", $"{Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.SOTSWorldShaperSoD")}\n{Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.MiningModeSoD")}");
                        ModifyExistingTooltip(tooltips, "[i:3624]", Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.ZombieHand"), newLine: false);
                    }
                }
            }

            if (item.type == ModContent.ItemType<MasochistSoul>())
            {
                if (SoulsItem.IsNotRuminating(item))
                {
                    if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded && !(SecretsOfTheSoulsCrossmod.Consolaria.Loaded))
                    {
                        ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/BionomicCluster]", "[i:XDContentMod/OtherworldCore]", afterText: false, newLine: false);
                    }
                }
                else
                {
                    if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded && !(SecretsOfTheSoulsCrossmod.Consolaria.Loaded))
                    {
                        ModifyExistingTooltip(tooltips, "[i:FargowiltasSouls/DubiousCircuitry]", $"[i:XDContentMod/OtherworldCore] {Language.GetTextValue("Mods.XDContentMod.Items.OtherworldCore.Tooltip")}");
                    }
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
    public class ApplySpecialTooltips
    {
        public static void ModifyTooltips(List<TooltipLine> tooltips, string tooltipName, int voidIncrease)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                if (tooltip.Mod == "Terraria" && tooltip.Name == tooltipName)
                {
                    string bwt = GetBladewingTooltip(voidIncrease);
                    tooltip.Text = $"{tooltip.Text}\n{bwt}";
                }
            }
        }

        public static bool IsAlternate => WishingStar.IsAlternate;

        public static string GetBladewingTooltip(int voidIncrease)
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
            return Language.GetTextValue("Mods.SecretsOfTheSouls.NewTooltips.GildedBladeWings", str1, str2, voidIncrease.ToString());
        }
    }
}
