using System;
using System.Collections;
using System.Xml;
using Unity;

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

		protected ContextInformation EvaluationContext
		{
			get
			{
				if (this.Configuration != null)
				{
					return this.Configuration.EvaluationContext;
				}
				throw new ConfigurationErrorsException("This element is not currently associated with any context.");
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
		protected virtual void ListErrors(IList errorList)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected void SetPropertyValue(ConfigurationProperty prop, object value, bool ignoreLocks)
		{
			try
			{
				if (value != null)
				{
					prop.Validate(value);
				}
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(string.Format("The value for the property '{0}' on type {1} is not valid.", prop.Name, this.ElementInformation.Type), ex);
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

		protected internal object this[ConfigurationProperty prop]
		{
			get
			{
				return this[prop.Name];
			}
			set
			{
				this[prop.Name] = value;
			}
		}

		protected internal object this[string propertyName]
		{
			get
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[propertyName];
				if (propertyInformation == null)
				{
					throw new InvalidOperationException("Property '" + propertyName + "' not found in configuration element");
				}
				return propertyInformation.Value;
			}
			set
			{
				PropertyInformation propertyInformation = this.ElementInformation.Properties[propertyName];
				if (propertyInformation == null)
				{
					throw new InvalidOperationException("Property '" + propertyName + "' not found in configuration element");
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
			this.elementPresent = true;
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
					else if (!(reader.LocalName == "xmlns") && (!(this is ConfigurationSection) || !(reader.LocalName == "configSource")) && !this.OnDeserializeUnrecognizedAttribute(reader.LocalName, reader.Value))
					{
						throw new ConfigurationErrorsException("Unrecognized attribute '" + reader.LocalName + "'.", reader);
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
						throw new ConfigurationErrorsException(string.Format("The value for the property '{0}' is not valid. The error is: {1}", propertyInformation.Name, ex.Message), reader);
					}
					hashtable[propertyInformation] = propertyInformation.Name;
					ConfigXmlTextReader configXmlTextReader = reader as ConfigXmlTextReader;
					if (configXmlTextReader != null)
					{
						propertyInformation.Source = configXmlTextReader.Filename;
						propertyInformation.LineNumber = configXmlTextReader.LineNumber;
					}
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
								goto Block_22;
							}
							if (hashtable.Contains(propertyInformation2))
							{
								goto Block_23;
							}
							((ConfigurationElement)propertyInformation2.Value).DeserializeElement(reader, serializeCollectionKey);
							hashtable[propertyInformation2] = propertyInformation2.Name;
							if (depth == reader.Depth)
							{
								reader.Read();
							}
						}
					}
					if (depth >= reader.Depth)
					{
						goto IL_0367;
					}
				}
				throw new ConfigurationErrorsException("Unrecognized element '" + reader.LocalName + "'.", reader);
				Block_22:
				throw new ConfigurationErrorsException("Property '" + propertyInformation2.Name + "' is not a ConfigurationElement.");
				Block_23:
				throw new ConfigurationErrorsException("The element <" + propertyInformation2.Name + "> may only appear once in this section.", reader);
			}
			reader.Skip();
			IL_0367:
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

		protected virtual bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
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
			if (this.modified)
			{
				return true;
			}
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.IsElement)
				{
					ConfigurationElement configurationElement = propertyInformation.Value as ConfigurationElement;
					if (configurationElement != null && configurationElement.IsModified())
					{
						this.modified = true;
						break;
					}
				}
			}
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
			this.elementPresent = false;
			if (parentElement != null)
			{
				this.ElementInformation.Reset(parentElement.ElementInformation);
				return;
			}
			this.InitializeDefault();
		}

		protected internal virtual void ResetModified()
		{
			this.modified = false;
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				propertyInformation.IsModified = false;
				ConfigurationElement configurationElement = propertyInformation.Value as ConfigurationElement;
				if (configurationElement != null)
				{
					configurationElement.ResetModified();
				}
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
				if (!propertyInformation.IsElement)
				{
					if (this.saveContext == null)
					{
						throw new InvalidOperationException();
					}
					if (this.saveContext.HasValue(propertyInformation))
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
			if (this.saveContext == null)
			{
				throw new InvalidOperationException();
			}
			if (!this.saveContext.HasValues())
			{
				return false;
			}
			if (elementName != null && elementName != "")
			{
				writer.WriteStartElement(elementName);
			}
			bool flag = this.SerializeElement(writer, false);
			if (elementName != null && elementName != "")
			{
				writer.WriteEndElement();
			}
			return flag;
		}

		protected internal virtual void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
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
					PropertyInformation propertyInformation2 = this.ElementInformation.Properties[propertyInformation.Name];
					object value = propertyInformation.Value;
					if (parentElement == null || !parentElement.HasValue(propertyInformation.Name))
					{
						propertyInformation2.Value = value;
					}
					else if (value != null)
					{
						object obj2 = parentElement[propertyInformation.Name];
						if (!propertyInformation.IsElement)
						{
							if (!object.Equals(value, obj2) || saveMode == ConfigurationSaveMode.Full || (saveMode == ConfigurationSaveMode.Modified && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere))
							{
								propertyInformation2.Value = value;
							}
						}
						else
						{
							ConfigurationElement configurationElement = (ConfigurationElement)value;
							if (!flag || configurationElement.IsModified())
							{
								if (obj2 == null)
								{
									propertyInformation2.Value = value;
								}
								else
								{
									ConfigurationElement configurationElement2 = (ConfigurationElement)obj2;
									((ConfigurationElement)propertyInformation2.Value).Unmerge(configurationElement, configurationElement2, saveMode);
								}
							}
						}
					}
				}
			}
		}

		internal bool HasValue(string propName)
		{
			PropertyInformation propertyInformation = this.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin > PropertyValueOrigin.Default;
		}

		internal bool IsReadFromConfig(string propName)
		{
			PropertyInformation propertyInformation = this.ElementInformation.Properties[propName];
			return propertyInformation != null && propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere;
		}

		internal bool IsElementPresent
		{
			get
			{
				return this.elementPresent;
			}
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
				throw new ConfigurationErrorsException(string.Format("Validator does not support type {0}", p.Type));
			}
			validator.Validate(p.ConvertFromString(value));
		}

		internal bool HasValue(ConfigurationElement parent, PropertyInformation prop, ConfigurationSaveMode mode)
		{
			if (prop.ValueOrigin == PropertyValueOrigin.Default)
			{
				return false;
			}
			if (mode == ConfigurationSaveMode.Modified && prop.ValueOrigin == PropertyValueOrigin.SetHere && prop.IsModified)
			{
				return true;
			}
			object obj = ((parent != null && parent.HasValue(prop.Name)) ? parent[prop.Name] : prop.DefaultValue);
			if (!prop.IsElement)
			{
				return !object.Equals(prop.Value, obj);
			}
			ConfigurationElement configurationElement = (ConfigurationElement)prop.Value;
			ConfigurationElement configurationElement2 = (ConfigurationElement)obj;
			return configurationElement.HasValues(configurationElement2, mode);
		}

		internal virtual bool HasValues(ConfigurationElement parent, ConfigurationSaveMode mode)
		{
			if (mode == ConfigurationSaveMode.Full)
			{
				return true;
			}
			if (this.modified && mode == ConfigurationSaveMode.Modified)
			{
				return true;
			}
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (this.HasValue(parent, propertyInformation, mode))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual void PrepareSave(ConfigurationElement parent, ConfigurationSaveMode mode)
		{
			this.saveContext = new ConfigurationElement.SaveContext(this, parent, mode);
			foreach (object obj in this.ElementInformation.Properties)
			{
				PropertyInformation propertyInformation = (PropertyInformation)obj;
				if (propertyInformation.IsElement)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)propertyInformation.Value;
					if (parent == null || !parent.HasValue(propertyInformation.Name))
					{
						configurationElement.PrepareSave(null, mode);
					}
					else
					{
						ConfigurationElement configurationElement2 = (ConfigurationElement)parent[propertyInformation.Name];
						configurationElement.PrepareSave(configurationElement2, mode);
					}
				}
			}
		}

		public Configuration CurrentConfiguration
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		protected bool HasContext
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		protected virtual string GetTransformedAssemblyString(string assemblyName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected virtual string GetTransformedTypeString(string typeName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
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

		private bool elementPresent;

		private ConfigurationLockCollection lockAllAttributesExcept;

		private ConfigurationLockCollection lockAllElementsExcept;

		private ConfigurationLockCollection lockAttributes;

		private ConfigurationLockCollection lockElements;

		private bool lockItem;

		private ConfigurationElement.SaveContext saveContext;

		private class SaveContext
		{
			public SaveContext(ConfigurationElement element, ConfigurationElement parent, ConfigurationSaveMode mode)
			{
				this.Element = element;
				this.Parent = parent;
				this.Mode = mode;
			}

			public bool HasValues()
			{
				return this.Mode == ConfigurationSaveMode.Full || this.Element.HasValues(this.Parent, this.Mode);
			}

			public bool HasValue(PropertyInformation prop)
			{
				return this.Mode == ConfigurationSaveMode.Full || this.Element.HasValue(this.Parent, prop, this.Mode);
			}

			public readonly ConfigurationElement Element;

			public readonly ConfigurationElement Parent;

			public readonly ConfigurationSaveMode Mode;
		}
	}
}
