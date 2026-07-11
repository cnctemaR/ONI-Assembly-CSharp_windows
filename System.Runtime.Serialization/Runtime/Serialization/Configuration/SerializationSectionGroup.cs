using System;
using System.Configuration;

namespace System.Runtime.Serialization.Configuration
{
	public sealed class SerializationSectionGroup : ConfigurationSectionGroup
	{
		public static SerializationSectionGroup GetSectionGroup(Configuration config)
		{
			if (config == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("config");
			}
			return (SerializationSectionGroup)config.SectionGroups["system.runtime.serialization"];
		}

		public DataContractSerializerSection DataContractSerializer
		{
			get
			{
				return (DataContractSerializerSection)base.Sections["dataContractSerializer"];
			}
		}

		public NetDataContractSerializerSection NetDataContractSerializer
		{
			get
			{
				return (NetDataContractSerializerSection)base.Sections["netDataContractSerializer"];
			}
		}
	}
}
