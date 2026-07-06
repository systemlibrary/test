# Configuration

## appsettings.json

`appsettings.json` is used for settings that:

- Should be changeable without recompilation  
- Are module-specific or global configuration values  
- Do not control middleware registration in any shape or form

## Configuration Files

The framework is not limited to `appsettings.json`.

You may define dedicated configuration files such as:

- `<Integration>.<Production>.json`

This file(s) can live in:

- `/Configs`
- `/Configurations`
- Application root
- Or inside `appsettings.json` as a named section without the transformation name.

## Middleware Configuration

Middleware activation is **not** controlled through `appsettings.json`.

Middleware such as:

- `UseAuthentication`
- `UseDeveloperPage`
- `UseFrameworkBeginMiddleware`
- `UseFrameworkEndMiddleware`

is configured through the `FrameworkOptions` C# object model during application startup, either of these:

- `Application.Run(...)`
- `app.UseFrameworkBeginMiddleware(...)`
- `app.UseFrameworkEndMiddleware(...)`

This keeps middleware behavior explicit, strongly typed, and environment-controlled in code rather than in configuration files.

## Usage

All configurations and settings are to be used from their 'Instance' class, as that contains the final valid setting per option.
For instance 'Forward' of LogSettings, you read it from LogInstance.Forward