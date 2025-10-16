using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Permafrost;
using Fargowiltas.Content.Items.Tiles;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using FargowiltasSouls;
using SOTS.Buffs;
using SecretsOfTheSouls.Content.Items.Misc.Boosters;
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;
using SecretsOfTheSouls.Core.Players;
using System.Collections.Generic;
using System;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FrostArtifactEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(134, 114, 223);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(45, 42, 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FrostArtifactEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrostArtifactHelmet>()
                .AddIngredient<FrostArtifactChestplate>()
                .AddIngredient<FrostArtifactTrousers>()
                .AddIngredient<HypericeClusterCannon>()
                .AddIngredient<FrigidEnchant>()
                .AddIngredient<PBow>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FrostArtifactEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<FrostArtifactEnchant>();

        public override void PostUpdate(Player player)
        {
            if (!player.HasEffect<FrostArtifactEffect>()) { DespawnAll(player); return; }

            int desired = player.ForceEffect<FrostArtifactEffect>() ? 5 : 3;
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

        public static int ShardProjType => ModContent.ProjectileType<PolarCannon>();

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

        public static void EnsureShards(Player player, int desiredCount, float radius = 96f, float speedDegPerFrame = 3.2f)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
                return;

            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            if (mp.hasSpawnedCannons) return;

            int scaled = player.GetWeaponDamage(player.HeldItem);
            int shardDamage = Math.Max(25, (int)Math.Round(scaled * 0.25));

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
                    var src = player.GetSource_Misc("PolarCannonSpawn");
                    int idx = Projectile.NewProjectile(src, player.Center, Vector2.Zero, ShardProjType, shardDamage, 0f, player.whoAmI, owned + i, desiredCount);
                    if ((uint)idx < Main.maxProjectiles)
                    {
                        Projectile p = Main.projectile[idx];
                        p.localAI[0] = radius;
                        p.localAI[1] = speedDegPerFrame;

                        p.damage = shardDamage;
                        p.originalDamage = shardDamage;
                        p.DamageType = player.HeldItem.DamageType;

                        p.netUpdate = true;
                    }
                }
            }

            mp.hasSpawnedCannons = true;

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
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            mp.hasSpawnedCannons = false;
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
                        int idx = Item.NewItem(player.GetSource_Misc("EnterVoidRecovery"), p.getRect(), ModContent.ItemType<SnowconePickup>(), 1, noBroadcast: false);
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
            SOTSEffectsPlayer fargoSOTSPlayer = player.GetModPlayer<SOTSEffectsPlayer>();
            fargoSOTSPlayer.hasSpawnedCannons = false;
        }

        private void DebuffAttacker(NPC npc, Player player)
        {
            int DebuffTime = 60 * 3;
            if (npc != null && npc.active && !npc.friendly)
            {
                npc.AddBuff(BuffID.Frostburn, DebuffTime);
                npc.AddBuff(BuffID.Frozen, DebuffTime);
                npc.AddBuff(BuffID.Frostburn2, DebuffTime);
            }
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