using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CloudBeat.Kit.MSTest.Attributes
{
    public class CbDataSourceAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new object[] { 0, 10, 0 };
            yield return new object[] { 10, 0, 0 };
            yield return new object[] { 1, 10, 10 };
            yield return new object[] { 10, 1, 10 };
            yield return new object[] { 2, 4, 8 };
        }
        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if (data != null)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0} - multiplicand: {1}, multiplier: {2}", methodInfo.Name, data[0], data[1]);
            }
            return null;
        }
    }
}
