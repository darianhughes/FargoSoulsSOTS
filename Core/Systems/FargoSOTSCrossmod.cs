using Terraria.ModLoader;

namespace FargoSoulsSOTS
{
    public static class FargoSOTSCrossmod
    {
        public static class FargowiltasCrossmod
        {
            public const string Name = "FargowiltasCrossmod";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class CalamityMod
        {
            public const string Name = "CalamityMod";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }

        public static class CommunitySoulsExpansion
        {
            public const string Name = "ssm";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class SOTSBardHealer
        {
            public const string Name = "SOTSBardHealer";
            public static bool Loaded => ModLoader.HasMod(Name);
            public static Mod Mod => ModLoader.GetMod(Name);
        }

        public static class RevengeancePlus
        {
            public const string Name = "RevengeancePlus";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);

        }

        public static class InfernalEclipseAPI
        {
            public const string Name = "InfernalEclipseAPI";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }

        public static class MagicStorage
        {
            public const string Name = "MagicStorage";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }

        public static class MusicDisplay
        {
            public const string Name = "MusicDisplay";
            public static Mod Mod => ModLoader.GetMod(Name);
            public static bool Loaded => ModLoader.HasMod(Name);
        }
    }
}
