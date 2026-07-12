using System;
using System.Configuration;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Configuration
{
	public sealed class DataContractSerializerSection : ConfigurationSection
	{
		[SecurityCritical]
		[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
		internal static DataContractSerializerSection UnsafeGetSection()
		{
			DataContractSerializerSection dataContractSerializerSection = (DataContractSerializerSection)ConfigurationManager.GetSection(ConfigurationStrings.DataContractSerializerSectionPath);
			if (dataContractSerializerSection == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(global::System.Runtime.Serialization.SR.GetString("Failed to load configuration section for dataContractSerializer.")));
			}
			return dataContractSerializerSection;
		}

		[ConfigurationProperty("declaredTypes", DefaultValue = null)]
		public DeclaredTypeElementCollection DeclaredTypes
		{
			get
			{
				return (DeclaredTypeElementCollection)base["declaredTypes"];
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
						new ConfigurationProperty("declaredTypes", typeof(DeclaredTypeElementCollection), null, null, null, ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
