using System.Reflection;
using System;
using Terraria;
using Terraria.ModLoader;
using MonoMod.RuntimeDetour;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Chaos;
using SOTS.Items.Invidia;
using SOTS.Items.Planetarium.Blocks;
using SOTS.Items.Pyramid;
using SOTS.Items.Secrets;
using SOTS;
using SOTS.NPCs.Boss.Excavator;
using Microsoft.Xna.Framework;

namespace FargoSoulsSOTS.Core.Systems.Hooks.SOTS
{
    public class NoExcavatorDespawnHook : ModSystem
    {
        /* This works but doesn't prevent the despawn.
        private static Hook _tileCountsHook;
        private delegate void TileCountsDelegate(object self, ReadOnlySpan<int> tileCounts);
        public override void Load()
        {
            var target = typeof(SOTSWorld).GetMethod(
                "TileCountsAvailable",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(ReadOnlySpan<int>) },
                modifiers: null
            );

            if (target != null)
                _tileCountsHook = new Hook(target, new TileCountsDelegate(Detour_TileCountsAvailable));
        }

        public override void Unload()
        {
            _tileCountsHook?.Dispose();
            _tileCountsHook = null;
        }

        private static void Detour_TileCountsAvailable(object self, ReadOnlySpan<int> tileCounts)
        {
            SOTSWorld.planetarium =
                tileCounts[ModContent.TileType<DullPlatingTile>()] +
                tileCounts[ModContent.TileType<AvaritianPlatingTile>()];

            SOTSWorld.phaseBiome =
                tileCounts[ModContent.TileType<PhaseOreTile>()];

            SOTSWorld.pyramidBiome =
                tileCounts[ModContent.TileType<SarcophagusTile>()] +
                tileCounts[ModContent.TileType<RefractingCrystalBlockTile>()] +
                tileCounts[ModContent.TileType<AcediaGatewayTile>()];

            if (IsExcavatorAlive())
            {
                SOTSWorld.AVBiome = 101;
            }
            else
            {
                SOTSWorld.AVBiome =
                    tileCounts[ModContent.TileType<SootBlockTile>()] +
                    tileCounts[ModContent.TileType<CrimsonSoot.CrimsonSootTile>()] +
                    tileCounts[ModContent.TileType<CorruptionSoot.CorruptionSootTile>()];
            }

            SOTSWorld.SanctuaryBiome =
                tileCounts[ModContent.TileType<RunicEvostoneTile>()] +
                tileCounts[ModContent.TileType<RunicEvostoneBrickTile>()] +
                tileCounts[ModContent.TileType<InvidiaPlatingTile>()] +
                tileCounts[ModContent.TileType<OvergrownEvostoneBrickTile>()];
        }

        private static bool IsExcavatorAlive()
        {
            return NPC.AnyNPCs(ModContent.NPCType<Excavator>());
        }
        */

        private static Hook _hook;

        public override void Load()
        {
            var excType = Type.GetType("SOTS.NPCs.Boss.Excavator.Excavator, SOTS", throwOnError: false);
            if (excType == null) return;

            var mi = excType.GetMethod("DespawnCheck",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi == null) return;

            _hook = new Hook(mi, PatchedDespawnCheck);
        }

        public override void Unload()
        {
            _hook?.Dispose();
            _hook = null;
        }
        private static bool PatchedDespawnCheck(object self)
        {
            var modNpc = (ModNPC)self;
            NPC npc = modNpc.NPC;

            var despawnCounterField = self.GetType().GetField("DespawnCounter",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (despawnCounterField == null)
                return false; // fail-safe

            int despawnCounter = (int)despawnCounterField.GetValue(self);

            Player target = Main.player[npc.target];

            bool tooFar = Vector2.Distance(target.Center, npc.Center) > 6400.0;
            if (target.dead || tooFar)
                despawnCounter++;
            else if (despawnCounter > 0)
                despawnCounter--;

            if (despawnCounter >= 600)
            {
                npc.active = false;
                despawnCounterField.SetValue(self, despawnCounter);
                return true;
            }

            npc.DiscourageDespawn(1000);
            despawnCounterField.SetValue(self, despawnCounter);
            return false;
        }
    }
}
