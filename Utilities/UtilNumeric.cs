using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class UtilNumeric
    {
        public static bool NumericTypeToDouble(object ValueWithStronglyType, out double o_ValueAsDouble)
        {
            bool o_Successful = false;

            try
            {
                o_ValueAsDouble = Convert.ToDouble(ValueWithStronglyType);
                o_Successful = true;
            }
            catch
            {
                o_ValueAsDouble = default(double);
                o_Successful = false;
            }

            return o_Successful;
        }

        public static dynamic DoubleToNumericType(Type T, double ValueAsDouble)
        {
            dynamic o_ReturnedValue = null; //default(T);

            if (T == typeof(double))
            {
                o_ReturnedValue = ValueAsDouble;
            }
            else if (T == typeof(int))
            {
                int ValueAsInt = 0;
                if (ValueAsDouble > int.MaxValue)
                {
                    ValueAsInt = int.MaxValue;
                }
                else
                {
                    if (ValueAsDouble < int.MinValue)
                    {
                        ValueAsInt = int.MinValue;
                    }
                    else
                    {
                        ValueAsInt = (int)ValueAsDouble;
                    }
                }
                o_ReturnedValue = ValueAsInt;
            }
            else if (T == typeof(uint))
            {
                uint ValueAsUInt = 0;
                if (ValueAsDouble > uint.MaxValue)
                {
                    ValueAsUInt = uint.MaxValue;
                }
                else
                {
                    if (ValueAsDouble < uint.MinValue)
                    {
                        ValueAsUInt = uint.MinValue;
                    }
                    else
                    {
                        ValueAsUInt = (uint)ValueAsDouble;
                    }
                }
                o_ReturnedValue = ValueAsUInt;
            }
            else if (T == typeof(long))
            {
                long ValueAsLong = 0;
                if (ValueAsDouble > long.MaxValue)
                {
                    ValueAsLong = long.MaxValue;
                }
                else
                {
                    if (ValueAsDouble < long.MinValue)
                    {
                        ValueAsLong = long.MinValue;
                    }
                    else
                    {
                        ValueAsLong = (long)ValueAsDouble;
                    }
                }
                o_ReturnedValue = ValueAsLong;
            }
            else if (T == typeof(ulong))
            {
                ulong ValueAsULong = 0;
                if (ValueAsDouble > ulong.MaxValue)
                {
                    ValueAsULong = ulong.MaxValue;
                }
                else
                {
                    if (ValueAsDouble < ulong.MinValue)
                    {
                        ValueAsULong = ulong.MinValue;
                    }
                    else
                    {
                        ValueAsULong = (ulong)ValueAsDouble;
                    }
                }
                o_ReturnedValue = ValueAsULong;
            }
            else if (T == typeof(TimeSpan))
            {
                TimeSpan ValueAsTimeStamp = TimeSpan.FromSeconds(ValueAsDouble);
                o_ReturnedValue = ValueAsTimeStamp;
            }

            return o_ReturnedValue;
        }
    }
}
