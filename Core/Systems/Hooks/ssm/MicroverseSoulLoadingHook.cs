using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Hooks.ssm
{
    [ExtendsFromMod(FargoSOTSCrossmod.CommunitySoulsExpansion.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.CommunitySoulsExpansion.Name)]
    public class MicroverseSoulLoadingHook : ModSystem
    {
        private static Hook _hook;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoSOTSConfig.Instance.UnfinishedContent;
        }

        public override void Load()
        {
            if (!ModLoader.TryGetMod("ssm", out var ssm))
                return;

            // ssm.Content.Items.Accessories.MicroverseSoul.IsLoadingEnabled(Mod)
            var targetType = ssm.Code?.GetType("ssm.Content.Items.Accessories.MicroverseSoul");
            if (targetType is null)
                return;

            var target = targetType.GetMethod(
                "IsLoadingEnabled",
                BindingFlags.Instance | BindingFlags.Public
            );
            if (target is null)
                return;

            _hook = new Hook(target, Detour);
        }

        public override void Unload()
        {
            _hook?.Dispose();
            _hook = null;
        }

        private static bool Detour(Func<object, Mod, bool> orig, object self, Mod mod)
        {
            // Keep original behavior and also allow loading if SOTS is present.
            return orig(self, mod) || ModLoader.HasMod("SOTS");
        }
    }
}
