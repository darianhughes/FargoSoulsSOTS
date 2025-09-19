using Terraria.ID;
using Terraria;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;
using SOTS.Items.Pyramid;
using SOTS.Items.Conduit;
using SOTS;
using SOTS.Void;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Core.Players;
using FargoSoulsSOTS.Common;
using FargoSoulsSOTS.Content.Projectiles.Masomode;
using Steamworks;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using SOTS.NPCs.Boss;
using System;

namespace FargoSoulsSOTS.Content.Items.Accessories.Enchantments
{
    public class CursedEnchant : BaseEnchant
    {
        public override Color nameColor => new(185, 173, 149);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CursedEffect>(Item);
            player.AddEffect<TinyPlanetoidEffect>(Item);
            player.AddEffect<CursedAppleEffect>(Item);
            player.AddEffect<GhostPepperMinionEffect>(Item);
        }

        public override void EquipFrameEffects(Player player, EquipType type)
        {
            if (player.HasEffect<CursedEffect>())
            {
                SOTSPlayer.ModPlayer(player).petPepper = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CursedHood>()
                .AddIngredient<CursedRobe>()
                .AddIngredient<TinyPlanetoid>()
                .AddIngredient<CursedImpale>()
                .AddIngredient<CurseballTome>()
                .AddIngredient<CursedApple>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class CursedEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<CursedEnchant>();

        public override void OnHitByEither(Player player, NPC npc, Projectile proj)
        {
            var mp = player.GetModPlayer<FargoSOTSPlayer>();

            int current = CursedEffect.CountCursedForOwner(player.whoAmI);
            if (current >= mp.MaxCursedPerPlayer)
                return;

            for (int i = 0; i < Main.maxNPCs && current < mp.MaxCursedPerPlayer; i++)
            {
                NPC curseNPC = Main.npc[i];

                if (isUncursable(curseNPC))
                    continue;
                if (Vector2.Distance(player.Center, curseNPC.Center) > FargoSOTSPlayer.CurseRadius)
                    continue;

                var gn = curseNPC.GetGlobalNPC<FargoSOTSGlobalNPC>();
                if (gn.IsCursed && gn.CursedOwner == player.whoAmI)
                    continue;

                // Permanent until NPC dies or owner dies
                gn.ApplyCurse(player.whoAmI, curseNPC);

                curseNPC.AddBuff(ModContent.BuffType<CursedVision>(), 2);

                if (Main.myPlayer == player.whoAmI)
                {
                    float above = MathF.Max(20f, curseNPC.height * 0.60f) + curseNPC.gfxOffY;
                    Vector2 spawnPos = curseNPC.Top + new Vector2(0f, -above);

                    int projId = Projectile.NewProjectile(
                        player.GetSource_FromThis(),
                        spawnPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<Keystone>(),
                        0, 0f, player.whoAmI,
                        curseNPC.whoAmI
                    );

                    gn.AttachedKeystoneId = projId;

                    var p = Main.projectile[projId];
                    p.ai[0] = curseNPC.whoAmI;
                    p.Center = spawnPos;
                    p.netUpdate = true;
                }

                current++;
            }
        }

        public static bool isUncursable(NPC curseNPC)
        {
            if (!curseNPC.active || !curseNPC.CanBeChasedBy() || curseNPC.friendly || curseNPC.boss)
                return true;

            int[] extraUncurseableNPCs =
            {
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                ModContent.NPCType<TrojanSquirrelHead>(),
                ModContent.NPCType<TrojanSquirrel>(),
                ModContent.NPCType<TrojanSquirrelArms>(),
                ModContent.NPCType<TrojanSquirrelLimb>(),
                ModContent.NPCType<TrojanSquirrelPart>(),
                ModContent.NPCType<SubspaceSerpentHead>(),
                ModContent.NPCType<SubspaceSerpentBody>(),
                ModContent.NPCType<SubspaceSerpentTail>(),
            };

            foreach (int id in extraUncurseableNPCs)
            {
                if (id == curseNPC.type)
                    return true;
            }

            return false;
        }

        public static int CountCursedForOwner(int ownerId)
        {
            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (!Main.npc[i].active) continue;
                var gn = Main.npc[i].GetGlobalNPC<FargoSOTSGlobalNPC>();
                if (gn.IsCursed && gn.CursedOwner == ownerId)
                    count++;
            }
            return count;
        }
    }

    public class TinyPlanetoidEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<TinyPlanetoid>();

        public override void PostUpdate(Player player)
        {
            player.VoidPlayer().voidMeterMax2 += 50;
        }
    }

    public class CursedAppleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<CursedApple>();
        public override void PostUpdate(Player player)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            VoidPlayer.ModPlayer(player).soulsOnKill += 2;
        }
    }

    public class GhostPepperMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<CursedApple>();
        public override bool MinionEffect => true;
    }
}
