using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargoSoulsSOTS.Content.Projectiles.Masomode;
using FargowiltasSouls;
using FargowiltasSouls.Content.Patreon.Volknet.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SOTS;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Projectiles.Nature;
using SOTS.Void;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static FargoSoulsSOTS.Content.Items.Accessories.Enchantments.ElementalEnchant;

namespace FargoSoulsSOTS.Core.Players
{
    public class FargoSOTSPlayer : ModPlayer
    {
        public const float CurseRadius = 420f;
        public const int CurseDuration = 60 * 8;
        public const int KeyStoneLifeTime = CurseDuration + 30;
        public const int LingerTicks = 120;

        public int BloomTimeLeft;
        public bool BloomReduced;
        public int CullCountPending;
        public int ChaosCharge;
        public int MinersCurse;
        public int MinersCurseDuration;
        public int storedCodeBurst;
        public float voidExpended;
        public bool GrayCrescentVoid;

        private bool strongCodeBurst = false;

        float prevVoid, prevMax, prevMax2;

        public int MaxCursedPerPlayer
        {
            get
            {
                return Player.ForceEffect<CursedEffect>() ? 10 : 5;
            }
        }
        public bool hasSpawnedShards = false;

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

        public override void PostUpdate()
        {
            VoidPlayer mp = VoidPlayer.ModPlayer(Player);

            if (MinersCurse > 100)
                MinersCurse = 100;

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
