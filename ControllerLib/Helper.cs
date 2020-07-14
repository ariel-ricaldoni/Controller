using System;

namespace ControllerLib
{
    public static class Helper
    {
        public static T EnumParse<T>(String value) where T : Enum
        {
            Object _enum;

            return (Enum.TryParse(typeof(T), value, true, out _enum)) ? (T)_enum : default;
        }
    }
}
