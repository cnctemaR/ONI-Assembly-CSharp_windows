using System;
using System.Reflection;
using System.Xml;

namespace System.Configuration
{
	public sealed class SettingValueElement : ConfigurationElement
	{
		[MonoTODO]
		public SettingValueElement()
		{
		}

		[MonoTODO]
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

		[MonoTODO]
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			this.original = new XmlDocument().ReadNode(reader);
			this.node = this.original.CloneNode(true);
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
			return this.original != this.node;
		}

		protected override void Reset(ConfigurationElement parentElement)
		{
			this.node = null;
		}

		protected override void ResetModified()
		{
			this.node = this.original;
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
			if (parentElement != null && sourceElement.GetType() != parentElement.GetType())
			{
				throw new ConfigurationErrorsException("Can't unmerge two elements of different type");
			}
			bool flag = saveMode == ConfigurationSaveMode.Minimal || saveMode == ConfigurationSaveMode.Modified;
			foreach (object obj in sourceElement.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.ValueOrigin != PropertyValueOrigin.Default)
				{
					PropertyInformation propertyInformation2 = base.ElementInformation.Properties[propertyInformation.Name];
					object value = propertyInformation.Value;
					if (parentElement == null || !this.HasValue(parentElement, propertyInformation.Name))
					{
						propertyInformation2.Value = value;
					}
					else if (value != null)
					{
						object item = this.GetItem(parentElement, propertyInformation.Name);
						if (!this.PropertyIsElement(propertyInformation))
						{
							if (!object.Equals(value, item) || saveMode == ConfigurationSaveMode.Full || (saveMode == ConfigurationSaveMode.Modified && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere))
							{
								propertyInformation2.Value = value;
							}
						}
						else
						{
							ConfigurationElement configurationElement = (ConfigurationElement)value;
							if (!flag || this.ElementIsModified(configurationElement))
							{
								if (item == null)
								{
									propertyInformation2.Value = value;
								}
								else
								{
									ConfigurationElement configurationElement2 = (ConfigurationElement)item;
									ConfigurationElement configurationElement3 = (ConfigurationElement)propertyInformation2.Value;
									this.ElementUnmerge(configurationElement3, configurationElement, configurationElement2, saveMode);
								}
							}
						}
					}
				}
			}
		}

		private bool HasValue(ConfigurationElement element, string propName)
		{
			PropertyInformation propertyInformation = element.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin > PropertyValueOrigin.Default;
		}

		private object GetItem(ConfigurationElement element, string property)
		{
			PropertyInformation propertyInformation = base.ElementInformation.Properties[property];
			if (propertyInformation == null)
			{
				throw new InvalidOperationException("Property '" + property + "' not found in configuration element");
			}
			return propertyInformation.Value;
		}

		private bool PropertyIsElement(PropertyInformation prop)
		{
			return typeof(ConfigurationElement).IsAssignableFrom(prop.Type);
		}

		private bool ElementIsModified(ConfigurationElement element)
		{
			return (bool)element.GetType().GetMethod("IsModified", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(element, new object[0]);
		}

		private void ElementUnmerge(ConfigurationElement target, ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			target.GetType().GetMethod("Unmerge", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, new object[] { sourceElement, parentElement, saveMode });
		}

		private XmlNode node;

		private XmlNode original;
	}
}
