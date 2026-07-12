using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.ComponentModel.Design
{
	internal class RuntimeLicenseContext : LicenseContext
	{
		private string GetLocalPath(string fileName)
		{
			Uri uri = new Uri(fileName);
			return uri.LocalPath + uri.Fragment;
		}

		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			if (this.savedLicenseKeys == null || this.savedLicenseKeys[type.AssemblyQualifiedName] == null)
			{
				if (this.savedLicenseKeys == null)
				{
					this.savedLicenseKeys = new Hashtable();
				}
				if (resourceAssembly == null)
				{
					resourceAssembly = Assembly.GetEntryAssembly();
				}
				if (resourceAssembly == null)
				{
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						if (!assembly.IsDynamic)
						{
							string text = this.GetLocalPath(assembly.EscapedCodeBase);
							text = new FileInfo(text).Name;
							Stream stream = assembly.GetManifestResourceStream(text + ".licenses");
							if (stream == null)
							{
								stream = this.CaseInsensitiveManifestResourceStreamLookup(assembly, text + ".licenses");
							}
							if (stream != null)
							{
								DesigntimeLicenseContextSerializer.Deserialize(stream, text.ToUpper(CultureInfo.InvariantCulture), this);
								break;
							}
						}
					}
				}
				else if (!resourceAssembly.IsDynamic)
				{
					string text2 = this.GetLocalPath(resourceAssembly.EscapedCodeBase);
					text2 = Path.GetFileName(text2);
					string text3 = text2 + ".licenses";
					Stream stream2 = resourceAssembly.GetManifestResourceStream(text3);
					if (stream2 == null)
					{
						string text4 = null;
						CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
						string name = resourceAssembly.GetName().Name;
						foreach (string text5 in resourceAssembly.GetManifestResourceNames())
						{
							if (compareInfo.Compare(text5, text3, CompareOptions.IgnoreCase) == 0 || compareInfo.Compare(text5, name + ".exe.licenses", CompareOptions.IgnoreCase) == 0 || compareInfo.Compare(text5, name + ".dll.licenses", CompareOptions.IgnoreCase) == 0)
							{
								text4 = text5;
								break;
							}
						}
						if (text4 != null)
						{
							stream2 = resourceAssembly.GetManifestResourceStream(text4);
						}
					}
					if (stream2 != null)
					{
						DesigntimeLicenseContextSerializer.Deserialize(stream2, text2.ToUpper(CultureInfo.InvariantCulture), this);
					}
				}
			}
			return (string)this.savedLicenseKeys[type.AssemblyQualifiedName];
		}

		private Stream CaseInsensitiveManifestResourceStreamLookup(Assembly satellite, string name)
		{
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			string name2 = satellite.GetName().Name;
			foreach (string text in satellite.GetManifestResourceNames())
			{
				if (compareInfo.Compare(text, name, CompareOptions.IgnoreCase) == 0 || compareInfo.Compare(text, name2 + ".exe.licenses") == 0 || compareInfo.Compare(text, name2 + ".dll.licenses") == 0)
				{
					name = text;
					break;
				}
			}
			return satellite.GetManifestResourceStream(name);
		}

		private static TraceSwitch s_runtimeLicenseContextSwitch = new TraceSwitch("RuntimeLicenseContextTrace", "RuntimeLicenseContext tracing");

		private const int ReadBlock = 400;

		internal Hashtable savedLicenseKeys;
	}
}
