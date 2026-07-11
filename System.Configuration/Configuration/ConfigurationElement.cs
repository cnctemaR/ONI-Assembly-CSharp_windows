using System;
using System.Collections;
using System.Xml;

namespace System.Configuration
{
	public abstract class ConfigurationElement
	{
		internal Configuration Configuration
		{
			get
			{
				return this._configuration;
			}
			set
			{
				this._configuration = value;
			}
		}

		internal virtual void InitFromProperty(PropertyInformation propertyInfo)
		{
			this.elementInfo = new ElementInformation(this, propertyInfo);
			this.Init();
		}

		public ElementInformation ElementInformation
		{
			get
			{
				if (this.elementInfo == null)
				{
					this.elementInfo = new ElementInformation(this, null);
				}
				return this.elementInfo;
			}
		}

		internal string RawXml
		{
			get
			{
				return this.rawXml;
			}
			set
			{
				if (this.rawXml == null || value != null)
				{
					this.rawXml = value;
				}
			}
		}

		protected internal virtual void Init()
		{
		}

		protected internal virtual ConfigurationElementProperty ElementProperty
		{
			get
			{
				if (this.elementProperty == null)
				{
					this.elementProperty = new ConfigurationElementProperty(this.ElementInformation.Validator);
				}
				return this.elementProperty;
			}
		}

		[MonoTODO]
		protected ContextInformation EvaluationContext
		{
			get
			{
				if (this.Configuration != null)
				{
					return this.Configuration.EvaluationContext;
				}
				throw new NotImplementedException();
			}
		}

		public ConfigurationLockCollection LockAllAttributesExcept
		{
			get
			{
				if (this.lockAllAttributesExcept == null)
				{
					this.lockAllAttributesExcept = new ConfigurationLockCollection(this, ConfigurationLockType.Attribute | ConfigurationLockType.Exclude);
				}
				return this.lockAllAttributesExcept;
			}
		}

		public ConfigurationLockCollection LockAllElementsExcept
		{
			get
			{
				if (this.lockAllElementsExcept == null)
				{
					this.lockAllElementsExcept = new ConfigurationLockCollection(this, ConfigurationLockType.Element | ConfigurationLockType.Exclude);
				}
				return this.lockAllElementsExcept;
			}
		}

		public ConfigurationLockCollection LockAttributes
		{
			get
			{
				if (this.lockAttributes == null)
				{
					this.lockAttributes = new ConfigurationLockCollection(this, ConfigurationLockType.Attribute);
				}
				return this.lockAttributes;
			}
		}

		public ConfigurationLockCollection LockElements
		{
			get
			{
				if (this.lockElements == null)
				{
					this.lockElements = new ConfigurationLockCollection(this, ConfigurationLockType.Element);
				}
				return this.lockElements;
			}
		}

		public bool LockItem
		{
			get
			{
				return this.lockItem;
			}
			set
			{
				this.lockItem = value;
			}
		}

		[MonoTODO]
		protected virtual void ListErrors(IList list)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected void SetPropertyValue(ConfigurationProperty prop, object value, bool ignoreLocks)
		{
			try
			{
				prop.Validate(value);
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(string.Format("The value for the property '{0}' is not valid. The error is: {1}", prop.Name, ex.Message), ex);
			}
		}

		internal ConfigurationPropertyCollection GetKeyProperties()
		{
			if (this.keyProps != null)
			{
				return this.keyProps;
			}
			ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
				if (configurationProperty.IsKey)
				{
					configurationPropertyCollection.Add(configurationProperty);
				}
			}
			return this.keyProps = configurationPropertyCollection;
		}

		internal ConfigurationElementCollection GetDefaultCollection()
		{
			if (this.defaultCollection != null)
			{
				return this.defaultCollection;
			}
			ConfigurationProperty configurationProperty = null;
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty configurationProperty2 = (ConfigurationProperty)obj;
				if (configurationProperty2.IsDefaultCollection)
				{
					configurationProperty = configurationProperty2;
					break;
				}
			}
			if (configurationProperty != null)
			{
				this.defaultCollection = this[configurationProperty] as ConfigurationElementCollection;
			}
			return this.defaultCollection;
		}

		protected internal object this[ConfigurationProperty property]
		{
			get
			{
				return this[property.Name];
			}
			set
			{
				this[property.Name] = value;
			}
		}

		protected internal object this[string property_name]
		{
			get
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[property_name];
				if (propertyInformation == null)
				{
					throw new InvalidOperationException("Property '" + property_name + "' not found in configuration element");
				}
				return propertyInformation.Value;
			}
			set
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[property_name];
				if (propertyInformation == null)
				{
					throw new InvalidOperationException("Property '" + property_name + "' not found in configuration element");
				}
				this.SetPropertyValue(propertyInformation.Property, value, false);
				propertyInformation.Value = value;
				this.modified = true;
			}
		}

		protected internal virtual ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.map == null)
				{
					this.map = ElementMap.GetMap(base.GetType());
				}
				return this.map.Properties;
			}
		}

		public override bool Equals(object compareTo)
		{
			ConfigurationElement configurationElement = compareTo as ConfigurationElement;
			if (configurationElement == null)
			{
				return false;
			}
			if (base.GetType() != configurationElement.GetType())
			{
				return false;
			}
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
				if (!object.Equals(this[configurationProperty], configurationElement[configurationProperty]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (object obj in this.Properties)
			{
				ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
				object obj2 = this[configurationProperty];
				if (obj2 != null)
				{
					num += obj2.GetHashCode();
				}
			}
			return num;
		}

		internal virtual bool HasValues()
		{
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.ValueOrigin != PropertyValueOrigin.Default)
				{
					return true;
				}
			}
			return false;
		}

		internal virtual bool HasLocalModifications()
		{
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere && propertyInformation.IsModified)
				{
					return true;
				}
			}
			return false;
		}

		protected internal virtual void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			Hashtable hashtable = new Hashtable();
			reader.MoveToContent();
			while (reader.MoveToNextAttribute())
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[reader.LocalName];
				if (propertyInformation == null || (serializeCollectionKey && !propertyInformation.IsKey))
				{
					if (reader.LocalName == "lockAllAttributesExcept")
					{
						this.LockAllAttributesExcept.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockAllElementsExcept")
					{
						this.LockAllElementsExcept.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockAttributes")
					{
						this.LockAttributes.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockElements")
					{
						this.LockElements.SetFromList(reader.Value);
					}
					else if (reader.LocalName == "lockItem")
					{
						this.LockItem = reader.Value.ToLowerInvariant() == "true";
					}
					else if (!(reader.LocalName == "xmlns"))
					{
						if (!(this is ConfigurationSection) || !(reader.LocalName == "configSource"))
						{
							if (!this.OnDeserializeUnrecognizedAttribute(reader.LocalName, reader.Value))
							{
								throw new ConfigurationErrorsException("Unrecognized attribute '" + reader.LocalName + "'.", reader);
							}
						}
					}
				}
				else
				{
					if (hashtable.ContainsKey(propertyInformation))
					{
						throw new ConfigurationErrorsException("The attribute '" + propertyInformation.Name + "' may only appear once in this element.", reader);
					}
					try
					{
						string value = reader.Value;
						this.ValidateValue(propertyInformation.Property, value);
						propertyInformation.SetStringValue(value);
					}
					catch (ConfigurationErrorsException)
					{
						throw;
					}
					catch (ConfigurationException)
					{
						throw;
					}
					catch (Exception ex)
					{
						string text = string.Format("The value for the property '{0}' is not valid. The error is: {1}", propertyInformation.Name, ex.Message);
						throw new ConfigurationErrorsException(text, reader);
					}
					hashtable[propertyInformation] = propertyInformation.Name;
				}
			}
			reader.MoveToElement();
			if (!reader.IsEmptyElement)
			{
				int depth = reader.Depth;
				reader.ReadStartElement();
				reader.MoveToContent();
				PropertyInformation propertyInformation2;
				for (;;)
				{
					if (reader.NodeType != XmlNodeType.Element)
					{
						reader.Skip();
					}
					else
					{
						propertyInformation2 = this.ElementInformation.Properties[reader.LocalName];
						if (propertyInformation2 == null || (serializeCollectionKey && !propertyInformation2.IsKey))
						{
							if (!this.OnDeserializeUnrecognizedElement(reader.LocalName, reader))
							{
								if (propertyInformation2 != null)
								{
									break;
								}
								ConfigurationElementCollection configurationElementCollection = this.GetDefaultCollection();
								if (configurationElementCollection == null || !configurationElementCollection.OnDeserializeUnrecognizedElement(reader.LocalName, reader))
								{
									break;
								}
							}
						}
						else
						{
							if (!propertyInformation2.IsElement)
							{
								goto Block_23;
							}
							if (hashtable.Contains(propertyInformation2))
							{
								goto Block_24;
							}
							ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation2.Value;
							configurationElement.DeserializeElement(reader, serializeCollectionKey);
							hashtable[propertyInformation2] = propertyInformation2.Name;
							if (depth == reader.Depth)
							{
								reader.Read();
							}
						}
					}
					if (depth >= reader.Depth)
					{
						goto IL_03A5;
					}
				}
				throw new ConfigurationErrorsException("Unrecognized element '" + reader.LocalName + "'.", reader);
				Block_23:
				throw new ConfigurationException("Property '" + propertyInformation2.Name + "' is not a ConfigurationElement.");
				Block_24:
				throw new ConfigurationErrorsException("The element <" + propertyInformation2.Name + "> may only appear once in this section.", reader);
			}
			reader.Skip();
			IL_03A5:
			this.modified = false;
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation3 = (PropertyInformation)obj;
				if (!string.IsNullOrEmpty(propertyInformation3.Name) && propertyInformation3.IsRequired && !hashtable.ContainsKey(propertyInformation3) && this.ElementInformation.Properties[propertyInformation3.Name] == null)
				{
					object obj2 = this.OnRequiredPropertyNotFound(propertyInformation3.Name);
					if (!object.Equals(obj2, propertyInformation3.DefaultValue))
					{
						propertyInformation3.Value = obj2;
						propertyInformation3.IsModified = false;
					}
				}
			}
			this.PostDeserialize();
		}

		protected virtual bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return false;
		}

		protected virtual bool OnDeserializeUnrecognizedElement(string element, XmlReader reader)
		{
			return false;
		}

		protected virtual object OnRequiredPropertyNotFound(string name)
		{
			throw new ConfigurationErrorsException("Required attribute '" + name + "' not found.");
		}

		protected virtual void PreSerialize(XmlWriter writer)
		{
		}

		protected virtual void PostDeserialize()
		{
		}

		protected internal virtual void InitializeDefault()
		{
		}

		protected internal virtual bool IsModified()
		{
			return this.modified;
		}

		protected internal virtual void SetReadOnly()
		{
			this.readOnly = true;
		}

		public virtual bool IsReadOnly()
		{
			return this.readOnly;
		}

		protected internal virtual void Reset(ConfigurationElement parentElement)
		{
			if (parentElement != null)
			{
				this.ElementInformation.Reset(parentElement.ElementInformation);
			}
			else
			{
				this.InitializeDefault();
			}
		}

		protected internal virtual void ResetModified()
		{
			this.modified = false;
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				propertyInformation.IsModified = false;
			}
		}

		protected internal virtual bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
		{
			this.PreSerialize(writer);
			if (serializeCollectionKey)
			{
				ConfigurationPropertyCollection keyProperties = this.GetKeyProperties();
				foreach (object obj in keyProperties)
				{
					ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
					writer.WriteAttributeString(configurationProperty.Name, configurationProperty.ConvertToString(this[configurationProperty.Name]));
				}
				return keyProperties.Count > 0;
			}
			bool flag = false;
			foreach (object obj2 in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj2;
				if (!propertyInformation.IsElement && propertyInformation.ValueOrigin != PropertyValueOrigin.Default)
				{
					if (!object.Equals(propertyInformation.Value, propertyInformation.DefaultValue))
					{
						writer.WriteAttributeString(propertyInformation.Name, propertyInformation.GetStringValue());
						flag = true;
					}
				}
			}
			foreach (object obj3 in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation2 = (PropertyInformation)obj3;
				if (propertyInformation2.IsElement)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation2.Value;
					if (configurationElement != null)
					{
						flag = configurationElement.SerializeToXmlElement(writer, propertyInformation2.Name) || flag;
					}
				}
			}
			return flag;
		}

		protected internal virtual bool SerializeToXmlElement(XmlWriter writer, string elementName)
		{
			if (!this.HasValues())
			{
				return false;
			}
			if (elementName != null && elementName != string.Empty)
			{
				writer.WriteStartElement(elementName);
			}
			bool flag = this.SerializeElement(writer, false);
			if (elementName != null && elementName != string.Empty)
			{
				writer.WriteEndElement();
			}
			return flag;
		}

		protected internal virtual void Unmerge(ConfigurationElement source, ConfigurationElement parent, ConfigurationSaveMode updateMode)
		{
			if (parent != null && source.GetType() != parent.GetType())
			{
				throw new ConfigurationException("Can't unmerge two elements of different type");
			}
			foreach (object obj in source.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.ValueOrigin != PropertyValueOrigin.Default)
				{
					PropertyInformation propertyInformation2 = this.ElementInformation.Properties[propertyInformation.Name];
					object value = propertyInformation.Value;
					if (parent == null || !parent.HasValue(propertyInformation.Name))
					{
						propertyInformation2.Value = value;
					}
					else if (value != null)
					{
						object obj2 = parent[propertyInformation.Name];
						if (propertyInformation.IsElement)
						{
							if (obj2 != null)
							{
								ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation2.Value;
								configurationElement.Unmerge((ConfigurationElement)value, (ConfigurationElement)obj2, updateMode);
							}
							else
							{
								propertyInformation2.Value = value;
							}
						}
						else if (!object.Equals(value, obj2) || updateMode == ConfigurationSaveMode.Full || (updateMode == ConfigurationSaveMode.Modified && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere))
						{
							propertyInformation2.Value = value;
						}
					}
				}
			}
		}

		internal bool HasValue(string propName)
		{
			PropertyInformation propertyInformation = this.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin != PropertyValueOrigin.Default;
		}

		internal bool IsReadFromConfig(string propName)
		{
			PropertyInformation propertyInformation = this.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere;
		}

		private void ValidateValue(ConfigurationProperty p, string value)
		{
			ConfigurationValidatorBase validator;
			if (p == null || (validator = p.Validator) == null)
			{
				return;
			}
			if (!validator.CanValidate(p.Type))
			{
				throw new ConfigurationException(string.Format("Validator does not support type {0}", p.Type));
			}
			validator.Validate(p.ConvertFromString(value));
		}

		private string rawXml;

		private bool modified;

		private ElementMap map;

		private ConfigurationPropertyCollection keyProps;

		private ConfigurationElementCollection defaultCollection;

		private bool readOnly;

		private ElementInformation elementInfo;

		private ConfigurationElementProperty elementProperty;

		private Configuration _configuration;

		private ConfigurationLockCollection lockAllAttributesExcept;

		private ConfigurationLockCollection lockAllElementsExcept;

		private ConfigurationLockCollection lockAttributes;

		private ConfigurationLockCollection lockElements;

		private bool lockItem;
	}
}
