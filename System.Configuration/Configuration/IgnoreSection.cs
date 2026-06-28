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

		protected internal override void DeserializeSection(XmlReader reader)
		{
			this.xml = reader.ReadOuterXml();
		}

		[MonoTODO]
		protected internal override void Reset(ConfigurationElement parentElement)
		{
			base.Reset(parentElement);
		}

		[MonoTODO]
		protected internal override void ResetModified()
		{
			base.ResetModified();
		}

		protected internal override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
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
