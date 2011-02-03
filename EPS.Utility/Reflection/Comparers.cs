using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    //this is a *very* rudimentary comparison routine
    public class MemberInfoComparer : EqualityComparer<MemberInfo>
    {
        public override bool Equals(MemberInfo x, MemberInfo y)
        {
            if (x.Name != y.Name || x.MemberType != y.MemberType)
                return false;

            if (x.MemberType == MemberTypes.Method)
            {
                MethodInfo xMethod = (MethodInfo)x;
                MethodInfo yMethod = (MethodInfo)y;

                Type xReturnType = xMethod.ReturnType, yReturnType = yMethod.ReturnType;

                //comparing ReturnType doesn't work on generic methods -- so we have to do things a little different
                if (xMethod.IsGenericMethod && yMethod.IsGenericMethod)
                {
                    if (xReturnType.IsGenericType && yReturnType.IsGenericType)
                        return (xReturnType.GetGenericTypeDefinition() == yReturnType.GetGenericTypeDefinition())
                            && xMethod.GetParameters().SequenceEqual(yMethod.GetParameters(), new ParameterInfoComparer());

                    //match type names   
                    if (xReturnType.IsGenericParameter && yReturnType.IsGenericParameter)
                        return (xReturnType.Name == yReturnType.Name 
                        && ((!xMethod.GetParameters().Any() && !yMethod.GetParameters().Any())
                        || xMethod.GetParameters().SequenceEqual(yMethod.GetParameters(), new ParameterInfoComparer())));
                }

                //return types match and there are 0 params or param list sequences match
                return (xReturnType == yReturnType
                    && ((!xMethod.GetParameters().Any() && !yMethod.GetParameters().Any())
                    || xMethod.GetParameters().SequenceEqual(yMethod.GetParameters(), new ParameterInfoComparer())));
                    
            }

            return true;
        }

        public override int GetHashCode(MemberInfo obj)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}{1}", obj.MemberType, obj.Name).GetHashCode();
        }
    }
}
