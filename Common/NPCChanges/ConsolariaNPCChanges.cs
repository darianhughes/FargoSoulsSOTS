using System.Collections.Generic;
using Consolaria.Content.NPCs.Bosses.Lepus;
using Consolaria.Content.NPCs.Bosses.Ocram;
using Consolaria.Content.NPCs.Bosses.Turkor;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.NPCChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaNPCChanges : GlobalNPC
    {
        private static Mod Consolaria => ModLoader.GetMod(SecretsOfTheSoulsCrossmod.Consolaria.Name);
        public override bool InstancePerEntity => true;

        private const float eternityHPMult = 1.10f;
        private const float eternityDamageMult = 1.10f;
        private const float masoHPMult = 1.25f;
        private const float masoDamageMult = 1.20f;
        private const float masoDamageReductionMult = 0.90f;

        private static readonly HashSet<int> ConsolariaBossIds = new()
        {
            Consolaria.Find<ModNPC>("Lepus").Type,
            ModContent.NPCType<TurkortheUngrateful>(),
            ModContent.NPCType<Ocram>(),
        };

        private static readonly Dictionary<int, (float emHp, float emDmg, float masoHp, float masoDmg)> Overrides
            = new()
            {
                // [ModContent.NPCType<TheAdvisorHead>()] = (1.12f, 1.12f, 1.30f, 1.22f), (EXAMPLE)
            };

        private static bool IsConsolariaBoss(int type) => ConsolariaBossIds.Contains(type);

        private static void GetMults(int type, out float emHp, out float emDmg, out float masoHp, out float masoDmg)
        {
            if (Overrides.TryGetValue(type, out var t))
            {
                emHp = t.emHp; emDmg = t.emDmg; masoHp = t.masoHp; masoDmg = t.masoDmg;
            }
            else
            {
                emHp = eternityHPMult; emDmg = eternityDamageMult; masoHp = masoHPMult; masoDmg = masoDamageMult;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            bool eternity = WorldSavingSystem.EternityMode;
            bool maso = WorldSavingSystem.MasochistModeReal;

            if (!npc.boss || !IsConsolariaBoss(npc.type))
                return;

            if (!eternity) return;

            GetMults(npc.type, out float emHp, out _, out float masoHp, out _);

            float hpMul = emHp * (maso ? masoHp : 1f);
            if (hpMul != 1f)
            {
                npc.lifeMax = (int)(npc.lifeMax * hpMul);
                if (npc.lifeMax < 1) npc.lifeMax = 1;
            }

            GetMults(npc.type, out _, out float emDmg, out _, out float masoDmg);
            float dmgMul = emDmg * (maso ? masoDmg : 1f);
            npc.damage = (int)(npc.damage * dmgMul);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!npc.boss || !IsConsolariaBoss(npc.type))
                return;

            if (!WorldSavingSystem.EternityMode)
                return;

            bool maso = WorldSavingSystem.MasochistModeReal;
            GetMults(npc.type, out _, out float emDmg, out _, out float masoDmg);

            float mul = emDmg * (maso ? masoDmg : 1f);
            if (mul != 1f)
                modifiers.SourceDamage *= mul;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!npc.boss || !IsConsolariaBoss(npc.type))
                return;

            if (WorldSavingSystem.EternityMode && WorldSavingSystem.MasochistModeReal && masoDamageReductionMult != 1f)
                modifiers.FinalDamage *= masoDamageReductionMult;
        }

        public override void OnSpawn(NPC npc, IEntitySource source) { }

        #region Consolaria Potion Dropping
        #endregion

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LeadingConditionRule emodeRule = new(new EModeDropCondition());

            #region Crates & Emode Accessories
            if (npc.type == ModContent.NPCType<DisasterBunny>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GlowBulb>()));
            }
            if (npc.type == ModContent.NPCType<TurkortheUngrateful>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<JellyJumpers>()));
            }
            if (npc.type == ModContent.NPCType<Ocram>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<AnkhoftheCursedRuler>()));
            }
            #endregion

            #region Tim's Concotion drop

            #endregion

            npcLoot.Add(emodeRule);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaEmodeFirstKillDrop : GlobalNPC
    {
        private static Mod Consolaria => ModLoader.GetMod(SecretsOfTheSoulsCrossmod.Consolaria.Name);
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            List<IItemDropRule> rules = [];

            if (npc.type == Consolaria.Find<ModNPC>("Lepus").Type)
            {
                rules.Add(FirstKillDrop(3, ItemID.LifeCrystal));
                rules.Add(FirstKillDrop(5, ItemID.WoodenCrate));
            }
            else if (npc.type == ModContent.NPCType<TurkortheUngrateful>())
            {
                rules.Add(FirstKillDrop(5, ItemID.IronCrate));
            }
            else if (npc.type == ModContent.NPCType<Ocram>())
            {
                rules.Add(FirstKillDrop(5, ItemID.GoldenCrateHard));
            }

            foreach (var rule in rules)
            {
                npcLoot.Add(rule);
            }
        }

        private static IItemDropRule Drop(int count, int itemID) => ItemDropRule.Common(itemID, minimumDropped: count, maximumDropped: count);

        public static IItemDropRule FirstKillDrop(int amount, int itemID)
        {
            IItemDropRule rule = new LeadingConditionRule(new CrossmodFirstKillCondition());
            rule.OnSuccess(Drop(amount, itemID));
            return rule;
        }
    }
}
