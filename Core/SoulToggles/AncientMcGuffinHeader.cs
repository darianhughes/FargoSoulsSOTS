using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.SoulToggles
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class AncientMcGuffinHeader : SoulHeader
    {
        public override int Item => -1;
    }
}
