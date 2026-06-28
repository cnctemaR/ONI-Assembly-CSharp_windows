using System;
using System.Xml;

namespace System.Configuration
{
	public sealed class DefaultSection : ConfigurationSection
	{
		protected internal override void DeserializeSection(XmlReader xmlReader)
		{
			if (base.RawXml == null)
			{
				base.RawXml = xmlReader.ReadOuterXml();
			}
			else
			{
				xmlReader.Skip();
			}
		}

		[MonoTODO]
		protected internal override bool IsModified()
		{
			return base.IsModified();
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

		[MonoTODO]
		protected internal override string SerializeSection(ConfigurationElement parentSection, string name, ConfigurationSaveMode saveMode)
		{
			return base.SerializeSection(parentSection, name, saveMode);
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return DefaultSection.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
