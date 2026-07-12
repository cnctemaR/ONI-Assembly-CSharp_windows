using System;

namespace System.Runtime.Serialization.Configuration
{
	internal static class ConfigurationStrings
	{
		private static string GetSectionPath(string sectionName)
		{
			return "system.runtime.serialization" + "/" + sectionName;
		}

		internal static string DataContractSerializerSectionPath
		{
			get
			{
				return ConfigurationStrings.GetSectionPath("dataContractSerializer");
			}
		}

		internal static string NetDataContractSerializerSectionPath
		{
			get
			{
				return ConfigurationStrings.GetSectionPath("netDataContractSerializer");
			}
		}

		internal const string SectionGroupName = "system.runtime.serialization";

		internal const string DefaultCollectionName = "";

		internal const string DeclaredTypes = "declaredTypes";

		internal const string Index = "index";

		internal const string Parameter = "parameter";

		internal const string Type = "type";

		internal const string EnableUnsafeTypeForwarding = "enableUnsafeTypeForwarding";

		internal const string DataContractSerializerSectionName = "dataContractSerializer";

		internal const string NetDataContractSerializerSectionName = "netDataContractSerializer";
	}
}
