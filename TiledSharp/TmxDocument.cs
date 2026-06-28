using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxDocument
	{
		public string TmxDirectory { get; private set; }

		protected XDocument ReadXml(string filepath)
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			string[] array = new string[0];
			if (entryAssembly != null)
			{
				array = entryAssembly.GetManifestResourceNames();
			}
			string fileResPath = filepath.Replace(Path.DirectorySeparatorChar.ToString(), ".");
			string text = Array.Find<string>(array, (string s) => s.EndsWith(fileResPath));
			XDocument xdocument;
			if (text != null)
			{
				string text2 = File.ReadAllText(text);
				xdocument = XDocument.Load(text2);
				this.TmxDirectory = "";
			}
			else
			{
				xdocument = XDocument.Load(filepath);
				this.TmxDirectory = Path.GetDirectoryName(filepath);
			}
			return xdocument;
		}
	}
}
