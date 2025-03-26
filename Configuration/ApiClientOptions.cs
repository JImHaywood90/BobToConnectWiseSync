namespace AzureApiPoc.Configuration
{
    public class ApiClientOptions
    {
        public string BaseUri { get; set; } = string.Empty;
        public Dictionary<string, string>? DefaultHeaders { get; set; } = new();
    }

    public class ApiClientsConfig : Dictionary<string, ApiClientOptions> { }
}