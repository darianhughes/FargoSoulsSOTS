using SOTS.Items.Pyramid;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargoSoulsSOTS.Content.Projectiles;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    [LegacyName("CursedSarcophagus")]
    public class CursedScroll : ModItem
    {
        //Unlike other bosses, the Pharoah's Curse MUST be summoned at the Sarcophagus for the fight to work properly. This makes it weird for mutant alt summons, but this is a good alteranitve that will allow us to do overloaded summons too.
        //Ironically, Revengence+ Boss Rush has a very similar issue, but that isn't something we have to worry about here.

        //public override string Texture => "SOTS/Items/Pyramid/Sarcophagus";

        private const int SearchRangeTiles = 3;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Roar;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 2);
            Item.consumable = true;
            Item.noMelee = true;
            Item.stack = 9999;
        }

        public override bool CanUseItem(Player player)
        {
            return TryFindNearestSarcophagusOrigin(player, SearchRangeTiles, out _, out _);
        }

        public override bool? UseItem(Player player)
        {
            if (!TryFindNearestSarcophagusOrigin(player, SearchRangeTiles, out Point16 originTile, out Vector2 spawnPos))
                return false;

            // Spawn exactly like SOTS SarcophagusTile.RightClick:
            // Projectile.NewProjectile(source, origin*16 + new Vector2(48f,16f), Vector2.Zero, SpawnEnemyProj, 0, 0, myPlayer, -1f, 0f, 0f)
            if (player.whoAmI == Main.myPlayer)
            {
                IEntitySource source = player.GetSource_ItemUse(Item);
                Projectile.NewProjectile(
                    source,
                    spawnPos,                       // already origin + (48,16)
                    Vector2.Zero,
                    ModContent.ProjectileType<SpawnEnemyProjNoLimit>(),
                    0,
                    0f,
                    player.whoAmI,
                    -1f, 0f, 0f
                );
            }

            return true;
        }
        private static bool TryFindNearestSarcophagusOrigin(Player player, int rangeTiles, out Point16 originTile, out Vector2 spawnPos)
        {
            originTile = Point16.NegativeOne;
            spawnPos = Vector2.Zero;

            int targetType = ModContent.TileType<SarcophagusTile>();
            Point p = player.Center.ToTileCoordinates();

            int r = rangeTiles;
            float bestDistSq = float.MaxValue;
            bool found = false;

            // Clamp search box to world bounds.
            int xMin = Utilities.Clamp(p.X - r, 0, Main.maxTilesX - 1);
            int xMax = Utilities.Clamp(p.X + r, 0, Main.maxTilesX - 1);
            int yMin = Utilities.Clamp(p.Y - r, 0, Main.maxTilesY - 1);
            int yMax = Utilities.Clamp(p.Y + r, 0, Main.maxTilesY - 1);

            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    Tile t = Main.tile[x, y];
                    if (t == null || !t.HasTile || t.TileType != targetType)
                        continue;

                    // Recover the top-left origin of the multi-tile (exactly like SOTS RightClick).
                    int frameX = t.TileFrameX / 18;
                    int frameY = t.TileFrameY / 18;
                    int originX = x - frameX;
                    int originY = y - frameY;

                    // Convert the SOTS spawn position (origin + (48,16) px).
                    Vector2 candidateSpawn = new Vector2(originX * 16 + 48f, originY * 16 + 16f);

                    float distSq = Vector2.DistanceSquared(candidateSpawn, player.Center);
                    if (distSq < bestDistSq)
                    {
                        bestDistSq = distSq;
                        originTile = new Point16(originX, originY);
                        spawnPos = candidateSpawn;
                        found = true;
                    }
                }
            }

            return found;
        }

        private static class Utilities
        {
            public static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);
        }
    }
}
