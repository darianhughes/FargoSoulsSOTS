using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargoSoulsSOTS.Common.ItemChanges;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargoSoulsSOTS.Core.Systems.Hooks
{
    public class FlashsparkBootsAdjustmentHook : ModSystem
    {
        /*
        private static Hook _uaHook;
        private static MethodInfo _uaMI;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out var sots) ||
                !sots.TryFind("FlashsparkBoots", out ModItem boots))
                return;

            _uaMI = boots.GetType().GetMethod(
                "UpdateAccessory",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (_uaMI != null)
                _uaHook = new Hook(_uaMI, (Action<ModItem, Player, bool>)Patched_UpdateAccessory);
        }

        public override void Unload()
        {
            _uaHook?.Dispose();
            _uaHook = null;
            _uaMI = null;
        }

        private static void Patched_UpdateAccessory(ModItem self, Player player, bool hideVisual)
        {
            if (player.HasEffect<FlashsparkEffect>())
            {
                // Call ORIGINAL SOTS logic
                _uaHook.Undo();
                try
                {
                    _uaMI.Invoke(self, new object[] { player, hideVisual });
                }
                finally
                {
                    _uaHook.Apply();
                }

                player.waterWalk = false;
                player.fireWalk = false;
                player.iceSkate = false;
                player.moveSpeed -= 0.2f;

                return;
            }

            player.buffImmune[BuffID.OnFire] = true;
            player.lavaMax += 600;
            player.rocketBoots = player.vanityRocketBoots = Math.Max(player.rocketBoots, 4);
            player.accRunSpeed = Math.Max(player.accRunSpeed, 7f);
            player.hellfireTreads = true;
        }
        */
    }
}
