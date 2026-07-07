using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

public abstract partial class Config<T> where T : class
{
    static object Lock = new object();

    public static T Current
    {
        get
        {
            if (_Current != null) return _Current;

            var type = typeof(T);

            lock (Lock)
            {
                if (_Current != null) return _Current;

                var configuration = ConfigFinder.FindByType(type);

                _Current = configuration.Get<T>(opt =>
                {
                    opt.ErrorOnUnknownConfiguration = false;
                });

                if (_Current == null)
                    throw new Exception(type.Name + ".json not found or empty. Expected in ~/, ~/Configs/, ~/Configurations/, or as a section in appsettings.json.");

                try
                {
                    DecryptPublicGetSetProperties(_Current, type);
                }
                catch (Exception ex)
                {
                    //if (FrameworkLog.IsDebug)
                    //{
                    //    var error = "[Framework.Debug=true] Decrypting a property in " + type.Name + " threw: " + ex.Message;
                    //    //FrameworkLog.Debug(error);
                    //}
                }

                try
                {
                    SetPublicEnumProperties(configuration, _Current, type);
                }
                catch (Exception ex)
                {
                    //if (FrameworkLog.IsDebug)
                    //{
                    //    var error = "[Framework.Debug=true] A public enum field " + type.Name + " threw: " + ex.Message;
                    //    //FrameworkLog.Debug(error);
                    //}
                }
            }

            return _Current;
        }
    }

    static T _Current;
}