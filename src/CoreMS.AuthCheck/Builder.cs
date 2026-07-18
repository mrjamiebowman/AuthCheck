using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace CoreMS.AuthCheck;

public static class Builder
{
    /******************************************/
    /*          azure app config              */
    /******************************************/

    public static TBuilder ConfigureAzureAppConfiguration<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        bool userAppConfig = builder.Configuration.GetValue<bool>("CONFIG_USE_APPCONFIG");

        if (userAppConfig != true)
        {
            Console.WriteLine("Azure App Configuration DISABLED.");
            return builder;
        }
        
        // launch settings can be prefixed with a "CONFIG_PREFIX" environment variable, e.g., "DEV", "STAGING", "PROD"
        string configPrefix = builder.Configuration.GetValue<string>("CONFIG_PREFIX") ?? string.Empty;

        // ensure it ends with a ":", i.e., "DEV:AZURE_TENANT_ID"
        if (!String.IsNullOrWhiteSpace(configPrefix) && !configPrefix.EndsWith(":"))
        {
            configPrefix += ":";
        }

        // azure app config
        var connStr = builder.Configuration.GetValue<string>($"{configPrefix}AZURE_APPCONFIG_CONNECTION_STRING");
        var labelFilter = builder.Configuration.GetValue<string>($"{configPrefix}AZURE_APPCONFIG_LABEL_FILTER");

        // client id & secret
        var tenantId = builder.Configuration.GetValue<string>($"{configPrefix}AZURE_TENANT_ID");
        var clientId = builder.Configuration.GetValue<string>($"{configPrefix}AZURE_CLIENT_ID");
        var secret = builder.Configuration.GetValue<string>($"{configPrefix}AZURE_CLIENT_SECRET");

        // validate configuration settings
        if (String.IsNullOrWhiteSpace(connStr) ||
            String.IsNullOrWhiteSpace(labelFilter) ||
            String.IsNullOrWhiteSpace(tenantId) ||
            String.IsNullOrWhiteSpace(clientId) ||
            String.IsNullOrWhiteSpace(secret)
            )
        {
            Console.WriteLine("Azure AppConfig / Key Vault settings were not found.");
            return builder;
        }

        Console.WriteLine($"Azure AppConfig / Key Vault. Label Filter: {labelFilter}.");

        // credentials
        var credentials = new ClientSecretCredential(tenantId, clientId, secret);

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            // label
            options.Select(KeyFilter.Any, labelFilter);

            options.Connect(connStr).ConfigureKeyVault(kv =>
            {
                kv.SetCredential(credentials);
            });
        });

        return builder;
    }
}
