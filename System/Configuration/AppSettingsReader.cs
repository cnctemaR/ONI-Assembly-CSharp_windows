using System;
using System.Collections.Specialized;
using System.Reflection;

namespace System.Configuration
{
	public class AppSettingsReader
	{
		public AppSettingsReader()
		{
			this.appSettings = ConfigurationSettings.AppSettings;
		}

		public object GetValue(string key, Type type)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			string text = this.appSettings[key];
			if (text == null)
			{
				throw new InvalidOperationException("'" + key + "' could not be found.");
			}
			if (type == typeof(string))
			{
				return text;
			}
			MethodInfo method = type.GetMethod("Parse", new Type[] { typeof(string) });
			if (method == null)
			{
				throw new InvalidOperationException("Type " + ((type != null) ? type.ToString() : null) + " does not have a Parse method");
			}
			object obj = null;
			try
			{
				obj = method.Invoke(null, new object[] { text });
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Parse error.", ex);
			}
			return obj;
		}

		private NameValueCollection appSettings;
	}
}
