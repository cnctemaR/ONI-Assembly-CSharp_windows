using System;
using System.Globalization;

namespace System.Data.Common
{
	internal sealed class DbConnectionStringBuilderHelper
	{
		public static int ConvertToInt32(object value)
		{
			return int.Parse(value.ToString(), CultureInfo.InvariantCulture);
		}

		public static bool ConvertToBoolean(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("null value cannot be converted to boolean");
			}
			string text = value.ToString().ToUpper().Trim();
			if (text == "YES" || text == "TRUE")
			{
				return true;
			}
			if (text == "NO" || text == "FALSE")
			{
				return false;
			}
			throw new ArgumentException(string.Format("Invalid boolean value: {0}", value.ToString()));
		}
	}
}
