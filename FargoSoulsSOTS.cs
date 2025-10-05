global using LumUtils = Luminance.Common.Utilities.Utilities;
using System.Collections.Generic;
using SOTS;
using SOTS.Void;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static FargoSoulsSOTS.FargoSOTSCrossmod;

namespace FargoSoulsSOTS
{
	public class FargoSoulsSOTS : Mod
	{
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
