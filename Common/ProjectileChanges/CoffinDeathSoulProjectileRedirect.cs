using SOTS.Items.Pyramid;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace SecretsOfTheSouls.Common.ProjectileChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class CoffinDeathSoulProjectileRedirect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private static int CoffinDarkSoulsProjType = ModContent.ProjectileType<CoffinDarkSouls>();
        private static int CursedCoffinNpcType = ModContent.NPCType<CursedCoffin>();

        private int localSearchCooldown;
        private float observedMaxSpeed;

        private float SpeedCap = 8f;
        private float AccelPerTick = 0.3f;
        private float HitRadius = 16f;

        private static class PyramidGateCache
        {
            public static readonly List<Point> Gates = new();
            public static uint LastScanTick;
            public static uint NoGateUntilTick;
            private const uint RescanIntervalTicks = 60 * 10;
            private const uint NoGateCooldownTicks = 60 * 10;

            private static bool scanning;

            public static bool TryGetNearest(Vector2 fromWorld, out Vector2 gateWorld)
            {
                gateWorld = default;

                // If we learned recently that no gates exist, skip quickly.
                if (Main.GameUpdateCount < NoGateUntilTick && Gates.Count == 0)
                    return false;

                // If our cache is stale AND not currently scanning, refresh.
                if (!scanning && (Main.GameUpdateCount - LastScanTick) >= RescanIntervalTicks)
                {
                    scanning = true;
                    RefreshNow();
                    scanning = false;
                }

                if (Gates.Count == 0)
                    return false;

                // Find nearest in cached list
                Point fromTile = fromWorld.ToTileCoordinates();
                float bestD2 = float.MaxValue;
                Point best = default;
                for (int i = 0; i < Gates.Count; i++)
                {
                    var p = Gates[i];
                    float d2 = (new Vector2(p.X, p.Y) - new Vector2(fromTile.X, fromTile.Y)).LengthSquared();
                    if (d2 < bestD2) { bestD2 = d2; best = p; }
                }

                gateWorld = best.ToWorldCoordinates(8, 8);
                return true;
            }

            private static void RefreshNow()
            {
                LastScanTick = Main.GameUpdateCount;
                Gates.Clear();

                int gateTileType = ModContent.TileType<PyramidGateTile>();

                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    for (int y = 0; y < Main.maxTilesY; y++)
                    {
                        Tile t = Framing.GetTileSafely(x, y);
                        if (t.HasTile && t.TileType == gateTileType)
                            Gates.Add(new Point(x, y));
                    }
                }

                if (Gates.Count == 0)
                    NoGateUntilTick = Main.GameUpdateCount + NoGateCooldownTicks;
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            localSearchCooldown = Main.rand.Next(6, 18);
        }

        public override void PostAI(Projectile projectile)
        {
            //only do this if cursed coffin is killed for the first time
            if (projectile.type != CoffinDarkSoulsProjType 
                //|| WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CursedCoffin]
                )
                return;

            int idx = (int)projectile.ai[0];
            if (idx < 0 || idx >= Main.npc.Length)
                return;

            NPC coffin = Main.npc[idx];
            if (coffin.active || coffin.type != CursedCoffinNpcType)
                return; // only affect death-burst souls

            // Staggered per-entity cooldown before asking cache
            if (localSearchCooldown-- > 0)
                return;
            localSearchCooldown = Main.rand.Next(12, 24);

            // Ask global cache (VERY cheap; scans at most once/10s globally)
            if (!PyramidGateCache.TryGetNearest(projectile.Center, out Vector2 gateWorld))
                return; // no gate known → keep default Souls behavior

            // Despawn on contact
            if (Vector2.DistanceSquared(projectile.Center, gateWorld) <= (HitRadius * HitRadius))
            {
                projectile.Kill();
                return;
            }

            // Cancel Fargo’s per-tick upward accel (ai[1] is added to Y in their AI)
            if (projectile.ai[1] != 0f)
                projectile.velocity.Y -= projectile.ai[1];

            // Direction to gate (normalized)
            Vector2 dir = gateWorld - projectile.Center;
            if (dir.LengthSquared() < 1f)
                return;
            dir.Normalize();

            // Non-decreasing speed policy until despawn
            float current = projectile.velocity.Length();
            if (current > observedMaxSpeed)
                observedMaxSpeed = current;

            float nextSpeed = observedMaxSpeed + AccelPerTick;
            if (nextSpeed > SpeedCap)
                nextSpeed = SpeedCap;
            if (nextSpeed < observedMaxSpeed)
                nextSpeed = observedMaxSpeed;

            observedMaxSpeed = nextSpeed;

            // Lock velocity to exact direction*magnitude (turning doesn’t bleed speed)
            projectile.velocity = dir * nextSpeed;
        }
    }
}
