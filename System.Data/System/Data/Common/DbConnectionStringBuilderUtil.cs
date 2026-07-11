using System;
using System.Data.SqlClient;
using System.Reflection;

namespace System.Data.Common
{
	internal static class DbConnectionStringBuilderUtil
	{
		internal static bool ConvertToBoolean(object value)
		{
			string text = value as string;
			if (text == null)
			{
				bool flag;
				try
				{
					flag = Convert.ToBoolean(value);
				}
				catch (InvalidCastException ex)
				{
					throw ADP.ConvertFailed(value.GetType(), typeof(bool), ex);
				}
				return flag;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "true") || StringComparer.OrdinalIgnoreCase.Equals(text, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "false") || StringComparer.OrdinalIgnoreCase.Equals(text, "no"))
			{
				return false;
			}
			string text2 = text.Trim();
			return StringComparer.OrdinalIgnoreCase.Equals(text2, "true") || StringComparer.OrdinalIgnoreCase.Equals(text2, "yes") || (!StringComparer.OrdinalIgnoreCase.Equals(text2, "false") && !StringComparer.OrdinalIgnoreCase.Equals(text2, "no") && bool.Parse(text));
		}

		internal static bool ConvertToIntegratedSecurity(object value)
		{
			string text = value as string;
			if (text == null)
			{
				bool flag;
				try
				{
					flag = Convert.ToBoolean(value);
				}
				catch (InvalidCastException ex)
				{
					throw ADP.ConvertFailed(value.GetType(), typeof(bool), ex);
				}
				return flag;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(text, "true") || StringComparer.OrdinalIgnoreCase.Equals(text, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "false") || StringComparer.OrdinalIgnoreCase.Equals(text, "no"))
			{
				return false;
			}
			string text2 = text.Trim();
			return StringComparer.OrdinalIgnoreCase.Equals(text2, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(text2, "true") || StringComparer.OrdinalIgnoreCase.Equals(text2, "yes") || (!StringComparer.OrdinalIgnoreCase.Equals(text2, "false") && !StringComparer.OrdinalIgnoreCase.Equals(text2, "no") && bool.Parse(text));
		}

		internal static int ConvertToInt32(object value)
		{
			int num;
			try
			{
				num = Convert.ToInt32(value);
			}
			catch (InvalidCastException ex)
			{
				throw ADP.ConvertFailed(value.GetType(), typeof(int), ex);
			}
			return num;
		}

		internal static string ConvertToString(object value)
		{
			string text;
			try
			{
				text = Convert.ToString(value);
			}
			catch (InvalidCastException ex)
			{
				throw ADP.ConvertFailed(value.GetType(), typeof(string), ex);
			}
			return text;
		}

		internal static bool TryConvertToApplicationIntent(string value, out ApplicationIntent result)
		{
			if (StringComparer.OrdinalIgnoreCase.Equals(value, "ReadOnly"))
			{
				result = ApplicationIntent.ReadOnly;
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(value, "ReadWrite"))
			{
				result = ApplicationIntent.ReadWrite;
				return true;
			}
			result = ApplicationIntent.ReadWrite;
			return false;
		}

		internal static bool IsValidApplicationIntentValue(ApplicationIntent value)
		{
			return value == ApplicationIntent.ReadOnly || value == ApplicationIntent.ReadWrite;
		}

		internal static string ApplicationIntentToString(ApplicationIntent value)
		{
			if (value == ApplicationIntent.ReadOnly)
			{
				return "ReadOnly";
			}
			return "ReadWrite";
		}

		internal static ApplicationIntent ConvertToApplicationIntent(string keyword, object value)
		{
			string text = value as string;
			if (text != null)
			{
				ApplicationIntent applicationIntent;
				if (DbConnectionStringBuilderUtil.TryConvertToApplicationIntent(text, out applicationIntent))
				{
					return applicationIntent;
				}
				text = text.Trim();
				if (DbConnectionStringBuilderUtil.TryConvertToApplicationIntent(text, out applicationIntent))
				{
					return applicationIntent;
				}
				throw ADP.InvalidConnectionOptionValue(keyword);
			}
			else
			{
				ApplicationIntent applicationIntent2;
				if (value is ApplicationIntent)
				{
					applicationIntent2 = (ApplicationIntent)value;
				}
				else
				{
					if (value.GetType().GetTypeInfo().IsEnum)
					{
						throw ADP.ConvertFailed(value.GetType(), typeof(ApplicationIntent), null);
					}
					try
					{
						applicationIntent2 = (ApplicationIntent)Enum.ToObject(typeof(ApplicationIntent), value);
					}
					catch (ArgumentException ex)
					{
						throw ADP.ConvertFailed(value.GetType(), typeof(ApplicationIntent), ex);
					}
				}
				if (DbConnectionStringBuilderUtil.IsValidApplicationIntentValue(applicationIntent2))
				{
					return applicationIntent2;
				}
				throw ADP.InvalidEnumerationValue(typeof(ApplicationIntent), (int)applicationIntent2);
			}
		}

		private const string ApplicationIntentReadWriteString = "ReadWrite";

		private const string ApplicationIntentReadOnlyString = "ReadOnly";
	}
}
