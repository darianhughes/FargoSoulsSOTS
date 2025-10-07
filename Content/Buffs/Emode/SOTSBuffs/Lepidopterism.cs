using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Buffs.Emode.SOTSBuffs
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class Lepidopterism : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int dps = Main.masterMode ? 6 : Main.expertMode ? 4 : 2;
            float slow = Main.masterMode ? 0.16f : Main.expertMode ? 0.12f : 0.08f;
            int defFlat = Main.masterMode ? 4 : Main.expertMode ? 3 : 2;

            if (player.lifeRegen > 0) player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            player.lifeRegen -= dps * 2;

            player.moveSpeed -= slow;

            player.statDefense -= defFlat;

            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(player.position, player.width, player.height, DustID.GlowingMushroom,
                    Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f), 120, default, 1.1f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.velocity *= 0.90f;
        }
    }
}
