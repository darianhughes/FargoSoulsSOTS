using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons.Energizers.SOTSEnergizers
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class EnergizerPolaris : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;
            Item.value = 1000000;
        }
    }
}
