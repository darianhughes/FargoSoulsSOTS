using System;
using System.Collections.Generic;
using System.Linq;
using FargoSoulsSOTS.Common;
using FargoSoulsSOTS.Common.ItemChanges;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargoSoulsSOTS.Content.Items.Accessories.Masomode;
using FargoSoulsSOTS.Content.Projectiles.Masomode;
using Fargowiltas.NPCs;
using FargowiltasSouls;
using FargowiltasSouls.Content.Patreon.Volknet.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SOTS;
using SOTS.Items.Fishing;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.Wings;
using SOTS.NPCs.Boss.Excavator;
using SOTS.Projectiles.Nature;
using SOTS.Void;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static FargoSoulsSOTS.Content.Items.Accessories.Enchantments.ElementalEnchant;

namespace FargoSoulsSOTS.Core.Players
{
    public class FargoSOTSPlayer : ModPlayer
    {
        //Cursed Enchant
        public const float CurseRadius = 420f;
        public const int CurseDuration = 60 * 8;
        public const int KeyStoneLifeTime = CurseDuration + 30;
        public const int LingerTicks = 120;

        //Jelly Jumper Effects
        private const int TicksPerStage = 90;
        private const int MaxStages = 4;
        private const int RocketLoopEveryTicks = 18;
        private static readonly float[] StageJumpVel = { 0f, 15f, 18f, 21f, 24f };

        public int BloomTimeLeft;
        public bool BloomReduced;
        public int CullCountPending;
        public int ChaosCharge;
        public int MinersCurse;
        public int MinersCurseDuration;
        public int storedCodeBurst;
        public float voidExpended;
        public bool GrayCrescentVoid;
        public bool debuffCorrosion;
        public bool DrillCapEquipped;
        public bool DrillCapVisible;

        private bool strongCodeBurst = false;
        private int announcedStage;
        private int rocketLoopTimer;
        private int heldDown;

        float prevVoid, prevMax, prevMax2;

        public int MaxCursedPerPlayer
        {
            get
            {
                return Player.ForceEffect<CursedEffect>() ? 10 : 5;
            }
        }
        public bool hasSpawnedShards = false;

        public override void ResetEffects()
        {
            debuffCorrosion = false;
            DrillCapEquipped = false;
            DrillCapVisible = false;
        }

        public override void UpdateEquips()
        {
            SOTSPlayer sotsPlayer = Player.GetModPlayer<SOTSPlayer>();

            if (Player.HasEffect<CursedEffect>())
                Player.gravControl = true;

            if (Player.HasEffect<GhostPepperMinionEffect>())
                sotsPlayer.petPepper = true;

            if (Player.HasEffect<HoloEyeMinionEffect>())
            {
                if (!sotsPlayer.HoloEye)
                    sotsPlayer.HoloEyeDamage += SOTSPlayer.ApplyAttackSpeedClassModWithGeneric(Player, DamageClass.Summon, 33);
                sotsPlayer.HoloEye = true;
                sotsPlayer.HoloEyeAutoAttack = true;

                //prevent the armor ability if you don't have the armor set on.
                if (!(Player.head == ModContent.ItemType<TwilightAssassinsCirclet>() && Player.body == ModContent.ItemType<TwilightAssassinsChestplate>() && Player.legs == ModContent.ItemType<TwilightAssassinsLeggings>()))
                {
                    sotsPlayer.HoloEyeAttack = false;
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (debuffCorrosion)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegen -= 10;
            }
        }

        public override void PostUpdate()
        {
            VoidPlayer mp = VoidPlayer.ModPlayer(Player);

            if (MinersCurse > 100)
                MinersCurse = 100;

            if (Player.FargoSouls().MutantPresence)
            {
                MachinaBoosterPlayer modPlayer = Player.GetModPlayer<MachinaBoosterPlayer>();
                modPlayer.CreativeFlightTier2 = false;
                modPlayer.canCreativeFlight = false;
            }

            TickVoidTracking(Player, mp);

            if (NPCUtils.AnyBosses())
            {
                MinersCurse = 0;
                MinersCurseDuration = 0;
            }

            if (Player.HeldItem.pick <= 0 && Player.HeldItem.hammer <= 0 && Player.HeldItem.axe <= 0)
            {
                if (MinersCurseDuration % 2 == 0)
                    MinersCurse--;
            }

            if (MinersCurse > 0)
            {
                MinersCurseDuration++;
                if (MinersCurseDuration >= 60 * 5)
                {
                    if (MinersCurseDuration % 2 == 0)
                        MinersCurse--;
                }
            }
            else
            {
                MinersCurse = 0;
                MinersCurseDuration = 0;
            }

            if (BloomTimeLeft > 0)
            {
                BloomTimeLeft--;
                if (BloomTimeLeft == 0)
                {
                    // Ability ended: cull minions now.
                    if (CullCountPending > 0)
                        DespawnOwnedMinions(CullCountPending);
                    CullCountPending = 0;
                }
            }

            if (Player.HasBuff(BuffID.PotionSickness))
            {
                if (Player.HasEffect<VesperaEffect>())
                {
                    float voidRegenBonus = Player.ForceEffect<VesperaEffect>() ? 0.05f : 0.15f;
                    mp.voidRegenSpeed += voidRegenBonus;
                }
            }
            if (Player.HasBuff<VoidAttunement>())
            {
                int voidBonus = Player.ForceEffect<VesperaEffect>() ? 50 : 25;
                mp.voidMeterMax2 += voidBonus;
            }

            if (!Player.HasEffect<ChaosTeleport>())
            {
                ChaosCharge = 0;
            }

            if (Player.HasEffect<EarthenEffect>())
            {
                float bonus = MinersCurse * 0.01f;

                Player.moveSpeed += bonus;

                Player.pickSpeed *= MathF.Max(0.05f, 1f - bonus);
            }

            if (Player.HasEffect<JellyJumpersEffect>())
            {
                if (Player.velocity.Y > 0)
                {
                    ResetCharge();
                }

                if (Player.controlDown && Player.velocity.Y == 0)
                {
                    heldDown++;
                    int stage = Math.Clamp(heldDown / TicksPerStage, 0, MaxStages);

                    if (--rocketLoopTimer < 0 && stage < MaxStages)
                    {
                        var rocket = SoundID.Item13 with { PitchVariance = 0.12f, Volume = 0.6f };
                        SoundEngine.PlaySound(rocket, Player.Center);
                        rocketLoopTimer = RocketLoopEveryTicks;
                    }

                    if (stage > 0 && stage != announcedStage)
                    {
                        announcedStage = stage;

                        SoundEngine.PlaySound(SoundID.MaxMana with { Volume = 0.8f, PitchVariance = 0.15f }, Player.Center);
                        SpawnCloudDust(Player, 24 + stage + 6);
                    }
                }
                if (Player.releaseDown)
                {
                    if (announcedStage > 0)
                        PerformChargedJump(announcedStage);

                    ResetCharge();
                }
            }
            else
            {
                ResetCharge();
            }
        }

        public override void PostUpdateMiscEffects()
        {
            ForceBiomes();
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            ModifyHitByBoth(ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            ModifyHitByBoth(ref modifiers);
        }

        private void ModifyHitByBoth(ref Player.HurtModifiers modifiers, NPC npc = null, Projectile proj = null)
        {
            if (debuffCorrosion)
            {
                ref StatModifier local = ref modifiers.SourceDamage;
                local += 0.05f;
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            int power = (attempt.playerFishingConditions).BaitPower + (attempt.playerFishingConditions).PolePower;
            bool flag = Player.HasBuff(123);
            if (Player.HasEffect<TwilightFishingEffect>() && SOTSPlayer.ScaleCatch2(power, 0, 100, flag ? 8 : 16, flag ? 80 : 160))
            {
                itemDrop = Main.hardMode
                    ? ModContent.ItemType<OtherworldCrate>()
                    : ModContent.ItemType<PlanetariumCrate>();
            }

        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (debuffCorrosion)
            {
                if (!Utils.NextBool(Main.rand, 4))
                {
                    Dust dust = Dust.NewDustDirect(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.UnholyWater, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, new Color(), 1f);
                    dust.noGravity = true;
                    Dust dust34 = dust;
                    dust34.velocity += Vector2.One;
                    dust.velocity.Y -= 0.5f;
                    if (Utils.NextBool(Main.rand, 4))
                    {
                        dust.noGravity = false;
                        dust.scale *= 0.5f;
                    }
                }
                Lighting.AddLight(Player.Center, 0.1f, 0.1f, 0.6f);
            }
        }

        void TickVoidTracking(Player Player, VoidPlayer mp)
        {
            bool meterChanged = mp.voidMeter != prevVoid;
            bool maxChanged = mp.voidMeterMax != prevMax || mp.voidMeterMax2 != prevMax2;

            if (meterChanged && !maxChanged && Player.HasEffect<TwilightAssassinEffect>())
            {
                float spent = prevVoid - mp.voidMeter;
                if (spent > 0f && !(spent >= 100f))
                    voidExpended += spent;
            }

            // Update snapshots AFTER checks
            prevVoid = mp.voidMeter;
            prevMax = mp.voidMeterMax;
            prevMax2 = mp.voidMeterMax2;

            // Convert expended void into stored bursts
            if (voidExpended > 0 && Player.HasEffect<TwilightAssassinEffect>())
            {
                int perBurst = Player.ForceEffect<TwilightAssassinEffect>() ? 7 : 5;

                if (voidExpended - perBurst >= 0)
                {
                    storedCodeBurst += 15;
                    voidExpended -= perBurst;
                }
            }
            else
            {
                voidExpended = 0;
            }

            // Fire stored bursts
            if (storedCodeBurst > 0 && Player.HasEffect<TwilightAssassinEffect>())
            {
                if (storedCodeBurst % 15 == 0)
                {
                    int damageMult = strongCodeBurst ? (Player.ForceEffect<TwilightAssassinEffect>() ? 3 : 2) : 1;
                    int base33 = (int)Math.Round(Player.GetTotalDamage(DamageClass.Magic).ApplyTo(33));
                    int damage = Math.Max(33, base33) * damageMult;

                    Projectile.NewProjectile(
                        Player.GetSource_FromThis(),
                        Player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<CodeBurst>(),
                        damage,
                        3f,
                        Player.whoAmI
                    );

                    strongCodeBurst = !strongCodeBurst;
                }
                storedCodeBurst--;
            }
            else
            {
                storedCodeBurst = 0;
            }
        }

        public static void DrawDebuffCounterForPlayer(
            SpriteBatch spriteBatch,
            Texture2D texture,
            Player player,
            Vector2 screenPos,
            ref int height,
            int value,
            Color drawColor)
        {
            if (value <= 0)
                return;

            const int colCount = 11;
            int colWidth = texture.Width / colCount;
            int colHeight = texture.Height;

            int innerWidth = colWidth - 2;
            int innerHeight = colHeight - 2;
            Vector2 origin = new(colWidth * 0.5f, colHeight * 0.5f);

            string s = value.ToString();
            int digitCount = s.Length;

            Vector2 pos = new(player.Center.X, player.Top.Y);
            pos.X += (digitCount * innerWidth) * 0.5f;
            pos.X += 4f;
            pos.Y -= height;

            Vector2 drawPos = pos;

            for (int i = s.Length - 1; i >= 0; i--)
            {
                int digit = s[i] - '0';
                var src = new Rectangle(
                    x: 1 + (1 + digit) * colWidth,
                    y: 1,
                    width: innerWidth,
                    height: innerHeight
                );

                spriteBatch.Draw(
                    texture,
                    drawPos - screenPos,
                    src,
                    drawColor,
                    0f,
                    origin,
                    1f,
                    SpriteEffects.None,
                    0f
                );

                drawPos.X -= innerWidth;
            }

            drawPos.X -= 4f;
            drawPos.Y -= 1f;

            var capSrc = new Rectangle(0, 0, colWidth, colHeight);

            spriteBatch.Draw(
                texture,
                drawPos - screenPos,
                capSrc,
                drawColor,
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );

            height += 24;
        }

        public static void DrawPermanentDebuffsForPlayer(
            Player player,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color color,
            Texture2D texture,
            ref int counter,
            ref int height)
        {
            DrawDebuffCounterForPlayer(spriteBatch, texture, player, screenPos, ref height, counter, color);
        }

        public static bool KeystoneLinger(Player player) => player.ForceEffect<CursedEffect>();

        public void SendClientChanges(Player player, NPC npc, int type = 0)
        {
            switch (type)
            {
                case 0:
                    byte whoAmI = (byte)player.whoAmI;
                    ModPacket packet1 = Mod.GetPacket(256);
                    packet1.Write(MinersCurse);
                    packet1.Send(-1, -1);
                    break;
            }
        }

        private void DespawnOwnedMinions(int countToCull)
        {
            // Prefer to remove newest minions first.
            var ownedMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Player.whoAmI)
                {
                    if (p.type == ModContent.ProjectileType<BloomingHook>())
                        p.Kill();
                    if (p.minion && p.minionSlots > 0f)
                        ownedMinions.Add(p);
                }
            }

            if (ownedMinions.Count <= 2)
                return;

            foreach (var p in ownedMinions
                .OrderByDescending(p => p.timeLeft) // “newest” heuristic
                .Take(Math.Min(countToCull, ownedMinions.Count - 2)))
            {
                p.Kill();
            }
        }

        private void ForceBiomes()
        {
            if (FargoGlobalNPC.SpecificBossIsAlive(ref FargoSOTSGlobalNPC.excavatorBoss, ModContent.NPCType<Excavator>())
                && Player.Distance(Main.npc[FargoSOTSGlobalNPC.excavatorBoss].Center) < 3000)
            {
                if (WorldGen.crimson)
                    Player.ZoneCrimson = true;
                else
                    Player.ZoneCorrupt = true;

                //This doesn't work but its here for safety
                if (SOTSWorld.AVBiome <= 100)
                    SOTSWorld.AVBiome = 101;
            }
        }

        private void PerformChargedJump(int stage)
        {
            stage = Math.Clamp(stage, 1, MaxStages);

            float v = StageJumpVel[stage];

            float desiredVy = -v;
            if (Player.velocity.Y > desiredVy)
                Player.velocity.Y = desiredVy;

            Player.fallStart = (int)(Player.position.Y / 16f);

            SpawnCloudDust(Player, 40 + stage * 12, burst: true);
        }

        private void SpawnCloudDust(Player p, int count, bool burst = false)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = p.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-24f, 8f));
                Vector2 vel = burst
                    ? Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 3.5f)
                    : new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-0.6f, -0.1f));

                int d = Dust.NewDust(pos, 0, 0, DustID.Cloud, vel.X, vel.Y, 150, default, Main.rand.NextFloat(1.0f, 1.6f));
                Main.dust[d].noGravity = true;
            }
        }

        private void ResetCharge()
        {
            heldDown = 0;
            announcedStage = 0;
            rocketLoopTimer = 0;
        }
    }

    public class FargoSOTSPlayerAssetLoader : ModSystem
    {
        public static Asset<Texture2D> HarvestingDigits;

        public override void Load()
        {
            if (Main.dedServ) return;

            if (ModLoader.TryGetMod("SOTS", out _))
            {
                HarvestingDigits = ModContent.Request<Texture2D>(
                    "SOTS/Common/GlobalNPCs/Harvesting",
                    AssetRequestMode.ImmediateLoad
                );
            }
        }

        public override void Unload()
        {
            HarvestingDigits = null;
        }
    }

    public class PlayerCurseDebuffCounterLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
            => new BeforeParent(PlayerDrawLayers.FrontAccFront);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            if (!player.active || player.dead) return;

            var asset = FargoSOTSPlayerAssetLoader.HarvestingDigits;
            if (asset is null || !asset.IsLoaded) return;

            var modPlr = player.GetModPlayer<FargoSOTSPlayer>();

            int height = 18;
            FargoSOTSPlayer.DrawPermanentDebuffsForPlayer(
                player,
                Main.spriteBatch,
                Main.screenPosition,
                Color.White,
                asset.Value,
                ref modPlr.MinersCurse,
                ref height
            );
        }
    }
}
