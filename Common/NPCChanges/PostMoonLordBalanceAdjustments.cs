using System.Collections.Generic;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.NPCChanges
{
    public class PostMoonLordBalanceAdjustments : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private static float extraHPMult
        {
            get
            {
                float hpMult = 1f;
                if (SecretsOfTheSoulsCrossmod.SOTS.Loaded) hpMult += 0.05f;
                if (SecretsOfTheSoulsCrossmod.Spooky.Loaded) hpMult += 0.05f;

                return hpMult;
            }
        }
        private const float extraDamageMult = 1f;

        private static HashSet<int> PostMLSoulsBossIDs = new()
        {
            ModContent.NPCType<TimberChampion>(),
            ModContent.NPCType<TerraChampion>(),
            ModContent.NPCType<EarthChampion>(),
            ModContent.NPCType<LifeChampion>(),
            ModContent.NPCType<NatureChampion>(),
            ModContent.NPCType<ShadowChampion>(),
            ModContent.NPCType<SpiritChampion>(),
            ModContent.NPCType<WillChampion>(),
            ModContent.NPCType<CosmosChampion>(),
            ModContent.NPCType<AbomBoss>(),
            ModContent.NPCType<MutantBoss>(),
        };

        private static readonly Dictionary<int, (float eHp, float eDmg)> Overrides
        = new()
        {
            // [ModContent.NPCType<TheAdvisorHead>()] = (1.12f, 1.12f), (EXAMPLE)
        };

        private static bool IsPostMLBoss(int type) => PostMLSoulsBossIDs.Contains(type);

        private static void GetMults(int type, out float eHp, out float eDmg)
        {
            if (Overrides.TryGetValue(type, out var t))
            {
                eHp = t.eHp; eDmg = t.eDmg;
            }
            else
            {
                eHp = extraHPMult; eDmg = extraDamageMult;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            bool eternity = WorldSavingSystem.EternityMode;
            bool maso = WorldSavingSystem.MasochistModeReal;

            if (!npc.boss || !IsPostMLBoss(npc.type))
                return;

            if (!eternity) return;

            GetMults(npc.type, out float emHp, out _);

            float hpMul = emHp * 1f;
            if (hpMul != 1f)
            {
                npc.lifeMax = (int)(npc.lifeMax * hpMul);
                if (npc.lifeMax < 1) npc.lifeMax = 1;
            }

            GetMults(npc.type, out _, out float emDmg);
            float dmgMul = emDmg * 1f;
            npc.damage = (int)(npc.damage * dmgMul);
        }
    }
}
