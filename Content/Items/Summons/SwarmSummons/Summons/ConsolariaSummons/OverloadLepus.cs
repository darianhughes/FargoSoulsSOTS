using Terraria.ID;
using Terraria.ModLoader;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using Consolaria.Content.Items.Summons;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Summons.ConsolariaSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OverloadLepus : SwarmSummonBase
    {
        private static Mod Consolaria => ModLoader.GetMod(SecretsOfTheSoulsCrossmod.Consolaria.Name);
        //public override string Texture => "Consolaria/Content/Items/Summons/SuspiciousLookingEgg";

        public OverloadLepus() : base(Consolaria.Find<ModNPC>("Lepus").Type, nameof(OverloadLepus), 50, ModContent.ItemType<SuspiciousLookingEgg>())
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<SuspiciousLookingEgg>()];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SuspiciousLookingEgg>())
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
