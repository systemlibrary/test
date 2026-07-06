using System.ComponentModel;
using System.Globalization;

namespace SystemLibrary.Common.Framework.Boostrap;

internal class ExtendedBoolConverter : BooleanConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value == null) return false;

        if (value is string s)
        {
            if (s.Length == 0) return false;

            if (s.Length == 1)
            {
                if (s == "1") return true;

                if (s == "0") return false;

                if (s == "y") return true;
                if (s == "Y") return true;
            }
            else if (s.Length == 2)
            {
                if (s.Equals("on", StringComparison.OrdinalIgnoreCase)) return true;
            }
            else if (s.Length == 3)
            {
                if (s.Equals("yes", StringComparison.OrdinalIgnoreCase)) return true;
            }

            if (bool.TryParse(s, out var b))
                return b;

            return false;
        }

        return base.ConvertFrom(context, culture, value);
    }
}
