using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CoreMS.AuthCheck.ServiceDefaults;

public static class OTel
{
    public static string ApplicationName { get; set; } = "CoreMS.AuthCheck";

    public static string ServiceVersion { get; set; } = "1.0.0";

    public static string BasePath { get; set; } = "corems.authcheck";

    public static readonly ActivitySource ActivitySource = new ActivitySource(ApplicationName);

    public static class MetricNames
    {

    }

    public static class Meters
    {
        public static Meter AppMeter = new Meter(ApplicationName, "1.0.0");

        private static Counter<int> DiscoveryDocument = AppMeter.CreateCounter<int>(Auth.Names.DiscoveryDocument, description: "Tracks when a discovery document is checked.");

        private static Counter<int> Token = AppMeter.CreateCounter<int>(Auth.Names.Token, description: "Tracks when a JWT token is requested.");

        public static class Auth
        {
            public static class Names
            {
                public static string DiscoveryDocument = $"{BasePath}.discoverydocument";

                public static string Token = $"{BasePath}.token";
            }

            public static void AddToken(int d = 1, TagList tagList = default) => Token.Add(d, tagList);

            public static void AddDiscoveryDocument(int d = 1, TagList tagList = default) => DiscoveryDocument.Add(d, tagList);
        }
    }
}
