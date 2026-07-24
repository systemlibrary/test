using System.Runtime.InteropServices;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Licensing;

internal static class License
{
    internal enum Tier
    {
        Pro
    }

    [DllImport("SystemLibrary.Common.Framework.LicenseEncKey", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr LicenseEncKey();

    static string _GetLicenseEncKey;
    static string GetLicenseEncKey
    {
        get
        {
            if (_GetLicenseEncKey == null)
            {
                try
                {
                    _GetLicenseEncKey = Marshal.PtrToStringAnsi(LicenseEncKey());
                }
                catch
                {
                    // Swallow: most likely unsupported OS/arch. Supported: WIN64, WINARM, Linux64, LinuxARM, macOS ARM64
                }
                if (_GetLicenseEncKey == null)
                {
                    _GetLicenseEncKey = "";
                }
            }

            return _GetLicenseEncKey;
        }
    }

    static Dictionary<Tier, bool> TiersLicensed = new Dictionary<Tier, bool>();

    static object _lock = new object();

    // All development environments grants access to all tiers
    internal static bool BypassEnvironmentCheck;
    internal static string TestLicense;

    internal static bool Pro()
    {
        return IsTierValid(Tier.Pro);
    }

    internal static string Generate(string companyId, string companyInformation, Tier tier)
    {
        companyId = companyId?.Replace("|", "#");
        companyInformation = companyInformation?.Replace("|", "#").MaxLength(48);

        return (companyId + "|" + companyInformation + "|" + ((int)tier) + "|" + DateTime.UtcNow.ToString("yyyy-MM-dd")).Encrypt(Marshal.PtrToStringAnsi(LicenseEncKey())).ToBase64(urlSafe: true);
    }

    static bool IsTierValid(Tier tier)
    {
        if (TiersLicensed.ContainsKey(tier) && TestLicense == null)
            return TiersLicensed[tier];

        lock (_lock)
        {
            if (TiersLicensed.TryGetValue(tier, out bool isValid2) && TestLicense == null) return isValid2;

            TiersLicensed[tier] = GetTierState(tier);

            FrameworkLog.Debug("[License] " + tier + ": " + TiersLicensed[tier]);
        }

        return TiersLicensed[tier];
    }

    static bool GetTierState(Tier licenseTier)
    {
        var tmpLicense = TestLicense ?? AppInstance.License;

        var license = "";

        if (EnvironmentConfig.IsDevelopment)
        {
            if (!BypassEnvironmentCheck)
            {
                if (tmpLicense.Is())
                {
                    try
                    {
                        license = tmpLicense.FromBase64(urlSafe: true).Decrypt(GetLicenseEncKey);
                    }
                    catch
                    {
                        Log.Error("[License] invalid, but development environment grants access");
                    }
                }
                return true;
            }
        }
        FrameworkLog.Debug("[License] checking " + tmpLicense.MaxLength(7) + "...");

        if (tmpLicense.IsNot()) return false;

        try
        {
            license = tmpLicense.FromBase64(urlSafe: true).Decrypt(GetLicenseEncKey);
        }
        catch
        {
            return Invalid("invalid");
        }

        var parts = license.Split('|');

        if (parts.Length != 4) return Invalid("invalid format");

        var id = parts[0];
        var information = parts[1];
        var tier = parts[2].ToEnum<Tier>();
        var created = parts[3];

        if (information == "SystemLibrary") return true;

        if (id.IsNot())
        {
            return Invalid("invalid company id");
        }

        if (information.IsNot())
        {
            return Invalid("invalid company information");
        }

        var tiers = EnumExtensions<Tier>.GetEnums();

        var isTierLowerOrEqual = false;

        foreach (var tierModel in tiers)
        {
            if (licenseTier == tierModel)
            {
                isTierLowerOrEqual = true;
                break;
            }
            if (tier == tierModel)
            {
                break;
            }
        }

        if (!isTierLowerOrEqual)
            return Invalid("invalid plan");

        // No date, means no expiry and previous parts have matched, we return true
        if (created.IsNot()) return true;

        var expires = created.ToDateTime().AddYears(1).Date;

        var today = DateTime.UtcNow.Date;

        if (expires <= today)
            return Invalid("expired at " + expires.ToString("yyyy-MM-dd"));

        if ((expires - today).TotalDays <= 14)
            Log.Error("license " + tmpLicense.MaxLength(7) + "... expires soon: " + expires.ToString("yyyy-MM-dd"));

        return true;
    }

    static bool Invalid(string message)
    {
        Log.Error("[License] " + message);

        return false;
    }
}
