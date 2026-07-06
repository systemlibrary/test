# Config - Post V1 Rewrite

## Design
Priority order for configuration resolution:
1. KeyVault
2. Environment variables  
3. appsettings.json (local fallback)

No user secrets — plain text files on developer machines are a security risk.

"systemLibraryCommonFramework": {
    "config": {
        "keyVaultUrl": "https://myvault.vault.azure.net/"
    }
}

## Current (V1)
Reads from JSON/XML files and appsettings.json sections only.
Environment variable overrides apply to appsettings.json values via APP_ prefix.
KeyVault support planned for v2.