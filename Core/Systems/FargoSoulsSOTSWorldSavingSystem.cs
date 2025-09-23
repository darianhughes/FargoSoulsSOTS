using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargoSoulsSOTS.Core.Systems
{
    public class FargoSoulsSOTSWorldSavingSystem : ModSystem
    {
        public static bool downedConstruct = false;

        //public static List<int> DroppedSummon = [];

        public override void ClearWorld()
        {
            downedConstruct = false;
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte downedFlags = new();
            downedFlags[0] = downedConstruct;
            writer.Write(downedFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte downedFlags = reader.ReadByte();

            downedConstruct = downedFlags[0];
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (WorldGen.generatingWorld)
                return;

            var downed = new List<string>();
            if (downedConstruct)
                downed.Add("downedConstruct");
            tag["downed"] = downed;

            //tag["droppedSummon"] = DroppedSummon;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedConstruct = downed.Contains("downedConstruct");
        }
    }
}
