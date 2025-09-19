using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargoSoulsSOTS.Core.SoulToggles;
using SOTS.Items.Permafrost;
using System.Collections.Generic;
using FargowiltasSouls;
using SOTS.Buffs;
using SOTS.Void;
using FargoSoulsSOTS.Core.Players;
using Mono.Cecil;
using FargoSoulsSOTS.Content.Items.Misc.Boosters;
using FargoSoulsSOTS.Content.Projectiles.Masomode;

namespace FargoSoulsSOTS.Content.Items.Accessories.Enchantments
{
    public class FrigidEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(187, 199, 255);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 50000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FrigidEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()

                .AddIngredient<FrigidCrown>()
                .AddRecipeGroup("FargoSoulsSOTS:FrigidChests")
                .AddIngredient<FrigidGreaves>()
                .AddIngredient<ShardStaff>()
                .AddIngredient<ShatterBlade>()
                .AddIngredient<FrigidJavelin>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class FrigidEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<FrigidEnchant>();

        public override void PostUpdate(Player player)
        {
            if (!player.HasEffect<FrigidEffect>()) { DespawnAll(player); return; }

            int desired = player.ForceEffect<FrigidEffect>() ? 6 : 3;
            int current = CountOwned(player);

            bool hasVoidShock = player.HasBuff(ModContent.BuffType<VoidShock>());
            bool hasVoidRecovery = player.HasBuff(ModContent.BuffType<VoidRecovery>());

            if (hasVoidShock)
            {
                EnsureShards(player, desired);
            }
            else if (hasVoidRecovery)
            {
                DespawnAllWithBonus(player);
            }
            else
            {
                DespawnAll(player);
            }
        }

        public override void OnHitByNPC(Player player, NPC npc, Player.HurtInfo hurtInfo)
        {
            if (ConsumeOneShard(player))
            {
                DebuffAttacker(npc, player);
            }
        }

        public override void OnHitByProjectile(Player player, Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (ConsumeOneShard(player))
            {
                DebuffAttacker(ResolveOwningNPC(proj) ?? NearestHostileNPC(proj.Center, 600f), player);
            }
        }

        public static int ShardProjType => ModContent.ProjectileType<VoidShatterShard>();

        public static int CountOwned(Player player)
        {
            int owned = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                    owned++;
            }
            return owned;
        }

        public static void EnsureShards(Player player, int desiredCount, int baseDamage = 10, float radius = 96f, float speedDegPerFrame = 3.2f)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
                return;

            FargoSOTSPlayer mp = player.GetModPlayer<FargoSOTSPlayer>();

            if (mp.hasSpawnedShards) return;

            var shards = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                    shards.Add(p);
            }

            int owned = shards.Count;
            if (owned < desiredCount)
            {
                int toSpawn = desiredCount - owned;
                for (int i = 0; i < toSpawn; i++)
                {
                    var src = player.GetSource_Misc("ShatterShardSpawn");
                    int idx = Projectile.NewProjectile(src, player.Center, Vector2.Zero, ShardProjType, baseDamage, 0f, player.whoAmI, owned + i, desiredCount);
                    if ((uint)idx < Main.maxProjectiles)
                    {
                        Projectile p = Main.projectile[idx];
                        p.localAI[0] = radius;
                        p.localAI[1] = speedDegPerFrame;
                        if (p.damage <= 0) p.damage = baseDamage;
                        p.originalDamage = p.damage;
                        p.netUpdate = true;
                    }
                }
            }

            mp.hasSpawnedShards = true;

            Reindex(player);
        }

        public static void TrimTo(Player player, int keepCount)
        {
            List<int> ids = new();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                    ids.Add(i);
            }
            // kill extras from the end
            for (int k = ids.Count - 1; k >= 0 && ids.Count > keepCount; k--)
            {
                Main.projectile[ids[k]].Kill();
                ids.RemoveAt(k);
            }
            Reindex(player);
        }

        public static void DespawnAll(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                    p.Kill();
            }
            FargoSOTSPlayer mp = player.GetModPlayer<FargoSOTSPlayer>();
            mp.hasSpawnedShards = false;
        }

        public static void DespawnAllWithBonus(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int idx = Item.NewItem(player.GetSource_Misc("EnterVoidRecovery"), p.getRect(), ModContent.ItemType<VoidShatterShardPickup>(), 1, noBroadcast: false);
                        ref Item it = ref Main.item[idx];

                        // center the item on the projectile and copy rotation/velocity
                        it.position = p.Center - new Vector2(it.width, it.height) * 0.5f;
                        it.velocity = p.velocity;

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, idx);
                    }

                    p.Kill();
                }
            }
            FargoSOTSPlayer fargoSOTSPlayer = player.GetModPlayer<FargoSOTSPlayer>();
            fargoSOTSPlayer.hasSpawnedShards = false;
        }

        private void DebuffAttacker(NPC npc, Player player)
        {
            int DebuffType = player.ForceEffect<FrigidEffect>() ? BuffID.Frostburn : BuffID.Frozen;
            int DebuffTime = 60 * 3;
            if (npc != null && npc.active && !npc.friendly && DebuffType > 0)
                npc.AddBuff(DebuffType, DebuffTime);
        }

        //Consumes one shard, then reindexes the remainder.
        public static bool ConsumeOneShard(Player player)
        {
            int pick = -1;
            int maxIndexVal = int.MinValue;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                {
                    int idx = (int)p.ai[0];
                    if (idx > maxIndexVal)
                    {
                        maxIndexVal = idx;
                        pick = i;
                    }
                }
            }

            if (pick != -1)
            {
                Main.projectile[pick].Kill();
                Reindex(player);
                return true;
            }

            return false;
        }

        //Reassigns contiguous indices and updates the shared Count on all shards.
        public static void Reindex(Player player)
        {
            List<Projectile> shards = new();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == ShardProjType)
                    shards.Add(p);
            }

            int n = shards.Count;
            for (int i = 0; i < n; i++)
            {
                Projectile p = shards[i];
                p.ai[0] = i;
                p.ai[1] = n;
                p.netUpdate = true;
            }
        }

        private static NPC ResolveOwningNPC(Projectile proj)
        {
            int tryA = (int)proj.ai[0];
            if (tryA >= 0 && tryA < Main.maxNPCs && Main.npc[tryA].active) return Main.npc[tryA];

            int tryB = (int)proj.ai[1];
            if (tryB >= 0 && tryB < Main.maxNPCs && Main.npc[tryB].active) return Main.npc[tryB];

            return null;
        }

        private static NPC NearestHostileNPC(Vector2 from, float maxDist)
        {
            NPC best = null;
            float bestD = maxDist;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly) continue;
                float d = Vector2.Distance(npc.Center, from);
                if (d < bestD)
                {
                    best = npc;
                    bestD = d;
                }
            }
            return best;
        }
    }
}