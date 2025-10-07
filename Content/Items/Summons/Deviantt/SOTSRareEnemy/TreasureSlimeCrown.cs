using System.Collections.Generic;
using Fargowiltas.Items.Summons;
using SOTS;
using SOTS.NPCs.TreasureSlimes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargoSoulsSOTS.Content.Items.Summons.Deviantt.SOTSRareEnemy
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class TreasureSlimeCrown : BaseSummon
    {
        public override int NPCType
        {
            get
            {
                if (Main.LocalPlayer.ZoneDungeon)
                    return ModContent.NPCType<DungeonTreasureSlime>();
                if (Main.LocalPlayer.SOTSPlayer().PyramidBiome)
                    return ModContent.NPCType<PyramidTreasureSlime>();

                if (Main.LocalPlayer.SOTSPlayer().AbandonedVillageBiome)
                    return ModContent.NPCType<MutagenTreasureSlime>();
                if (Main.LocalPlayer.SOTSPlayer().SanctuaryBiome && Main.hardMode)
                    return ModContent.NPCType<VoidTreasureSlime>();

                if (Main.LocalPlayer.ZoneHallow)
                    return ModContent.NPCType<HallowTreasureSlime>();
                if (Main.LocalPlayer.ZoneCorrupt)
                    return ModContent.NPCType<CorruptionTreasureSlime>();
                if (Main.LocalPlayer.ZoneCrimson)
                    return ModContent.NPCType<CrimsonTreasureSlime>();

                if (Main.LocalPlayer.ZoneSnow)
                    return ModContent.NPCType<IceTreasureSlime>();
                if (Main.LocalPlayer.ZoneJungle)
                    return ModContent.NPCType<JungleTreasureSlime>();

                if (Main.LocalPlayer.ZoneNormalUnderground | Main.LocalPlayer.ZoneNormalCaverns)
                    return ModContent.NPCType<GoldenTreasureSlime>();
                if (Main.LocalPlayer.ZoneUnderworldHeight && NPC.downedBoss3)
                    return ModContent.NPCType<ShadowTreasureSlime>();

                return ModContent.NPCType<BasicTreasureSlime>();
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = 0;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            List<int> intList = CapableNPCS(Main.LocalPlayer);
            tooltips.Add(new TooltipLine(Mod, "Slime1", Language.GetTextValue("Mods.FargoSoulsSOTS.Items.TreasureSlimeCrown.PossibleSlimes")));

            if (intList.Contains(ModContent.NPCType<BasicTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime1", Lang.GetNPCName(ModContent.NPCType<BasicTreasureSlime>()).Value)
                {
                    OverrideColor = Color.LightGray
                });
            if (intList.Contains(ModContent.NPCType<GoldenTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime2", Lang.GetNPCName(ModContent.NPCType<GoldenTreasureSlime>()).Value)
                {
                    OverrideColor = Color.Gold
                });
            if (intList.Contains(ModContent.NPCType<IceTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime3", Lang.GetNPCName(ModContent.NPCType<IceTreasureSlime>()).Value)
                {
                    OverrideColor = Color.LightBlue
                });
            if (intList.Contains(ModContent.NPCType<JungleTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "SlimeJ", Lang.GetNPCName(ModContent.NPCType<JungleTreasureSlime>()).Value)
                {
                    OverrideColor = Color.DarkOliveGreen
                });
            if (intList.Contains(ModContent.NPCType<DungeonTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime4", Lang.GetNPCName(ModContent.NPCType<DungeonTreasureSlime>()).Value)
                {
                    OverrideColor = Color.Indigo
                });
            if (intList.Contains(ModContent.NPCType<PyramidTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime5", Lang.GetNPCName(ModContent.NPCType<PyramidTreasureSlime>()).Value)
                {
                    OverrideColor = Color.SandyBrown
                });
            if (intList.Contains(ModContent.NPCType<MutagenTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime6", Lang.GetNPCName(ModContent.NPCType<MutagenTreasureSlime>()).Value)
                {
                    OverrideColor = Color.Pink
                });
            if (intList.Contains(ModContent.NPCType<CorruptionTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime7", Lang.GetNPCName(ModContent.NPCType<CorruptionTreasureSlime>()).Value)
                {
                    OverrideColor = Color.MediumPurple
                });
            if (intList.Contains(ModContent.NPCType<CrimsonTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime8", Lang.GetNPCName(ModContent.NPCType<CrimsonTreasureSlime>()).Value)
                {
                    OverrideColor = Color.Red
                });
            if (intList.Contains(ModContent.NPCType<HallowTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime9", Lang.GetNPCName(ModContent.NPCType<HallowTreasureSlime>()).Value)
                {
                    OverrideColor = Color.HotPink
                });
            if (intList.Contains(ModContent.NPCType<VoidTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime10", Lang.GetNPCName(ModContent.NPCType<VoidTreasureSlime>()).Value)
                {
                    OverrideColor = Color.Black
                });
            if (intList.Contains(ModContent.NPCType<ShadowTreasureSlime>()))
                tooltips.Add(new TooltipLine(Mod, "Slime11", Lang.GetNPCName(ModContent.NPCType<ShadowTreasureSlime>()).Value)
                {
                    OverrideColor = Color.Purple
                });
            if (intList.Count > 0)
                return;
            tooltips.Add(new TooltipLine(Mod, "Slime12", Language.GetTextValue("Mods.FargoSoulsSOTS.Items.TreasureSlimeCrown.None")) // This should be impossible?
            {
                OverrideColor = new(150, 150, 150)
            });
        }

        public List<int> CapableNPCS(Player player)
        {
            SOTSPlayer modPlayer = player.GetModPlayer<SOTSPlayer>();
            List<int> intList = new List<int>();
            bool flag = SOTSPlayer.ZoneForest(player);

            if (player.ZoneDungeon)
                intList.Add(ModContent.NPCType<DungeonTreasureSlime>());
            if (modPlayer.PyramidBiome)
                intList.Add(ModContent.NPCType<PyramidTreasureSlime>());

            if (modPlayer.AbandonedVillageBiome)
                intList.Add(ModContent.NPCType<MutagenTreasureSlime>());
            if (modPlayer.SanctuaryBiome && Main.hardMode)
                intList.Add(ModContent.NPCType<VoidTreasureSlime>());

            if (flag)
                intList.Add(ModContent.NPCType<BasicTreasureSlime>());

            if (player.ZoneHallow)
                intList.Add(ModContent.NPCType<HallowTreasureSlime>());
            if (player.ZoneCorrupt)
                intList.Add(ModContent.NPCType<CorruptionTreasureSlime>());
            if (player.ZoneCrimson)
                intList.Add(ModContent.NPCType<CrimsonTreasureSlime>());

            if (player.ZoneSnow)
                intList.Add(ModContent.NPCType<IceTreasureSlime>());
            if (player.ZoneJungle)
                intList.Add(ModContent.NPCType<JungleTreasureSlime>());

            if (player.ZoneNormalUnderground || player.ZoneNormalCaverns)
                intList.Add(ModContent.NPCType<GoldenTreasureSlime>());
            if (player.ZoneUnderworldHeight && NPC.downedBoss3)
                intList.Add(ModContent.NPCType<ShadowTreasureSlime>());

            return intList;
        }
    }
}
