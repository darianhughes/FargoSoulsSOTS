using SOTS;
using SOTS.NPCs.Boss.Lux;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.SOTSSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class OverloadLux : SwarmSummonBase
    {
        public override string Texture => "SOTS/Items/ElectromagneticLure";

        public OverloadLux() : base(ModContent.NPCType<Lux>(), nameof(OverloadLux), 50, ModContent.ItemType<ChaosLure>())
        {
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6, false));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            this.SetResearchCost(1);

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.LihzahrdPowerCell];
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "ChaosLure")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
