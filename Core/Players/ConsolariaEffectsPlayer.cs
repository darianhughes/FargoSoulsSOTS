using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Players
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ConsolariaEffectsPlayer : ModPlayer
    {
        public bool ostaraEnchant;

        public override void ResetEffects()
        {
            ostaraEnchant = false;
        }

        public override void PostUpdateEquips()
        {
            if (Player.velocity.Y != 0 && ostaraEnchant)
            {
                Player.moveSpeed += 0.15f;
            }
        }
    }
}
