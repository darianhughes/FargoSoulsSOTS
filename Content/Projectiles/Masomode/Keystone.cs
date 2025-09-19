using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SOTS.Void;
using FargoSoulsSOTS.Content.Items.Misc.Boosters;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace FargoSoulsSOTS.Content.Projectiles.Masomode
{
    public class Keystone : ModProjectile
    {
        public const float HoverOffsetY = 25f;
        public const float FireRange = 520f;
        public const int FireCooldown = 30;
        public const int BoltDamage = 45;
        public const float BoltSpeed = 12f;
        public const int HostCheckInterval = 10;

        public ref float HostNpcId => ref Projectile.ai[0];
        public ref float FireTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 13;
            Projectile.height = 34;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 12;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ModContent.GetInstance<VoidMagic>();

            Projectile.scale = 0.5f;
            DrawOffsetX = 0;   
            DrawOriginOffsetY = 0;
        }

        public override void AI()
        {
            bool lingering = Projectile.localAI[0] == 1f;
            int hostIndex = (int)HostNpcId;

            if (!lingering)
            {
                if (!Main.npc.IndexInRange(hostIndex))
                {
                    Projectile.Kill();
                    return;
                }

                NPC host = Main.npc[hostIndex];
                if (!host.active) { Projectile.Kill(); return; }

                float above = MathF.Max(20f, host.height * 0.60f); // physics-only; no gfxOffY here
                Projectile.Center = host.Top + new Vector2(0f, -above);
                Projectile.velocity = Vector2.Zero;

                FireTimer++;
                if (FireTimer >= FireCooldown && Main.myPlayer == Projectile.owner)
                {
                    int best = -1;
                    float bestDist = FireRange;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC n = Main.npc[i];
                        if (!n.active || n.friendly || !n.CanBeChasedBy() || n.whoAmI == hostIndex) continue;

                        float d = Vector2.Distance(n.Center, host.Center);
                        if (d <= bestDist && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1))
                        {
                            best = i;
                            bestDist = d;
                        }
                    }

                    if (best != -1)
                    {
                        Vector2 dir = (Main.npc[best].Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * BoltSpeed;
                        int dmg = GetScaledBoltDamage(Projectile.owner);
                        int bolt = Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            dir,
                            ModContent.ProjectileType<KeystoneBolt>(),
                            dmg,
                            2f,
                            Projectile.owner,
                            hostIndex
                        );
                        if (Main.projectile.IndexInRange(bolt))
                            Main.projectile[bolt].tileCollide = true;

                        SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                        FireTimer = 0;
                    }
                }
                return;
            }

            // Linger (still fires)
            Projectile.velocity *= 0.92f;
            if (Projectile.velocity.Length() < 0.5f)
                Projectile.velocity.Y -= 0.02f;

            FireTimer++;
            if (FireTimer >= FireCooldown && Main.myPlayer == Projectile.owner)
            {
                int best = -1;
                float bestDist = FireRange;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (!n.active || n.friendly || !n.CanBeChasedBy()) continue;

                    float d = Vector2.Distance(n.Center, Projectile.Center);
                    if (d <= bestDist && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1))
                    {
                        best = i;
                        bestDist = d;
                    }
                }

                if (best != -1)
                {
                    Vector2 dir = (Main.npc[best].Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * BoltSpeed;
                    int dmg = GetScaledBoltDamage(Projectile.owner);
                    int bolt = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        dir,
                        ModContent.ProjectileType<KeystoneBolt>(),
                        dmg,
                        2f,
                        Projectile.owner,
                        -1
                    );
                    if (Main.projectile.IndexInRange(bolt))
                        Main.projectile[bolt].tileCollide = true;

                    SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                    FireTimer = 0;
                }
            }
        }

        public void StartLinger(int ticks)
        {
            Projectile.localAI[0] = 1f;
            HostNpcId = -1;
            if (Projectile.timeLeft < ticks)
                Projectile.timeLeft = ticks;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, ModContent.ItemType<KeystoneShard>(), Main.rand.Next(1, 3));
        }

        private static int GetScaledBoltDamage(int playerId)
        {
            Player p = Main.player[playerId];
            int baseDmg = BoltDamage;
            float scale = p.GetTotalDamage(ModContent.GetInstance<VoidMagic>()).ApplyTo(1f);
            return (int)Math.Max(1, baseDmg * scale);
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);

            Vector2 origin = new Vector2(frame.Width * 0.5f, frame.Height * 0.5f);

            const float spriteNudgeX = 0f;
            const float spriteNudgeY = 0f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(spriteNudgeX, spriteNudgeY);

            Main.EntitySpriteDraw(
                tex,
                drawPos,
                frame,
                lightColor,
                0f,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
