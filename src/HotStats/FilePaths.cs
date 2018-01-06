using System;
using System.IO;

namespace HotStats
{
    public class FilePaths
    {
        public static string DataDir => $"{Environment.CurrentDirectory}/data";
        public static string Data => $"{DataDir}/data.json";
        public static string HeroData => $"{DataDir}/herodata.json";
        public static string Images => $"{Environment.CurrentDirectory}/images";
        public static string Seasons => $"{DataDir}/seasons.json";

        public static string MyDocuments => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            @"Heroes of the Storm\Accounts");
    }
}