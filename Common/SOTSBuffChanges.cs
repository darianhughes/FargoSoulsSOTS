using Microsoft.Xna.Framework;
using SOTS.Buffs;
using SOTS.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSBuffChanges : GlobalBuff
    {
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type == ModContent.BuffType<AbyssalInferno>())
            {
                for (int index1 = 0; index1 < 24; ++index1)
                {
                    if (Utils.NextBool(Main.rand, 30))
                    {
                        float angleRad = MathHelper.ToRadians(index1 * 15f);

                        Vector2 offset = new Vector2(32f, 0f).RotatedBy(angleRad);
                        Vector2 pushVel = Utils.SafeNormalize(offset, Vector2.Zero) * -6f;

                        int dustId = Dust.NewDust(npc.Center + offset - new Vector2(5f, 0f), 0, 0, ModContent.DustType<CopyDust4>(), 0f, 0f, 0, default, 1f);

                        Dust d = Main.dust[dustId];
                        d.velocity *= 0.3f;
                        d.velocity += pushVel;
                        d.color = new Color(100, 255, 100, 0);
                        d.noGravity = true;
                        d.fadeIn = 0.1f;
                        d.scale *= 2.25f;
                    }
                }
            }
            base.Update(type, npc, ref buffIndex);
        }
    }
}
