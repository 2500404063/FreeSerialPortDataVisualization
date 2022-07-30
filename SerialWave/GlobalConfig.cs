using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialWave
{
    public class WaveItem
    {
        public int Channel { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Label { get; set; } = "";
        public string Type { get; set; } = "";
        public string Color { get; set; } = "";
    }

    internal static class GlobalConfig
    {
        public static string? SerialName;
        public static int BaudRate = 115200;
        public static bool ReadHex = true;
        public static bool LittleEnd = true;
        public static bool StructFormat = false;
        public static bool DecodeEnable = false;
        public static List<WaveItem> WaveItems = new();
        public static int Channels = 1;
        public static string BackgroundColor = "";

        public static string _OldEditedStructure = "";
    }
}
