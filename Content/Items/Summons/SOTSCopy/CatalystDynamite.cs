using System;
using System.Collections.Generic;
using Fargowiltas.Items.Summons;
using SOTS.Items.Celestial;
using SOTS.NPCs.Boss;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    [LegacyName("CatalyzedCrystal")]
    public class CatalystDynamite : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<SubspaceSerpentHead>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<CatalystBomb>()];
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneUnderworldHeight;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CatalystBomb>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
