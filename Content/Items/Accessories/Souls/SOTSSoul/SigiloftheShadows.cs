using System.Collections.Generic;
using System.Xml;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using SOTS;
using SOTS.Helpers;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Celestial;
using SOTS.Items.Chaos;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Permafrost;
using SOTS.Items.Planetarium;
using SOTS.Items.SoldStuff;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace SecretsOfTheSouls.Content.Items.Accessories.Souls.SOTSSoul
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SigiloftheShadows : BaseSoul
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override bool Eternity => true;
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);

            player.buffImmune[36] = true;
            player.buffImmune[69] = true;
            player.buffImmune[32] = true;
            player.buffImmune[46] = true;
            player.buffImmune[47] = true;
            player.buffImmune[44] = true;

            StatModifier local = player.GetDamage<VoidGeneric>();
            local += 0.18f;
            player.GetCritChance<VoidGeneric>() += 10f;
            voidPlayer.bonusVoidGain += 10f;
            voidPlayer.voidMeterMax2 += 100;
            voidPlayer.voidCost -= 0.1f;

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
                int num = SOTSPlayer.ApplyDamageClassModWithGeneric(player, ModContent.GetInstance<VoidGeneric>(), 80);
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
                voidPlayer.voidMeter += 3 + sotsPlayer.onhitdamage / 9;
                VoidPlayer.VoidEffect(player, 3 + sotsPlayer.onhitdamage / 9);
            }
            sotsPlayer.CritVoidsteal += 0.7f;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (!(line.Name == "ItemName" && !(line.Name == "Damage" && !(line.Name == "Favorite") && !(line.Name == "FavoriteDesc"))))
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
            int damage = 80;
            Color color = Color.LightGray;
            float lerp = 0.75f;
            Color tooltipColor = Color.Lerp(Color.Purple, Color.LightGray, lerp);
            string textValue = Language.GetTextValue("Mods.SOTS.Common.Damage");

            if (IsNotRuminating(Item))
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string text = Language.GetTextValue("Mods.SOTS.Common.Void2", damage.ToString(), textValue);
                    var damageTooltip = new TooltipLine(Mod, $"{Mod.Name}:DamageTooltip", text);
                    damageTooltip.OverrideColor = tooltipColor;
                    tooltips.Insert(firstTooltip, damageTooltip);
                }
            }

            int uniqueVisionNumber = SOTSPlayer.ModPlayer(Main.LocalPlayer).UniqueVisionNumber;
            tooltips.Add(new(Mod, "VoidMageIncubator", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Item.ModItem.Name}.VMIncubatorTooltip", Language.GetTextValue($"Mods.SOTS.VoidmageIncubatorTextList.{uniqueVisionNumber % 8}"))));
            tooltips.Add(new(Mod, "OtherTooltips", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Item.ModItem.Name}.OtherComponentsTooltip")));
            tooltips.Add(new(Mod, "Splash", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Item.ModItem.Name}.Splash")));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SafetySwitch>()
                .AddIngredient<RotHeart>()
                .AddIngredient<IlluminantLantern>()
                .AddIngredient<BladeNecklace>()
                .AddIngredient<FrigidHourglass>()
                .AddIngredient<VoidmageIncubator>()
                .AddIngredient<VoidspaceEmblem>()
                .AddIngredient<DeviatingEnergy>(10)
                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SafetySwitchEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SigiloftheShadowsHeader>();
        public override int ToggleItemType => ModContent.ItemType<SafetySwitch>();

        public override void PostUpdateEquips(Player player)
        {
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            voidPlayer.safetySwitch = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class RotHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SigiloftheShadowsHeader>();
        public override int ToggleItemType => ModContent.ItemType<RotHeart>();

        public override void PostUpdateEquips(Player player)
        {
            player.SOTSPlayer().RotHeart = true;
            player.VoidPlayer().GainHealthOnVoidUse += 0.1f;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class IlluminantLanternEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SigiloftheShadowsHeader>();
        public override int ToggleItemType => ModContent.ItemType<IlluminantLantern>();

        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                SOTSWorld.lightingChange += 0.077625f;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class BladeNecklaceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SigiloftheShadowsHeader>();
        public override int ToggleItemType => ModContent.ItemType<BladeNecklace>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            BeadPlayer modPlayer2 = player.GetModPlayer<BeadPlayer>();
            modPlayer2.RetaliationSouls = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FrigidHourglassEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SigiloftheShadowsHeader>();
        public override int ToggleItemType => ModContent.ItemType<FrigidHourglass>();
        public override void PostUpdateEquips(Player player)
        {
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            voidPlayer.frozenMaxDuration += 900;
            voidPlayer.frozenMinTimer -= 900;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VoidmageIncubatorEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SigiloftheShadowsHeader>();
        public override int ToggleItemType => ModContent.ItemType<VoidmageIncubator>();
        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            sotsPlayer.VMincubator = true;
            sotsPlayer.TimeFreezeImmune = true;
        }
    }
}
