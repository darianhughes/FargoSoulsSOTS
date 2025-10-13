using Terraria;
using Microsoft.Xna.Framework;

namespace SecretsOfTheSouls.Core.SecretsofTheSoulsUtils
{
    public static class NPCHelpers
    {
        public static void FollowPlayer(NPC npc, Player player, float speed = 6f, float inertia = 18f, float stopDistance = 64f, float leashDistance = 1600f)
        {
            if (player == null || !player.active || player.dead)
                return;

            // Optional: behave like a flyer
            npc.noGravity = true;
            npc.noTileCollide = true;

            Vector2 toTarget = player.Center - npc.Center;
            float dist = toTarget.Length();

            // If too far, snap a bit faster so it catches up
            float moveSpeed = speed;
            if (dist > 800f) moveSpeed *= 1.5f;
            if (dist > 1200f) moveSpeed *= 2.0f;

            // If very close, slow down (gentle stop)
            if (dist < stopDistance)
            {
                npc.velocity *= 0.90f; // brake
                return;
            }

            // Teleport back if leashed too far (prevents getting stuck off-screen)
            if (dist > leashDistance)
            {
                npc.Center = player.Center + new Vector2(Main.rand.NextFloat(-80, 80), Main.rand.NextFloat(-80, 80));
                npc.velocity = Vector2.Zero;
                return;
            }

            // Normalized desired velocity toward the player
            Vector2 desired = toTarget.SafeNormalize(Vector2.Zero) * moveSpeed;

            // Smoothly approach desired velocity
            npc.velocity = (npc.velocity * (inertia - 1f) + desired) / inertia;

            // Face the movement direction
            if (npc.velocity.X != 0f)
                npc.direction = npc.spriteDirection = npc.velocity.X > 0f ? 1 : -1;
        }
    }
}
