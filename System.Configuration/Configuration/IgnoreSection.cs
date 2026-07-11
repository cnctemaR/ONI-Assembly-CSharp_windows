using System;
using System.Xml;

namespace System.Configuration
{
	public sealed class IgnoreSection : ConfigurationSection
	{
		protected internal override bool IsModified()
		{
			return false;
		}

		protected internal override void DeserializeSection(XmlReader xmlReader)
		{
			this.xml = xmlReader.ReadOuterXml();
		}

		[MonoTODO]
		protected internal override void Reset(ConfigurationElement parentSection)
		{
			base.Reset(parentSection);
		}

		[MonoTODO]
		protected internal override void ResetModified()
		{
			base.ResetModified();
		}

		protected internal override string SerializeSection(ConfigurationElement parentSection, string name, ConfigurationSaveMode saveMode)
		{
			return this.xml;
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return IgnoreSection.properties;
			}
		}

		private string xml;

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
