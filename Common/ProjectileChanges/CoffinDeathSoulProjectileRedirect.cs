using SOTS.Items.Pyramid;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using System.Collections.Generic;
using Terraria.DataStructures;
using SecretsOfTheSouls.Core.Systems;
using Terraria.Chat;
using Terraria.Localization;

namespace SecretsOfTheSouls.Common.ProjectileChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class CoffinDeathSoulProjectileRedirect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        // Fargo types we target
        private static int CoffinDarkSoulsProjType = ModContent.ProjectileType<CoffinDarkSouls>();
        private static int CursedCoffinNpcType = ModContent.NPCType<CursedCoffin>();

        // Simple, hot‑reload friendly tunables
        private static float HomingRampDurationTicks = 120f;      // 2.0s
        private static float ExpectedDistanceTiles = 50f;         // ref distance mentioned by user
        private static float MaxAccelPerTick = 0.95f;             // cleaner, slightly gentler field
        private static float SpeedMax = 8f;                       // slightly lower max speed
        private static float HitRadiusPx = 18f;                   // despawn radius at gate center

        private static int GateRescanIntervalTicks = 60 * 10;    // global cache refresh
        private static int GateNoResultCooldownTicks = 60 * 10;   // cooldown after 0 results
        private static int LocalSearchCooldownMin = 12;           // per‑proj query staggering
        private static int LocalSearchCooldownMax = 24;

        // Quick near-ring easing (simple, effective)
        private static float NearRingTiles = 6f;                  // within 6 tiles of gate
        private static float NearRingDamp = 0.9f;                 // damp velocity per tick
        private static float NearRingSpeedMax = 6f;               // lower local max speed
        // Scale down initial outward poof speed from Fargo's burst (death-burst only)
        private static float OutwardSpawnSpeedScale = 0.7f;

        // Per‑projectile state
        private int ageTicks;
        private int localSearchCooldown;
        private bool hasTarget;
        private Vector2 targetWorld;
        private bool phasingOut;           // began gate-entry animation
        private int phaseTicks;            // ticks spent phasing
        private const int PhaseTotal = 12; // quick scale-down duration

        // Message persistence handled via SecretsOfTheSoulsWorldSavingSystem.coffinMessagePlayed

        // Cached list of 5‑wide PyramidGate centers (one point per gate)
        private static class PyramidGates
        {
            public static readonly List<Point> CenterTiles = new();
            public static uint LastScanTick;
            public static uint NoGateUntilTick;

            public static bool TryNearest(Vector2 fromWorld, out Vector2 gateWorld)
            {
                gateWorld = default;

                // Ensure we scan immediately on first request or when empty
                if (CenterTiles.Count == 0 || (Main.GameUpdateCount - LastScanTick) >= GateRescanIntervalTicks)
                    Scan();

                if (CenterTiles.Count == 0)
                    return false;

                Point fromTile = fromWorld.ToTileCoordinates();
                float bestD2 = float.MaxValue;
                Point best = default;
                for (int i = 0; i < CenterTiles.Count; i++)
                {
                    Point p = CenterTiles[i];
                    float dx = p.X - fromTile.X;
                    float dy = p.Y - fromTile.Y;
                    float d2 = dx * dx + dy * dy;
                    if (d2 < bestD2) { bestD2 = d2; best = p; }
                }

                gateWorld = best.ToWorldCoordinates(8, 8);
                return true;
            }

            private static void Scan()
            {
                LastScanTick = Main.GameUpdateCount;
                CenterTiles.Clear();

                int gateTileType = ModContent.TileType<PyramidGateTile>();
                var seen = new HashSet<Point>();

                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    for (int y = 0; y < Main.maxTilesY; y++)
                    {
                        Tile t = Framing.GetTileSafely(x, y);
                        if (!t.HasTile || t.TileType != gateTileType)
                            continue;

                        // Compute left/top of the 5‑wide and grab the center tile
                        int left = x - t.TileFrameX / 18;
                        int top = y - t.TileFrameY / 18;
                        Point center = new(left + 2, top);
                        if (seen.Add(center))
                            CenterTiles.Add(center);
                    }
                }

                if (CenterTiles.Count == 0)
                    NoGateUntilTick = Main.GameUpdateCount + (uint)GateNoResultCooldownTicks;
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            ageTicks = 0;
            hasTarget = false;
            localSearchCooldown = Main.rand.Next(LocalSearchCooldownMin, LocalSearchCooldownMax);
            phasingOut = false;
            phaseTicks = 0;
        }

        public override void PostAI(Projectile projectile)
        {
            // Only affect Fargo's Coffin Dark Souls, and only after the Coffin dies
            if (projectile.type != CoffinDarkSoulsProjType)
                return;

            int idx = (int)projectile.ai[0];
            if (idx < 0 || idx >= Main.npc.Length)
                return;

            NPC coffin = Main.npc[idx];
            if (coffin.active || coffin.type != CursedCoffinNpcType)
                return;

            ageTicks++;
            if (ageTicks == 1)
            {
                // Reduce initial outward burst speed for a cleaner look
                projectile.velocity *= OutwardSpawnSpeedScale;
            }

            // Periodically find nearest gate center (cached globally)
            if (--localSearchCooldown <= 0)
            {
                localSearchCooldown = Main.rand.Next(LocalSearchCooldownMin, LocalSearchCooldownMax);
                if (PyramidGates.TryNearest(projectile.Center, out Vector2 nearest))
                {
                    targetWorld = nearest;
                    hasTarget = true;
                }
            }

            if (!hasTarget)
                return;

            // Smoothly turn on the homing field over 1.5 seconds
            float ramp = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(ageTicks / HomingRampDurationTicks, 0f, 1f));

            // 5x5 tile square centered on gate center tile (more forgiving capture)
            Point centerTile = targetWorld.ToTileCoordinates();
            Point projTile = projectile.Center.ToTileCoordinates();
            bool insideGateSquare = System.Math.Abs(projTile.X - centerTile.X) <= 2 && System.Math.Abs(projTile.Y - centerTile.Y) <= 2;
            if (!phasingOut && (insideGateSquare || Vector2.DistanceSquared(projectile.Center, targetWorld) <= HitRadiusPx * HitRadiusPx))
            {
                // Begin quick scale-down + effects, then despawn
                phasingOut = true;
                phaseTicks = 0;
                // one-time impact burst
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.DD2_WitherBeastAuraPulse, projectile.Center);
            }

            if (phasingOut)
            {
                // Slow to a stop and shrink
                projectile.velocity *= 0.85f;
                projectile.scale *= 0.9f;
                projectile.Opacity *= 0.85f;

                // Purple-y dust poof
                for (int i = 0; i < 6; i++)
                {
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Terraria.ID.DustID.Shadowflame, 0f, 0f, 150, default, 1.1f);
                    d.velocity = (d.position - projectile.Center).SafeNormalize(Main.rand.NextVector2Unit()) * Main.rand.NextFloat(1.2f, 3.6f);
                    d.noGravity = true;
                }

                phaseTicks++;
                if (phaseTicks >= PhaseTotal)
                {
                    int coffinId = (int)projectile.ai[0];
                    projectile.Kill();
                    TryAnnounceLockRevealedIfLast(coffinId);
                }
                return; // skip homing while phasing out
            }

            // Completely cancel Fargo's per-tick upward accel (keep pure radial expansion)
            if (projectile.ai[1] != 0f)
                projectile.velocity.Y -= projectile.ai[1];

            // Apply homing as a simple acceleration toward the gate center
            float expectedDistPx = ExpectedDistanceTiles * 16f;
            Vector2 dir = (targetWorld - projectile.Center).SafeNormalize(Vector2.Zero);
            float dist = Vector2.Distance(projectile.Center, targetWorld);
            float distFactor = MathHelper.Clamp(dist / expectedDistPx, 0.25f, 1f);
            float accel = MaxAccelPerTick * ramp * distFactor;

            projectile.velocity += dir * accel;

            // Clamp top speed for smooth visuals
            float speed = projectile.velocity.Length();
            if (speed > SpeedMax)
                projectile.velocity *= (SpeedMax / speed);

            // Near-ring damping: slow down and cap speed when close to the gate
            float nearPx = NearRingTiles * 16f;
            if (dist < nearPx)
            {
                projectile.velocity *= NearRingDamp;
                speed = projectile.velocity.Length();
                if (speed > NearRingSpeedMax)
                    projectile.velocity *= (NearRingSpeedMax / speed);
            }
        }

        private static void TryAnnounceLockRevealedIfLast(int coffinId)
        {
            if (coffinId < 0 || coffinId >= Main.npc.Length)
                return;

            // Any remaining death-burst souls for this coffin?
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.type != CoffinDarkSoulsProjType)
                    continue;

                int id = (int)p.ai[0];
                if (id == coffinId)
                {
                    // One still remains; abort for now.
                    return;
                }
            }

            // None remain: show the reveal message now (first time only per world)
            if (SecretsOfTheSoulsWorldSavingSystem.coffinMessagePlayed)
                return;

            var color = new Color(175, 75, 255);
            string text = "The coffin's curse fades, opening the keyhole on the mysterious gate...";

            if (Main.netMode == Terraria.ID.NetmodeID.Server)
            {
                // Server: persist and broadcast
                SecretsOfTheSoulsWorldSavingSystem.coffinMessagePlayed = true;
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
            }
            else if (Main.netMode == Terraria.ID.NetmodeID.SinglePlayer)
            {
                // Singleplayer: local persist + local text
                SecretsOfTheSoulsWorldSavingSystem.coffinMessagePlayed = true;
                Main.NewText(text, color);
            }
            // Multiplayer clients do nothing; they will receive the server broadcast
        }
    }
}
