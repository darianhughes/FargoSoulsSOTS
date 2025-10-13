global using LumUtils = Luminance.Common.Utilities.Utilities;
using Terraria.ModLoader;

namespace SecretsOfTheSouls
{
	public class SecretsOfTheSouls : Mod
	{
        internal static SecretsOfTheSouls Instance;

        public override void Load()
        {
            Instance = this;
        }
    }
}
