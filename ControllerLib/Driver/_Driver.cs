using System;
using System.Runtime.InteropServices;

namespace ControllerLib.Driver
{
    public abstract class Driver
    {
        public Int32 GetMarshalSizeOf(Type type)
        {
            return Marshal.SizeOf(type);
        }
    }
}
