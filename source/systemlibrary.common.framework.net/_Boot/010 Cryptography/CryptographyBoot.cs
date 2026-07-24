namespace SystemLibrary.Common.Framework.Bootstrap;

internal static partial class CryptographyBoot
{
    internal static void Strap() { }

    static CryptographyBoot()
    {
        CryptographyInstance.KeyDirectory = FrameworkSettingsInstance.Current.Cryptography.KeyDirectory;

        var keys = GetKeys();
        CryptographyInstance.Keys = keys;

        foreach (var a in CryptographyInstance.Keys)
            BootstrapLog.Write(a.KeyStart + " " + a.Source + " " + a.FilePath);

        BootstrapLog.Write(CryptographyInstance.Keys?.Length);

        CryptographyInstance.PrimaryKey = keys.First(x => !x.Source.StartsWith("RSA"));
        CryptographyInstance.RsaKeys = keys.Where(x => x.Source.StartsWith("RSA")).ToArray();
        CryptographyInstance.DecryptKeys = keys.Where(x => !x.Source.StartsWith("RSA")).ToArray();
    }
}
