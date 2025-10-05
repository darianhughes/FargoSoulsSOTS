using System.Collections.Generic;
using System.Linq;
using FargoSoulsSOTS.Core.Interfaces;
using SOTS;
using SOTS.Buffs;
using SOTS.Items.Planetarium;
using SOTS.Void;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common
{
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
            string textValue = Language.GetTextValue("Mods.SOTS.Common.CV", FargoSoulsSOTS.VoidCost(Main.LocalPlayer, item, num));
            if (item.mana > 0)
                tooltips.FirstOrDefault(x => x.Name == "UseMana" && x.Mod == "Terraria").Text = textValue;
            else
                tooltips.Insert(tooltips.FindIndex(x => x.Name == "Knockback" && x.Mod == "Terraria") + 1, new TooltipLine(Mod, "VoidCost", textValue));
            FargoSoulsSOTS.HandleVoidPrefix(tooltips, item, num);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.ModItem is VoidItem)
                return true;
            if (player.HasBuff(ModContent.BuffType<VoidRecovery>()))
                return false;
            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            int baseCost = (int)(double)((VoidSoulsHybrid)item.ModItem).VoidCost;
            int num = FargoSoulsSOTS.VoidCost(player, item, baseCost);
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
    }
}
