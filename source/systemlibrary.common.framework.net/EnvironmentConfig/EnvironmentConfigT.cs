using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Abstract base class for environment configuration with a custom config class and environment enum.
/// Extend this instead of <c>EnvironmentConfig</c> when you need additional properties beyond <c>Name</c>.
/// </summary>
public abstract class EnvironmentConfig<T, TEnvironmentTypeEnum> : Config<T>
    where T : class
    where TEnvironmentTypeEnum : struct, IComparable, IFormattable, IConvertible
{
    TEnvironmentTypeEnum? _EnvironmentType;

    /// <summary>
    /// Returns the current environment as <c>TEnvironmentTypeEnum</c>, or the default enum value if not resolved.
    /// </summary>
    public TEnvironmentTypeEnum EnvironmentType
    {
        get
        {
            if (_EnvironmentType == null)
            {
                _EnvironmentType = EnvironmentInstance.EnvironmentName.ToEnum<TEnvironmentTypeEnum>();
            }

            return _EnvironmentType.Value;
        }
    }
}
