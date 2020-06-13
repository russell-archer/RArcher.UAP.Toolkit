using System;
using System.Diagnostics;
using System.Reflection;

namespace RArcher.UAP.Toolkit.Common
{
    public static class TypeHelper
    {
        /// <summary>Returns true if the type is a fundametal type like a string, int, double, bool, etc.</summary>
        /// <param name="t">The type to inspect</param>
        /// <returns>Returns true if the type is a string, int, double, bool, etc.</returns>
        public static bool IsFundamentalType(Type t)
        {
            // Types with null values can't be fundamental types (except string)
            if(t == null) return false;

            return t == typeof(string) || t.GetTypeInfo().IsValueType;
        }

        /// <summary>Returns true if a type can be given a null value, false otherwise</summary>
        /// <param name="t">The type to test</param>
        /// <returns>Returns true if a type can be given a null value, false otherwise</returns>
        public static bool TypeCanBeNull(Type t)
        {
            return t == null || !t.GetTypeInfo().IsValueType;
        }

        /// <summary>Returns the string value cast to the appropriate type, or null if the cast failed</summary>
        /// <param name="val">Value as a string</param>
        /// <param name="t">The type to cast the string value to</param>
        /// <returns>Returns the string value cast to the appropriate type, or null if the cast failed</returns>
        public static object CastFundamentalTypeValue(string val, Type t)
        {
            try
            {
                if(t == typeof(int)) return int.Parse(val);
                if(t == typeof(double)) return double.Parse(val);
                if(t == typeof(float)) return float.Parse(val);
                if(t == typeof(bool)) return bool.Parse(val);
                if(t == typeof(short)) return short.Parse(val);
            }
            catch
            {
                Debug.WriteLine("TypeHelper.CastFundamentalTypeValue: Warning: Unable to cast \"{0}\" to {1}", val, t.Name);
            }

            return null;
        }         
    }
}