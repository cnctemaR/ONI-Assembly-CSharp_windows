using System;
using System.IO;

namespace System.ComponentModel
{
	public class LicFileLicenseProvider : LicenseProvider
	{
		public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
		{
			try
			{
				if (context == null || context.UsageMode != LicenseUsageMode.Designtime)
				{
					return null;
				}
				string text = Path.GetDirectoryName(type.Assembly.Location);
				text = Path.Combine(text, type.FullName + ".LIC");
				if (!File.Exists(text))
				{
					return null;
				}
				StreamReader streamReader = new StreamReader(text);
				string text2 = streamReader.ReadLine();
				streamReader.Close();
				if (this.IsKeyValid(text2, type))
				{
					return new LicFileLicense(text2);
				}
			}
			catch
			{
				if (allowExceptions)
				{
					throw;
				}
			}
			return null;
		}

		protected virtual string GetKey(Type type)
		{
			return type.FullName + " is a licensed component.";
		}

		protected virtual bool IsKeyValid(string key, Type type)
		{
			return key != null && key.Equals(this.GetKey(type));
		}
	}
}
