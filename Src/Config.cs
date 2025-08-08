using dotenv.net;

namespace Src
{
    public class IrisConfig
    {
        public static (int apiId, string apiHash, string botToken, string PathSession, string Commands) getSetting()
        {
            DotEnv.Load();
            int apiId = int.Parse(Environment.GetEnvironmentVariable("API_ID")!);
            string apiHash = Environment.GetEnvironmentVariable("API_HASH")!;
            string botToken = Environment.GetEnvironmentVariable("BOT_TOKEN")!;
            string PathSession = Environment.GetEnvironmentVariable("SESSION_PATH")!;
            string Commands = Environment.GetEnvironmentVariable("SQLITE_COMMANDS")!;

            return (apiId, apiHash, botToken, PathSession, Commands);
        }
    }
}