using System;
using System.Xml;

namespace System.Configuration
{
	public sealed class SettingValueElement : ConfigurationElement
	{
		[global::System.MonoTODO]
		public SettingValueElement()
		{
		}

		[global::System.MonoTODO]
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return base.Properties;
			}
		}

		public XmlNode ValueXml
		{
			get
			{
				return this.node;
			}
			set
			{
				this.node = value;
			}
		}

		[global::System.MonoTODO]
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			this.node = new XmlDocument().ReadNode(reader);
		}

		public override bool Equals(object settingValue)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		protected override bool IsModified()
		{
			throw new NotImplementedException();
		}

		protected override void Reset(ConfigurationElement parentElement)
		{
			this.node = null;
		}

		protected override void ResetModified()
		{
			throw new NotImplementedException();
		}

		protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
		{
			if (this.node == null)
			{
				return false;
			}
			this.node.WriteTo(writer);
			return true;
		}

		protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			throw new NotImplementedException();
		}

		private XmlNode node;
	}
}
