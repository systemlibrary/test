namespace SystemLibrary.Common.Framework.Bootstrap;

internal class FrameworkSettings
{
    public _AppSettings App { get; set; }
    public CryptographySettings Cryptography { get; set; }
    public JsonSettings Json { get; set; }
    public LogSettings Log { get; set; }
    public CacheSettings Cache { get; set; }
    public ClientSettings Client { get; set; }
    public MetricsSettings Metrics { get; set; }

    public FrameworkSettings()
    {
        App = new _AppSettings();
        Cryptography = new CryptographySettings();
        Json = new JsonSettings();
        Log = new LogSettings();
        Cache = new CacheSettings();
        Client = new ClientSettings();
        Metrics = new MetricsSettings();
    }
}