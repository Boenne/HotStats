using System.Collections;
using System.Linq;
using System.Reflection;

namespace HotStats
{
    public static class Resources
    {
        static Resources()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            {
                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    ResourceNames = reader.Cast<DictionaryEntry>().Select(entry =>
                        ((string) entry.Key).Replace("%20", " ")).ToArray();
                }
            }
        }

        public static string[] ResourceNames { get; set; }
    }
}