using System.IO;
using System.Threading.Tasks;

namespace HotStats.Services
{
    public class VersionChecker : IVersionChecker
    {
        public async Task<bool> IsVersionOutdated()
        {
            try
            {
                var localVersion = typeof(VersionChecker).Assembly.GetName().Version.ToString();
                var stream =
                    await WebClients.WebClient.OpenReadTaskAsync(
                        "https://github.com/Boenne/HotStats/raw/master/Program/version.txt");
                using (stream)
                using (var streamReader = new StreamReader(stream))
                {
                    var onlineVersion = await streamReader.ReadLineAsync();
                    return localVersion != onlineVersion;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public interface IVersionChecker
    {
        Task<bool> IsVersionOutdated();
    }
}