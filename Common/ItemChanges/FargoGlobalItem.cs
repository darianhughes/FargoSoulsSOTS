using System.Collections.Generic;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargoSoulsSOTS.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using SOTS;
using SOTS.Items;
using SOTS.Items.DoorItems;
using SOTS.Items.Planetarium;
using SOTS.Items.Wings;
using SOTS.Void;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.ItemChanges
{
    public class FargoGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            Mod sots = ModLoader.GetMod("SOTS");
            DoorPlayer dp = player.GetModPlayer<DoorPlayer>();
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);

            if (item.type == ModContent.ItemType<SupersonicSoul>())
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
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                player.AddEffect<EarthenEffect>(item);
            }

            if (item.type == ModContent.ItemType<FlightMasterySoul>())
            {
                voidPlayer.bonusVoidGain += 3f;
                voidPlayer.voidRegenSpeed += 0.25f;
                sotsPlayer.SpiritSymphony = true;
                MachinaBoosterPlayer modPlayer = player.GetModPlayer<MachinaBoosterPlayer>();
                int num;
                bool flag = (num = 1) != 0;
                modPlayer.CreativeFlightTier2 = num != 0;
                modPlayer.canCreativeFlight = flag;

                player.AddEffect<GravityAnchorEffect>(item);
                player.noKnockback = true;
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
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

                voidPlayer.bonusVoidGain += 3f;
                voidPlayer.voidRegenSpeed += 0.25f;
                sotsPlayer.SpiritSymphony = true;
                MachinaBoosterPlayer modPlayer = player.GetModPlayer<MachinaBoosterPlayer>();
                int num;
                bool flag = (num = 1) != 0;
                modPlayer.CreativeFlightTier2 = num != 0;
                modPlayer.canCreativeFlight = flag;

                player.AddEffect<GravityAnchorEffect>(item);

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

                voidPlayer.bonusVoidGain += 3f;
                voidPlayer.voidRegenSpeed += 0.25f;
                sotsPlayer.SpiritSymphony = true;
                MachinaBoosterPlayer modPlayer = player.GetModPlayer<MachinaBoosterPlayer>();
                int num;
                bool flag = (num = 1) != 0;
                modPlayer.CreativeFlightTier2 = num != 0;
                modPlayer.canCreativeFlight = flag;

                player.AddEffect<GravityAnchorEffect>(item);

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
            if (item.type == ModContent.ItemType<SupersonicSoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.SubspaceDash"));
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.SOTSSupersonicEffects"));
            }

            if (item.type == ModContent.ItemType<WorldShaperSoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.EarthenEnchant.SimpleTooltip"));
            }

            if (item.type == ModContent.ItemType<FlightMasterySoul>())
            {
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.GravityAnchor"));
                foreach (TooltipLine tooltip in tooltips)
                {
                    if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip0")
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
                        tooltip.Text = $"{tooltip.Text}\n{Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.GildedBladeWings", (object)str1, (object)str2)}";
                    }
                }
            }

            if (item.type == ModContent.ItemType<DimensionSoul>())
            {
                foreach (TooltipLine tooltip in tooltips)
                {
                    if (tooltip.Mod == "Terraria" && tooltip.Name == "Tooltip3")
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
                        tooltip.Text = $"{tooltip.Text}\n{Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.GildedBladeWings", (object)str1, (object)str2)}";
                    }
                }
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.SOTSSupersonicEffects"));
                AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.NewTooltips.SubspaceDash"));
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
                        AddTooltip(tooltips, Language.GetTextValue("Mods.FargoSoulsSOTS.Items.SpaceForce.SoulTooltip"));
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
}
