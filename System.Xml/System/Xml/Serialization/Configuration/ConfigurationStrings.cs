using System;
using System.Globalization;

namespace System.Xml.Serialization.Configuration
{
	internal static class ConfigurationStrings
	{
		private static string GetSectionPath(string sectionName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", "system.xml.serialization", sectionName);
		}

		internal static string SchemaImporterExtensionsSectionPath
		{
			get
			{
				return ConfigurationStrings.GetSectionPath("schemaImporterExtensions");
			}
		}

		internal static string DateTimeSerializationSectionPath
		{
			get
			{
				return ConfigurationStrings.GetSectionPath("dateTimeSerialization");
			}
		}

		internal static string XmlSerializerSectionPath
		{
			get
			{
				return ConfigurationStrings.GetSectionPath("xmlSerializer");
			}
		}

		internal const string Name = "name";

		internal const string SchemaImporterExtensionsSectionName = "schemaImporterExtensions";

		internal const string DateTimeSerializationSectionName = "dateTimeSerialization";

		internal const string XmlSerializerSectionName = "xmlSerializer";

		internal const string SectionGroupName = "system.xml.serialization";

		internal const string SqlTypesSchemaImporterChar = "SqlTypesSchemaImporterChar";

		internal const string SqlTypesSchemaImporterNChar = "SqlTypesSchemaImporterNChar";

		internal const string SqlTypesSchemaImporterVarChar = "SqlTypesSchemaImporterVarChar";

		internal const string SqlTypesSchemaImporterNVarChar = "SqlTypesSchemaImporterNVarChar";

		internal const string SqlTypesSchemaImporterText = "SqlTypesSchemaImporterText";

		internal const string SqlTypesSchemaImporterNText = "SqlTypesSchemaImporterNText";

		internal const string SqlTypesSchemaImporterVarBinary = "SqlTypesSchemaImporterVarBinary";

		internal const string SqlTypesSchemaImporterBinary = "SqlTypesSchemaImporterBinary";

		internal const string SqlTypesSchemaImporterImage = "SqlTypesSchemaImporterImage";

		internal const string SqlTypesSchemaImporterDecimal = "SqlTypesSchemaImporterDecimal";

		internal const string SqlTypesSchemaImporterNumeric = "SqlTypesSchemaImporterNumeric";

		internal const string SqlTypesSchemaImporterBigInt = "SqlTypesSchemaImporterBigInt";

		internal const string SqlTypesSchemaImporterInt = "SqlTypesSchemaImporterInt";

		internal const string SqlTypesSchemaImporterSmallInt = "SqlTypesSchemaImporterSmallInt";

		internal const string SqlTypesSchemaImporterTinyInt = "SqlTypesSchemaImporterTinyInt";

		internal const string SqlTypesSchemaImporterBit = "SqlTypesSchemaImporterBit";

		internal const string SqlTypesSchemaImporterFloat = "SqlTypesSchemaImporterFloat";

		internal const string SqlTypesSchemaImporterReal = "SqlTypesSchemaImporterReal";

		internal const string SqlTypesSchemaImporterDateTime = "SqlTypesSchemaImporterDateTime";

		internal const string SqlTypesSchemaImporterSmallDateTime = "SqlTypesSchemaImporterSmallDateTime";

		internal const string SqlTypesSchemaImporterMoney = "SqlTypesSchemaImporterMoney";

		internal const string SqlTypesSchemaImporterSmallMoney = "SqlTypesSchemaImporterSmallMoney";

		internal const string SqlTypesSchemaImporterUniqueIdentifier = "SqlTypesSchemaImporterUniqueIdentifier";

		internal const string Type = "type";

		internal const string Mode = "mode";

		internal const string CheckDeserializeAdvances = "checkDeserializeAdvances";

		internal const string TempFilesLocation = "tempFilesLocation";

		internal const string UseLegacySerializerGeneration = "useLegacySerializerGeneration";
	}
}
