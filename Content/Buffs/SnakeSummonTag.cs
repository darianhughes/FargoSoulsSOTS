using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Buffs
{
    public class SnakeSummonTag : ModBuff
    {
        public override string Texture => "FargoSoulsSOTS/Assets/Textures/Empty";

        public static readonly int TagDamage = 1;

        public override void Update(Player player, ref int buffIndex)
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class TagedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<SnakeSummonTag>() && projectile.IsMinionOrSentryRelated)
            {
                modifiers.FlatBonusDamage += SnakeSummonTag.TagDamage * ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];

                Player player = Main.player[projectile.owner];

                if (player.HeldItem.CountsAsClass(DamageClass.Summon) || player.HeldItem.CountsAsClass(DamageClass.SummonMeleeSpeed))
                {
                    npc.AddBuff(BuffID.Venom, 60 * 3);
                }
            }
        }
    }
}
