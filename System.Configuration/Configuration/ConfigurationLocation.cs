using System;
using System.IO;
using System.Xml;

namespace System.Configuration
{
	public class ConfigurationLocation
	{
		internal ConfigurationLocation()
		{
		}

		internal ConfigurationLocation(string path, string xmlContent, Configuration parent, bool allowOverride)
		{
			if (!string.IsNullOrEmpty(path))
			{
				char c = path[0];
				if (c <= '.')
				{
					if (c != ' ' && c != '.')
					{
						goto IL_003C;
					}
				}
				else if (c != '/' && c != '\\')
				{
					goto IL_003C;
				}
				throw new ConfigurationErrorsException("<location> path attribute must be a relative virtual path.  It cannot start with any of ' ' '.' '/' or '\\'.");
				IL_003C:
				path = path.TrimEnd(ConfigurationLocation.pathTrimChars);
			}
			this.path = path;
			this.xmlContent = xmlContent;
			this.parent = parent;
			this.allowOverride = allowOverride;
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		internal bool AllowOverride
		{
			get
			{
				return this.allowOverride;
			}
		}

		internal string XmlContent
		{
			get
			{
				return this.xmlContent;
			}
		}

		internal Configuration OpenedConfiguration
		{
			get
			{
				return this.configuration;
			}
		}

		public Configuration OpenConfiguration()
		{
			if (this.configuration == null)
			{
				if (!this.parentResolved)
				{
					Configuration parentWithFile = this.parent.GetParentWithFile();
					if (parentWithFile != null)
					{
						string configPathFromLocationSubPath = this.parent.ConfigHost.GetConfigPathFromLocationSubPath(this.parent.LocationConfigPath, this.path);
						this.parent = parentWithFile.FindLocationConfiguration(configPathFromLocationSubPath, this.parent);
					}
				}
				this.configuration = new Configuration(this.parent, this.path);
				using (XmlTextReader xmlTextReader = new ConfigXmlTextReader(new StringReader(this.xmlContent), this.path))
				{
					this.configuration.ReadData(xmlTextReader, this.allowOverride);
				}
				this.xmlContent = null;
			}
			return this.configuration;
		}

		internal void SetParentConfiguration(Configuration parent)
		{
			if (this.parentResolved)
			{
				return;
			}
			this.parentResolved = true;
			this.parent = parent;
			if (this.configuration != null)
			{
				this.configuration.Parent = parent;
			}
		}

		private static readonly char[] pathTrimChars = new char[] { '/' };

		private string path;

		private Configuration configuration;

		private Configuration parent;

		private string xmlContent;

		private bool parentResolved;

		private bool allowOverride;
	}
}
