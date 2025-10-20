using Terraria.ID;
using Terraria.ModLoader;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using Consolaria.Content.Items.Summons;
using Consolaria.Content.NPCs.Bosses.Turkor;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Summons.ConsolariaSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OverloadTurkor : SwarmSummonBase
    {
        //public override string Texture => "Consolaria/Content/Items/Summons/CursedStuffing";

        public OverloadTurkor() : base(ModContent.NPCType<TurkortheUngrateful>(), nameof(OverloadTurkor), 50, ModContent.ItemType<CursedStuffing>())
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<CursedStuffing>()];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CursedStuffing>())
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
