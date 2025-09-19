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

                if (!curseNPC.active || !curseNPC.CanBeChasedBy() || curseNPC.friendly || curseNPC.boss)
                    continue;
                if (Vector2.Distance(player.Center, curseNPC.Center) > FargoSOTSPlayer.CurseRadius)
                    continue;

                var gn = curseNPC.GetGlobalNPC<FargoSOTSGlobalNPC>();
                if (gn.IsCursed && gn.CursedOwner == player.whoAmI)
                    continue;

                // Permanent until NPC dies or owner dies
                gn.ApplyCurse(player.whoAmI, curseNPC);

                // Debuff icon on THIS npc
                curseNPC.AddBuff(ModContent.BuffType<CursedVision>(), 2);

                // Keystone anchored ABOVE THIS npc
                if (Main.myPlayer == player.whoAmI)
                {
                    Vector2 spawnPos = curseNPC.Top - new Vector2(0f, Keystone.HoverOffsetY);
                    int projId = Projectile.NewProjectile(
                        player.GetSource_FromThis(),
                        spawnPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<Keystone>(),
                        0, 0f, player.whoAmI,
                        curseNPC.whoAmI // ai[0] = host id
                    );

                    gn.AttachedKeystoneId = projId;

                    if (Main.projectile.IndexInRange(projId))
                    {
                        var p = Main.projectile[projId];
                        p.ai[0] = curseNPC.whoAmI;     // be explicit
                        p.Center = spawnPos;           // be explicit
                        p.netUpdate = true;
                    }
                }

                current++;
            }
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
