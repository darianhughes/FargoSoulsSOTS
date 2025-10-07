using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using System.IO;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;

namespace FargoSoulsSOTS.Common.ProjectileChanges
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class BloomHookMinionAnchor : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        // --- tag so we only affect hooks spawned by Wormwood ---
        public bool FromWormwood;

        public void MarkAsWormwood()
        {
            FromWormwood = true;
        }

        public static void TagAsWormwood(int projIdx)
        {
            if (projIdx >= 0 && projIdx < Main.maxProjectiles)
            {
                var p = Main.projectile[projIdx];
                p.GetGlobalProjectile<BloomHookMinionAnchor>().FromWormwood = true;
                p.netUpdate = true;
            }
        }

        private int _frame;
        private float _aiCounter;
        private float _aiCounter2;
        private Vector2 _rotateVec = new Vector2(4f, 0f);

        // cache SOTS types
        private static int _bloomHookType = -1, _flowerBoltType = -1;
        private static Asset<Texture2D> _vineTex;
        private static int BloomHookType { get { if (_bloomHookType == -1 && ModLoader.TryGetMod("SOTS", out var s)) _bloomHookType = s.Find<ModProjectile>("BloomingHook").Type; return _bloomHookType; } }
        private static int FlowerBoltType { get { if (_flowerBoltType == -1 && ModLoader.TryGetMod("SOTS", out var s)) _flowerBoltType = s.Find<ModProjectile>("FriendlyFlowerBolt").Type; return _flowerBoltType; } }
        private static Texture2D VineTexture
        {
            get
            {
                if (_vineTex == null || !_vineTex.IsLoaded)
                {
                    // same asset SOTS uses
                    _vineTex = ModContent.Request<Texture2D>("SOTS/Projectiles/Nature/BloomingVine", AssetRequestMode.ImmediateLoad);
                }
                return _vineTex.Value;
            }
        }

        public override bool AppliesToEntity(Projectile proj, bool lateInstantiation) => proj.type == BloomHookType;

        // store anchor in localAI[0] as (whoAmI + 1); 0 = unset
        private static int GetAnchorIndex(Projectile hook) => (int)hook.localAI[0] - 1;
        private static void SetAnchorIndex(Projectile hook, int idx) => hook.localAI[0] = idx + 1;

        private static Projectile GetAnchorMinion(Projectile hook)
        {
            int idx = GetAnchorIndex(hook);
            if (idx >= 0 && idx < Main.maxProjectiles)
            {
                ref Projectile p = ref Main.projectile[idx];
                if (p.active && p.owner == hook.owner && p.minion && p.minionSlots > 0f)
                    return p;
            }
            int best = -1; float bestD2 = float.MaxValue;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                ref Projectile p = ref Main.projectile[i];
                if (p.active && p.owner == hook.owner && p.minion && p.minionSlots > 0f)
                {
                    float d2 = Vector2.DistanceSquared(p.Center, hook.Center);
                    if (d2 < bestD2) { bestD2 = d2; best = i; }
                }
            }
            if (best != -1) { SetAnchorIndex(hook, best); return Main.projectile[best]; }
            return null;
        }

        private static Vector2 FindTarget(Projectile hook)
        {
            Player plr = Main.player[hook.owner];
            float best = 800f; Vector2 pos = hook.Center; bool found = false;

            if (plr.HasMinionAttackTargetNPC)
            {
                NPC n = Main.npc[plr.MinionAttackTargetNPC];
                if (n.active && !n.friendly)
                {
                    float d = Vector2.Distance(n.Center, hook.Center);
                    if (d < best && Collision.CanHitLine(hook.position, hook.width, hook.height, n.position, n.width, n.height))
                    { best = d; pos = n.Center; found = true; }
                }
            }

            if (!found)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (n.CanBeChasedBy(null))
                    {
                        float d = Vector2.Distance(n.Center, hook.Center);
                        if (d < best && Collision.CanHitLine(hook.position, hook.width, hook.height, n.position, n.width, n.height))
                        { best = d; pos = n.Center; found = true; }
                    }
                }
            }
            return found ? pos : new Vector2(-1f, -1f);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type != BloomHookType) return;

            // Do nothing unless this hook was tagged by Wormwood spawner.
            // If tagged but anchor missing, acquire one immediately.
            if (FromWormwood && GetAnchorIndex(projectile) < 0)
                GetAnchorMinion(projectile);
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.type != BloomHookType) return base.PreAI(projectile);

            // Only override AI for Wormwood-tagged hooks.
            if (!projectile.GetGlobalProjectile<BloomHookMinionAnchor>().FromWormwood)
                return base.PreAI(projectile); // let SOTS run its normal AI

            Player owner = Main.player[projectile.owner];
            if (!owner.active || owner.dead)
            {
                projectile.Kill();
                return false;
            }

            Projectile anchor = GetAnchorMinion(projectile);
            if (anchor is null)
            {
                projectile.Kill();
                return false;
            }

            // ===== Minion-anchored AI =====
            Vector2 target = FindTarget(projectile);
            _aiCounter++;

            Vector2 toTarget = target - projectile.Center;
            Vector2 dirToTarget = toTarget.LengthSquared() > 1e-4f ? Vector2.Normalize(toTarget) : Vector2.Zero;
            _rotateVec += dirToTarget * 1f;
            _rotateVec = new Vector2(4f, 0f).RotatedBy(Utils.ToRotation(_rotateVec));

            Vector2 toAnchor = anchor.Center - projectile.Center;
            float dist = toAnchor.Length();
            if (dist >= 64f)
            {
                Vector2 dir = dist > 0f ? toAnchor / dist : Vector2.Zero;
                projectile.velocity = dir * (dist - 64f);
            }
            else if (anchor.Center.Y < projectile.Center.Y)
            {
                projectile.velocity.Y = -2f;
            }
            else
            {
                Vector2 wobble = new Vector2(0.15f, 0f).RotatedBy(MathHelper.ToRadians(_aiCounter * 2f));
                Vector2 drift = new Vector2(0.9f, 0f).RotatedBy(projectile.rotation);
                if (target.X == -1f && target.Y == -1f) drift = Vector2.Zero;
                projectile.velocity = drift + wobble;
            }

            const float push = 0.4f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (i == projectile.whoAmI) continue;
                Projectile other = Main.projectile[i];
                if (other.active && other.owner == projectile.owner && other.type == projectile.type)
                {
                    if (System.Math.Abs(projectile.position.X - other.position.X) +
                        System.Math.Abs(projectile.position.Y - other.position.Y) < projectile.width)
                    {
                        projectile.velocity.X += (projectile.position.X < other.position.X) ? -push : push;
                        projectile.velocity.Y += (projectile.position.Y < other.position.Y) ? -push : push;
                    }
                }
            }

            projectile.rotation = Utils.ToRotation(_rotateVec);

            _aiCounter2++;
            if (_aiCounter2 >= 75f && ((target.X != -1f && target.Y != -1f) || _frame != 0))
            {
                projectile.frameCounter++;
                if (projectile.frameCounter >= 5)
                {
                    _frame++;
                    if (_frame == 7 && Main.myPlayer == projectile.owner)
                    {
                        SoundEngine.PlaySound(SoundID.Item30 with { PitchVariance = 0.2f }, projectile.Center);

                        int boltType = FlowerBoltType > 0 ? FlowerBoltType : ProjectileID.FlowerPow;
                        Projectile.NewProjectile(
                            projectile.GetSource_FromThis(),
                            projectile.Center,
                            _rotateVec,
                            boltType,
                            projectile.damage,
                            1f,
                            projectile.owner
                        );
                        projectile.netUpdate = true;
                    }
                    if (_frame >= 13) { _aiCounter2 = 0f; _frame = 0; }
                    projectile.frameCounter = 0;
                }
            }
            projectile.frame = _frame;

            if (projectile.timeLeft < 100) projectile.timeLeft = 100;

            return false;
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (projectile.type != BloomHookType) return;
            bitWriter.WriteBit(FromWormwood);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            if (projectile.type != BloomHookType) return;
            FromWormwood = bitReader.ReadBit();
        }

        public override bool PreDraw(Projectile projectile, ref Color drawColor)
        {
            if (projectile.type != BloomHookType) return base.PreDraw(projectile, ref drawColor);

            if (!projectile.GetGlobalProjectile<BloomHookMinionAnchor>().FromWormwood)
                return base.PreDraw(projectile, ref drawColor);

            Projectile anchor = GetAnchorMinion(projectile);
            if (anchor != null)
            {
                Texture2D vine = VineTexture;
                Vector2 vineOrigin = new Vector2(vine.Width * 0.5f, vine.Height * 0.5f);

                Vector2 delta = projectile.Center - anchor.Center;
                float halfLen = delta.Length() / 2f;
                if (delta.X < 0f) halfLen = -halfLen;
                Vector2 mid = anchor.Center + delta / 2f;
                float rot = delta.ToRotation();

                float t = projectile.ai[0];
                for (int i = 9; i > 0; --i)
                {
                    Vector2 arm = new Vector2(halfLen, 0f).RotatedBy(MathHelper.ToRadians(18 * i));
                    arm.Y /= 4f;
                    Vector2 p = mid + arm.RotatedBy(rot);
                    Vector2 wobble = new Vector2(2.5f, 0f).RotatedBy(MathHelper.ToRadians(i * 36) + t * 2f);
                    Vector2 screenPos = p + wobble - Main.screenPosition;

                    Main.spriteBatch.Draw(
                        vine,
                        screenPos,
                        null,
                        projectile.GetAlpha(drawColor),
                        MathHelper.ToRadians(18 * i - 45) + rot,
                        vineOrigin,
                        projectile.scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }

            Texture2D projTex = TextureAssets.Projectile[projectile.type].Value;
            int frames = Main.projFrames[projectile.type] > 0 ? Main.projFrames[projectile.type] : 1;
            int frameHeight = projTex.Height / frames;
            int frame = projectile.frame % frames;
            Rectangle src = new Rectangle(0, frame * frameHeight, projTex.Width, frameHeight);
            Vector2 origin = new Vector2(src.Width * 0.5f, src.Height * 0.5f);

            Main.spriteBatch.Draw(
                projTex,
                projectile.Center - Main.screenPosition,
                src,
                projectile.GetAlpha(drawColor),
                projectile.rotation,
                origin,
                projectile.scale,
                SpriteEffects.None,
                0f
            );

            // Block the original ModProjectile.PreDraw (which draws vine to the player).
            return false;
        }
    }
}
