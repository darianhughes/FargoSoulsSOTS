using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargoSoulsSOTS.Content.Buffs.Emode.SOTSBuffs
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class CryomagneticDisruption : ModBuff
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
            player.moveSpeed *= 0.75f;
            player.jumpSpeedBoost *= 0.8f;
            if (player.lifeRegen > 0) player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            player.lifeRegen -= 30;

            player.MinionAttackTargetNPC = -1;

            if (player.grapCount > 0)
            {
                player.RemoveAllGrapplingHooks();
            }

            if (player.mount.Active)
            {
                player.mount.Dismount(player);
            }

            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(player.position, player.width, player.height, DustID.Electric, 0f, 0f, 150, Color.Cyan, 1.1f);
                Main.dust[d].velocity *= 0.4f;
                Main.dust[d].noGravity = true;
            }
        }
    }
}
