using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Magicallity.Time.Shared
{
    public static class Extensions
    {
        public static T GetArgAs<T>(this IEnumerable<object> list, int index, T defaultValue = default(T))
        {
            var argValue = list.ElementAtOrDefault(index);
            if (argValue != null)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        return (T)converter.ConvertFromString(argValue.ToString());
                    }
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }
    }
}
