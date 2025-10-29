using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SecretsOfTheSouls.Core.Systems
{
    public class SecretsOfTheSoulsWorldSavingSystem : ModSystem
    {
        public static bool downedConstruct = false;
        public static bool downedTreasureSlime = false;
        //public static bool curseTextPlayed = false;

        //public static List<int> DroppedSummon = [];

        public override void ClearWorld()
        {
            downedConstruct = false;
            downedTreasureSlime = false;
            //curseTextPlayed = false;
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte downedFlags = new();
            downedFlags[0] = downedConstruct;
            downedFlags[1] = downedTreasureSlime;
            //downedFlags[2] = curseTextPlayed;
            writer.Write(downedFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte downedFlags = reader.ReadByte();

            downedConstruct = downedFlags[0];
            downedTreasureSlime = downedFlags[1];
            //curseTextPlayed = downedFlags[2];
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (WorldGen.generatingWorld)
                return;

            var downed = new List<string>();
            if (downedConstruct)
                downed.Add("downedConstruct");
            if (downedTreasureSlime)
                downed.Add("downedTreasureSlime");
            //if (curseTextPlayed)
                //downed.Add("curseTextPlayed");
            tag["downed"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedConstruct = downed.Contains("downedConstruct");
            downedTreasureSlime = downed.Contains("downedTreasureSlime");
            //curseTextPlayed = downed.Contains("curseTextPlayed");
        }
    }
}
