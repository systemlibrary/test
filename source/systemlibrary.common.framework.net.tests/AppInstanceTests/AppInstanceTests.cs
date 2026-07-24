using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.Tests;

[TestClass]
public class AppInstanceTests 
{
    [TestMethod]
    public void AppName__Is_Set()
    {
        var appName = AppInstance.Name;

        Assert.IsTrue(appName.Is(), "App name is invalid: " + appName);
    }

    [TestMethod]
    public void TryOutputLog()
    {
        Log.Dump(AppInstance.Name);
        Log.Dump(AppInstance.Debug);
        Log.Dump(AppInstance.HostName);
        Log.Dump(AppInstance.Salt);
        Log.Dump(AppInstance.Diagnostics);
        Log.Dump(AppInstance.License);
        Log.Dump("Dump 0");
        Log.Trace("Trace 1");
        Log.Information("Info 2");
        Log.Warning("Warning 3");
        Log.Debug("Debug 4 Long textDebug 4 Long textDebug 4 Long texDebug 4 Long textDebug 4 Long tDebug 4 Long textDebug 4 Long tDebug 4 Long textDebug 4 Long ttDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long textDebug 4 Long text");
        Log.Error("Error 5");
        Log.Critical("Critical 6");
        Log.Critical(EnvironmentConfig.IsProd);
        Log.Critical(EnvironmentConfig.IsDevelopment);
        Log.Critical(EnvironmentConfig.IsTest);
        Log.Critical(EnvironmentConfig.IsAutomation);
    }
}