using System;
using System.Configuration;
using System.Xml;

namespace System.Runtime.Serialization.Configuration
{
	public sealed class ParameterElement : ConfigurationElement
	{
		public ParameterElement()
		{
		}

		public ParameterElement(string typeName)
			: this()
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			this.Type = typeName;
		}

		public ParameterElement(int index)
			: this()
		{
			this.Index = index;
		}

		[ConfigurationProperty("index", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0)]
		public int Index
		{
			get
			{
				return (int)base["index"];
			}
			set
			{
				base["index"] = value;
			}
		}

		[ConfigurationProperty("", DefaultValue = null, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ParameterElementCollection Parameters
		{
			get
			{
				return (ParameterElementCollection)base[""];
			}
		}

		protected override void PostDeserialize()
		{
			this.Validate();
		}

		protected override void PreSerialize(XmlWriter writer)
		{
			this.Validate();
		}

		[ConfigurationProperty("type", DefaultValue = "")]
		[StringValidator(MinLength = 0)]
		public string Type
		{
			get
			{
				return (string)base["type"];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					value = string.Empty;
				}
				base["type"] = value;
			}
		}

		private void Validate()
		{
			PropertyInformationCollection propertyInformationCollection = base.ElementInformation.Properties;
			if (propertyInformationCollection["index"].ValueOrigin == PropertyValueOrigin.Default && propertyInformationCollection["type"].ValueOrigin == PropertyValueOrigin.Default)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(global::System.Runtime.Serialization.SR.GetString("Configuration parameter element must set either type or index.")));
			}
			if (propertyInformationCollection["index"].ValueOrigin != PropertyValueOrigin.Default && propertyInformationCollection["type"].ValueOrigin != PropertyValueOrigin.Default)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(global::System.Runtime.Serialization.SR.GetString("Configuration parameter element can set only one of either type or index.")));
			}
			if (propertyInformationCollection["index"].ValueOrigin != PropertyValueOrigin.Default && this.Parameters.Count > 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(global::System.Runtime.Serialization.SR.GetString("Configuration parameter element must only add params with type.")));
			}
		}

		internal Type GetType(string rootType, Type[] typeArgs)
		{
			return TypeElement.GetType(rootType, typeArgs, this.Type, this.Index, this.Parameters);
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("index", typeof(int), 0, null, new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None),
						new ConfigurationProperty("", typeof(ParameterElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
						new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, int.MaxValue, null), ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		internal readonly Guid identity = Guid.NewGuid();

		private ConfigurationPropertyCollection properties;
	}
}
