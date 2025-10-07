using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SOTS.NPCs.Boss.Curse;
using SOTS.Items.Pyramid;
using FargoSoulsSOTS.Common.SOTSEffects;

namespace FargoSoulsSOTS.Content.Projectiles
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class SpawnEnemyProjNoLimit : ModProjectile
    {
        public override string Texture => "SOTS/Items/Pyramid/SpawnEnemyProj";

        private const float PharaohSentinel = -1f;

        private static readonly HashSet<ushort> ProtectedTiles = new()
        {
            // vanilla protected set
            88, 21, 26, 107, 108, 111, 226, 237, 221, 222, 223, 211, 404
        };

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.alpha = 255;
            Projectile.timeLeft = 24;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.alpha = 255;
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            float mode = Projectile.ai[0];

            if (mode == PharaohSentinel)
            {
                if (Projectile.ai[2] == 1f)
                {
                    Fargowiltas.Fargowiltas.SwarmSetDefaults = true;

                    Fargowiltas.Fargowiltas.SwarmActive = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var src = Projectile.GetSource_FromThis();
                    var spawnPos = new Vector2(Projectile.position.X + Projectile.width / 2f,
                                               Projectile.position.Y + Projectile.height);
                    int idx = NPC.NewNPC(src, (int)spawnPos.X, (int)spawnPos.Y,
                        ModContent.NPCType<PharaohsCurse>());
                    Main.npc[idx].netUpdate = true;
                    if (Projectile.ai[2] == 1f)
                    {
                        NPC npc = Main.npc[idx];
                        npc.TrySetFargoSwarmActive(true);
                    }
                }

                if (Projectile.ai[2] == 1f)
                {
                    Fargowiltas.Fargowiltas.SwarmSetDefaults = false;
                }

                return;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            int cx = (int)(Projectile.Center.X / 16f);
            int cy = (int)(Projectile.Center.Y / 16f);

            for (int x = cx - 1; x <= cx + 1; x++)
            {
                for (int y = cy - 1; y <= cy + 1; y++)
                {
                    TryKillTileAndWall(x, y, Projectile.ModProjectile);
                }
            }
        }

        private static void TryKillTileAndWall(int x, int y, ModProjectile modProj)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return;

            Tile tile = Main.tile[x, y];
            bool shouldBreak = true;

            if (tile != null && tile.HasTile)
            {
                if (Main.tileDungeon[tile.TileType])
                    shouldBreak = false;

                if (ProtectedTiles.Contains(tile.TileType))
                    shouldBreak = false;

                if (!Main.hardMode && tile.TileType == 58)
                    shouldBreak = false;

                if (!TileLoader.CanExplode(x, y))
                    shouldBreak = false;

                if (shouldBreak)
                {
                    bool noItem = tile.TileType == ModContent.TileType<CursedHive>();

                    WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: noItem);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 1);
                }
            }

            for (int wx = x - 1; wx <= x + 1; wx++)
            {
                for (int wy = y - 1; wy <= y + 1; wy++)
                {
                    if (!WorldGen.InWorld(wx, wy, 1))
                        continue;

                    Tile wTile = Main.tile[wx, wy];
                    if (wTile != null && wTile.WallType > 0)
                    {
                        if (TileLoader.CanExplode(wx, wy))
                        {
                            WorldGen.KillWall(wx, wy);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendTileSquare(-1, wx, wy, 1);
                        }
                    }
                }
            }
        }
    }
}
