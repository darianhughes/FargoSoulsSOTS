using System.Collections.Generic;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using SecretsOfTheSouls.Content.Buffs.Emode;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;
using SecretsOfTheSouls.Core.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Materials;
using Fargowiltas.Content.Items.Tiles;
using SOTS.Items.Chaos;
using SOTS.Items.Planetarium;
using SOTS.Void;
using SOTS;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using SOTS.Helpers;
using Terraria.UI.Chat;

namespace SecretsOfTheSouls.Content.Items.Accessories.Souls.SOTSSoul
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [AutoloadEquip(EquipType.Face)]
    public class SubspaceVoyagerSoul : BaseSoul
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
        [
            AccessoryEffectLoader.GetEffect<BloomStrike>(),
            AccessoryEffectLoader.GetEffect<PlasmaHook>()
        ];

        public static List<int> Forces =
        [
            ModContent.ItemType<ChaosForce>(),
            ModContent.ItemType<SpaceForce>(),
        ];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            //Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);

            if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
            {
                foreach (int force in Forces)
                    modPlayer.ForceEffects.Add(force);


                ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
            }

            //Jelly Jumpers
            player.buffImmune[ModContent.BuffType<Corrosion>()] = true;

            player.extraFall += 10;
            player.autoJump = true;

            player.AddEffect<JellyJumpersEffect>(Item);

            //Drill Cap
            player.buffImmune[ModContent.BuffType<Grounded>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;

            player.AddEffect<DrillCapEffect>(Item);

            //Plasma Grasp
            player.buffImmune[BuffID.Electrified] = true;

            player.AddEffect<PlasmaHook>(Item);

            //Cooled Fidget Spiner

            //Gadget Coat
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            mp.GadgetCoat = true;

            //Sigil of the Shadows
            player.buffImmune[36] = true;
            player.buffImmune[69] = true;
            player.buffImmune[32] = true;
            player.buffImmune[46] = true;
            player.buffImmune[47] = true;
            player.buffImmune[44] = true;

            StatModifier local = player.GetDamage<VoidGeneric>();
            local += 0.25f;
            player.GetCritChance<VoidGeneric>() += 15f;
            voidPlayer.bonusVoidGain += 15f;
            voidPlayer.voidMeterMax2 += 175;
            voidPlayer.voidCost -= 0.2f;

            //Safety Switch
            if (player.AddEffect<SafetySwitchEffect>(Item))
                voidPlayer.safetySwitchVisual = !hideVisual;

            //Rot Heart
            player.AddEffect<RotHeartEffect>(Item);

            //Illuminant Lantern
            player.AddEffect<IlluminantLanternEffect>(Item);

            //Blade Necklace
            if (player.AddEffect<BladeNecklaceEffect>(Item))
            {
                BeadPlayer modPlayer2 = player.GetModPlayer<BeadPlayer>();
                int num = SOTSPlayer.ApplyDamageClassModWithGeneric(player, ModContent.GetInstance<VoidGeneric>(), 150);
                modPlayer2.soulDamage += num;
            }

            //Frigid Hourglass
            player.AddEffect<FrigidHourglassEffect>(Item);

            //Voidmage Incubator
            int uniqueVisionNumber = sotsPlayer.UniqueVisionNumber;
            VoidmageIncubator vmIncubator = new VoidmageIncubator();
            vmIncubator.GetBonuses(player, uniqueVisionNumber % 8);
            player.AddEffect<VoidmageIncubatorEffect>(Item);

            //Voidspace Emblem
            if (sotsPlayer.onhit == 1)
            {
                voidPlayer.voidMeter += (float)(3 + sotsPlayer.onhitdamage / 9);
                VoidPlayer.VoidEffect(player, 3 + sotsPlayer.onhitdamage / 9);
            }
            sotsPlayer.CritVoidsteal += 0.7f;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (!((line.Name == "ItemName") && !((line.Name == "Damage") && !(line.Name == "Favorite") && !(line.Name == "FavoriteDesc"))))
                return true;
            Color color = ColorHelper.TesseractColor(0.0f);
            Color black = Color.Black;
            TextSnippet[] array = ChatManager.ParseMessage(line.Text, black).ToArray();
            ChatManager.ConvertNormalSnippets(array);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), color, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            int num;
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, array, new Vector2(line.X, line.Y), black, line.Rotation, line.Origin, line.BaseScale, out num, line.MaxWidth, false);
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            int damage = 150;
            Color color = Color.LightGray;
            float lerp = 0.75f;
            Color tooltipColor = Color.Lerp(Color.Purple, Color.LightGray, lerp);
            string textValue = Language.GetTextValue("Mods.SOTS.Common.Damage");

            if (IsNotRuminating(Item))
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string text = Language.GetTextValue("Mods.SOTS.Common.Void2", (object)damage.ToString(), (object)textValue);
                    var damageTooltip = new TooltipLine(Mod, $"{Mod.Name}:DamageTooltip", text);
                    damageTooltip.OverrideColor = tooltipColor;
                    tooltips.Insert(firstTooltip, damageTooltip);
                }
            }
            else
            {
                int uniqueVisionNumber = SOTSPlayer.ModPlayer(Main.LocalPlayer).UniqueVisionNumber;
                tooltips.Add(new(Mod, "VoidMageIncubator", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Item.ModItem.Name}.VMIncubatorTooltip", Language.GetTextValue($"Mods.SOTS.VoidmageIncubatorTextList.{uniqueVisionNumber % 8}"))));
                tooltips.Add(new(Mod, "OtherTooltips", Language.GetTextValue($"Mods.{Mod.Name}.Items.SigiloftheShadows.OtherComponentsTooltip")));
                tooltips.Add(new(Mod, "Splash", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Item.ModItem.Name}.Splash")));
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
            {
                foreach (int force in Forces)
                    recipe.AddIngredient(force);
            }

            recipe.AddIngredient<GadgetCoat>();
            recipe.AddIngredient<SigiloftheShadows>();
            recipe.AddIngredient<AbomEnergy>(10);
            recipe.AddTile<CrucibleCosmosSheet>();
            recipe.Register();
        }
    }
}