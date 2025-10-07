using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using System;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;

namespace SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SnakeWhipProjectile : ModProjectile
    {
        public Color fishingLineColor = Color.SaddleBrown;
        public Color lightingColor = Color.Transparent;
        public Color? drawColor;
        public int? swingDust = DustID.BrownMoss;
        public int dustAmount = 1;
        public SoundStyle? whipCrackSound = new SoundStyle?(SoundID.Item153);
        public Texture2D whipSegment;
        public Texture2D whipTip;
        public List<Vector2> whipPoints = new List<Vector2>();
        public float multihitModifier = 0.8f;
        public float segmentRotation;

        private bool runOnce = true;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float RangeScale => Projectile.ai[1] == 0f ? 1f : Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = false;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 0.4f;

            whipSegment = ModContent.Request<Texture2D>("SecretsOfTheSouls/Content/Projectiles/Eternity/SOTSEternity/SnakeWhipSegment").Value;
            whipTip = ModContent.Request<Texture2D>("SecretsOfTheSouls/Content/Projectiles/Eternity/SOTSEternity/SnakeWhipTip").Value;
        }
        public override bool PreAI()
        {
            whipPoints.Clear();
            Projectile.FillWhipControlPoints(Projectile, whipPoints);
            return true;
        }

        public override void AI()
        {
            WhipAIMotion();
            WhipSFX(lightingColor, swingDust, dustAmount, whipCrackSound);
        }

        private void WhipAIMotion()
        {
            Player player = Main.player[Projectile.owner];
            float totalTime = player.itemAnimationMax * Projectile.MaxUpdates;

            if (runOnce)
            {
                float playerRange = player.whipRangeMultiplier + 1f;
                Projectile.WhipSettings.RangeMultiplier *= playerRange * RangeScale;

                Projectile.WhipSettings.Segments = Math.Max(6, Projectile.WhipSettings.Segments);

                runOnce = false;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (whipPoints.Count >= 1)
                Projectile.Center = whipPoints[^1];

            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            Timer++;

            if (Timer >= totalTime || player.itemAnimation <= 0)
                Projectile.Kill();
        }

        private void WhipSFX(Color lightCol, int? dustID, int dustNum, SoundStyle? sound)
        {
            Player player = Main.player[Projectile.owner];
            float totalTime = player.itemAnimationMax * Projectile.MaxUpdates;
            player.heldProj = Projectile.whoAmI;

            Vector2 tipPos = GetTipPosition();
            if (Timer == totalTime / 2f && sound.HasValue)
                SoundEngine.PlaySound(sound.Value, tipPos);

            if (Timer < totalTime * 0.5f)
                return;

            if (dustID.HasValue)
            {
                for (int i = 0; i < dustNum; i++)
                    Dust.NewDust(tipPos, 2, 2, dustID.Value, 0f, 0f, 0, default, 0.5f);
            }

            if (lightCol != Color.Transparent)
                Lighting.AddLight(tipPos, lightCol.ToVector3() / 255f);
        }

        private Vector2 GetTipPosition()
        {
            return whipPoints.Count >= 2 ? whipPoints[^2] : Projectile.Center;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * multihitModifier);
            if (Projectile.damage < 1)
                Projectile.damage = 1;

            target.AddBuff(ModContent.BuffType<SnakeSummonTag>(), 300);

            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            Vector2 tipPos = GetTipPosition();
            for (int j = 0; j < 8; j++)
            {
                Vector2 dustOffset = new Vector2(2f, 0f).RotatedBy(MathHelper.ToRadians(j * 45) + Main.rand.NextFloat(-0.1f, 0.1f));
                Dust dust = Dust.NewDustDirect(tipPos + dustOffset, 0, 0, DustID.BrownMoss);
                dust.noGravity = true;
                dust.scale = 0.7f;
                dust.velocity *= 1.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (whipPoints == null || whipPoints.Count < 2)
                return false;

            DrawFishingLineBetweenPoints(whipPoints, fishingLineColor);

            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 pos = whipPoints[0];

            for (int i = 0; i < whipPoints.Count - 1; i++)
            {
                Texture2D tex = whipSegment;
                float scale = 1f;

                if (i == whipPoints.Count - 2)
                {
                    tex = whipTip;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.35f, 1.1f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }

                Rectangle frame = new Rectangle(0, 0, tex.Width, tex.Height);
                Vector2 origin = frame.Size() / 2f;
                Vector2 diff = whipPoints[i + 1] - whipPoints[i];
                float rot = diff.ToRotation();

                Main.EntitySpriteDraw(tex, pos - Main.screenPosition, frame, Lighting.GetColor(whipPoints[i].ToTileCoordinates()), rot, origin, scale, effect, 0f);
                pos += diff;
            }

            return false;
        }

        private void DrawFishingLineBetweenPoints(List<Vector2> points, Color color)
        {
            Texture2D tex = TextureAssets.FishingLine.Value;
            Rectangle frame = tex.Frame();
            Vector2 origin = new Vector2(frame.Width / 2f, 2f);
            Vector2 pos = points[0];

            for (int i = 0; i < points.Count - 2; i++)
            {
                Vector2 diff = points[i + 1] - points[i];
                float rot = diff.ToRotation() - MathHelper.PiOver2;
                float length = diff.Length() + 2f;
                Vector2 scale = new Vector2(1f, length / frame.Height);
                Color lightCol = Lighting.GetColor(points[i].ToTileCoordinates(), color);
                Main.EntitySpriteDraw(tex, pos - Main.screenPosition, frame, lightCol, rot, origin, scale, SpriteEffects.None, 0f);
                pos += diff;
            }
        }
    }
}
