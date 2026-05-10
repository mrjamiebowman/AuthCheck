using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MrJB.AuthCheck.ServiceDefaults;

public static class OTel
{
    public static string ApplicationName { get; set; } = "mrjb.authcheck";

    public static string ServiceVersion { get; set; } = "1.0.0";

    public static readonly ActivitySource ActivitySource = new ActivitySource(ApplicationName);

    public static class MetricNames
    {

    }

    public static class Meters
    {
        public static Meter AppMeter = new Meter("mrjb.authcheck", "1.0.0");

        private static Counter<int> Token = AppMeter.CreateCounter<int>(Auth.Names.Token, description: "Tracks when a user is having issues logging in, resetting passwords, etc.");

        public static class Auth
        {
            public static class Names
            {
                private static string _path = $"authcheck";

                public static string Token = $"{_path}.token";
            }

            public static void AddToken(int d = 1) => Token.Add(d);
            public static void AddToken(int d = 1, TagList tagList = default) => Token.Add(d, tagList);
        }
    }
}
