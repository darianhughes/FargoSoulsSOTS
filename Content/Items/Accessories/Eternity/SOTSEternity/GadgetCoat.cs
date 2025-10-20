using System.Collections.Generic;
using SecretsOfTheSouls.Content.Buffs.Emode;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using SecretsOfTheSouls.Core.Players;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Permafrost;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls;
using SOTS.Void;
using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;
using Terraria.Localization;

namespace SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    //[AutoloadEquip(EquipType.Face)]
    public class GadgetCoat : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<PlasmaHook>()];
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
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
            player.AddEffect<FidgetSpinnerEffect>(Item);

            //Gadget Coat
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            mp.GadgetCoat = true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            int damage = (int)(60 * player.ActualClassDamage(ModContent.GetInstance<VoidRanged>()));
            Color color = Color.LightGray;
            float lerp = 0.75f;
            Color tooltipColor = Color.Lerp(Color.Purple, new(38, 168, 35), lerp);
            string textValue = Language.GetTextValue("Mods.SOTS.Common.Damage");

            if (IsNotRuminating(Item))
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string text = Language.GetTextValue("Mods.SOTS.Common.VoidR", (object)damage.ToString(), (object)textValue);
                    var damageTooltip = new TooltipLine(Mod, $"{Mod.Name}:DamageTooltip", text);
                    damageTooltip.OverrideColor = tooltipColor;
                    tooltips.Insert(firstTooltip, damageTooltip);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<JellyJumpers>()
                .AddIngredient<DrillCap>()
                .AddIngredient<PlasmaGrasp>()
                .AddIngredient<CooledFidgetSpinner>()
                .AddIngredient<AbsoluteBar>(5)
                .AddIngredient(ItemID.MartianConduitPlating, 30)
                .AddIngredient<DeviatingEnergy>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
