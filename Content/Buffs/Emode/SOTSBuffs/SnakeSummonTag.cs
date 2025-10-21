using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;

namespace SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SnakeSummonTag : ModBuff
    {
        public override string Texture => "SecretsOfTheSouls/Assets/Textures/Empty";

        public static readonly int TagDamage = 1;

        public override void Update(Player player, ref int buffIndex)
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class TagedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            if (player.HasEffect<ChaosEffect>() && player.HasEffect<PatchLeatherEffect>() &&  projectile.IsMinionOrSentryRelated)
            {
                npc.AddBuff(BuffID.Venom, 60 * 3);
            }

            if (npc.HasBuff<SnakeSummonTag>() && projectile.IsMinionOrSentryRelated)
            {
                modifiers.FlatBonusDamage += SnakeSummonTag.TagDamage * ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];

                if (player.HeldItem.CountsAsClass(DamageClass.Summon) || player.HeldItem.CountsAsClass(DamageClass.SummonMeleeSpeed))
                {
                    npc.AddBuff(BuffID.Venom, 60 * 3);
                }
            }
        }
    }
}
