namespace HotStats.Services
{
    public class InternetConnectivityService : IInternetConnectivityService
    {
        public bool IsOnline()
        {
            try
            {
                using (WebClients.WebClient.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public interface IInternetConnectivityService
    {
        bool IsOnline();
    }
}