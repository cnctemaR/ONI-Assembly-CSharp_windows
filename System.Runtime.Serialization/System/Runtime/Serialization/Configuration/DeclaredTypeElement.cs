using System;
using System.Configuration;
using System.Security;

namespace System.Runtime.Serialization.Configuration
{
	public sealed class DeclaredTypeElement : ConfigurationElement
	{
		public DeclaredTypeElement()
		{
		}

		public DeclaredTypeElement(string typeName)
			: this()
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			this.Type = typeName;
		}

		[ConfigurationProperty("", DefaultValue = null, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public TypeElementCollection KnownTypes
		{
			get
			{
				return (TypeElementCollection)base[""];
			}
		}

		[ConfigurationProperty("type", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
		[DeclaredTypeValidator]
		public string Type
		{
			get
			{
				return (string)base["type"];
			}
			set
			{
				base["type"] = value;
			}
		}

		[SecuritySafeCritical]
		protected override void PostDeserialize()
		{
			if (base.EvaluationContext.IsMachineLevel)
			{
				return;
			}
			if (!PartialTrustHelpers.IsInFullTrust())
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(global::System.Runtime.Serialization.SR.GetString("Failed to load configuration section for dataContractSerializer.")));
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("", typeof(TypeElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
						new ConfigurationProperty("type", typeof(string), string.Empty, null, new DeclaredTypeValidator(), ConfigurationPropertyOptions.IsKey)
					};
				}
				return this.properties;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
