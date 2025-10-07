using System.Collections.Generic;
using System.Linq;
using SecretsOfTheSouls.Core.Interfaces;
using SOTS;
using SOTS.Buffs;
using SOTS.Items.Planetarium;
using SOTS.Void;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static SecretsOfTheSouls.SecretsOfTheSoulsCrossmod;

namespace SecretsOfTheSouls.Common
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VoidSoulsItemHybrid : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.ModItem is VoidSoulsHybrid;
        }

        public override void SetDefaults(Item item)
        {
            if (item.DamageType == DamageClass.Melee)
                item.DamageType = ModContent.GetInstance<VoidMelee>();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem is VoidItem)
                return;
            int num = (int)(double)((VoidSoulsHybrid)item.ModItem).VoidCost;
            string textValue = Language.GetTextValue("Mods.SOTS.Common.CV", VoidCost(Main.LocalPlayer, item, num));
            if (item.mana > 0)
                tooltips.FirstOrDefault(x => x.Name == "UseMana" && x.Mod == "Terraria").Text = textValue;
            else
                tooltips.Insert(tooltips.FindIndex(x => x.Name == "Knockback" && x.Mod == "Terraria") + 1, new TooltipLine(Mod, "VoidCost", textValue));
            HandleVoidPrefix(tooltips, item, num);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.ModItem is VoidItem)
                return true;
            if (player.HasBuff(ModContent.BuffType<VoidRecovery>()))
                return false;
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            int baseCost = (int)(double)((VoidSoulsHybrid)item.ModItem).VoidCost;
            int num = VoidCost(player, item, baseCost);
            if (voidPlayer.safetySwitch && voidPlayer.voidMeter < num && !voidPlayer.frozenVoid)
                return false;
            ++player.GetModPlayer<BeadPlayer>().attackNum;
            if (player.whoAmI == Main.myPlayer)
                voidPlayer.voidMeter -= num;
            if (item.mana > 0)
                player.statMana += item.mana;
            return true;
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            mult = 0.0f;
        }

        public override void PreReforge(Item item)
        {
            item.GetGlobalItem<PrefixItem>().voidCostMultiplier = 1f;
        }

        public static int VoidCost(Player player, Item item, int baseCost)
        {
            int num = (int)(baseCost * (double)VoidPlayer.ModPlayer(player).voidCost * item.GetGlobalItem<PrefixItem>().voidCostMultiplier);
            if (num < 1)
                num = 1;
            return num;
        }

        public static void HandleVoidPrefix(List<TooltipLine> tooltips, Item item, int voidAmt)
        {
            if (voidAmt <= 1 || item.GetGlobalItem<PrefixItem>().voidCostMultiplier == 1.0)
                return;
            int num = (int)(100.0 * ((double)(((int)(item.GetGlobalItem<PrefixItem>().voidCostMultiplier * (double)voidAmt) / (float)voidAmt) - 1.0)));
            if (num == 0)
                return;
            string str = num > 0 ? "+" : "";
            TooltipLine tooltipLine = new TooltipLine(Terraria.ModLoader.ModLoader.GetMod(nameof(SOTSBardHealer)), "PrefixVoidCost", str + num.ToString() + Language.GetTextValue("Mods.SOTS.Prefixes.CosVoid.DisplayName"))
            {
                IsModifier = true,
                IsModifierBad = num > 0
            };
            tooltips.Add(tooltipLine);
        }
    }
}
