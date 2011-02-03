using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace EPS.Reflection
{
    public class ParameterInfoComparer : EqualityComparer<ParameterInfo>
    {
        public override bool Equals(ParameterInfo x, ParameterInfo y)
        {
            return x.DefaultValue == y.DefaultValue
                && x.IsIn == y.IsIn && x.IsOptional == y.IsOptional && x.IsOut == y.IsOut
                && x.IsRetval == y.IsRetval && x.ParameterType == y.ParameterType
                && x.Position == y.Position;
        }

        public override int GetHashCode(ParameterInfo obj)
        {
            return (String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}{4}{5}{6}", obj.DefaultValue, obj.IsIn, obj.IsOptional, obj.IsOut,
                obj.IsRetval, obj.ParameterType, obj.Position)).GetHashCode();
        }
    }
}
