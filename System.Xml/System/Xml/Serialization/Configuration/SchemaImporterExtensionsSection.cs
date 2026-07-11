using System;
using System.Configuration;
using System.Xml.Serialization.Advanced;

namespace System.Xml.Serialization.Configuration
{
	public sealed class SchemaImporterExtensionsSection : ConfigurationSection
	{
		public SchemaImporterExtensionsSection()
		{
			this.properties.Add(this.schemaImporterExtensions);
		}

		private static string GetSqlTypeSchemaImporter(string typeName)
		{
			return "System.Data.SqlTypes." + typeName + ", System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		}

		protected override void InitializeDefault()
		{
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterChar", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeCharSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterNChar", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeNCharSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterVarChar", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeVarCharSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterNVarChar", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeNVarCharSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterText", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeTextSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterNText", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeNTextSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterVarBinary", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeVarBinarySchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterBinary", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeBinarySchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterImage", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeVarImageSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterDecimal", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeDecimalSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterNumeric", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeNumericSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterBigInt", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeBigIntSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterInt", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeIntSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterSmallInt", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeSmallIntSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterTinyInt", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeTinyIntSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterBit", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeBitSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterFloat", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeFloatSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterReal", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeRealSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterDateTime", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeDateTimeSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterSmallDateTime", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeSmallDateTimeSchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterMoney", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeMoneySchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterSmallMoney", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeSmallMoneySchemaImporterExtension")));
			this.SchemaImporterExtensions.Add(new SchemaImporterExtensionElement("SqlTypesSchemaImporterUniqueIdentifier", SchemaImporterExtensionsSection.GetSqlTypeSchemaImporter("TypeUniqueIdentifierSchemaImporterExtension")));
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return this.properties;
			}
		}

		[ConfigurationProperty("", IsDefaultCollection = true)]
		public SchemaImporterExtensionElementCollection SchemaImporterExtensions
		{
			get
			{
				return (SchemaImporterExtensionElementCollection)base[this.schemaImporterExtensions];
			}
		}

		internal SchemaImporterExtensionCollection SchemaImporterExtensionsInternal
		{
			get
			{
				SchemaImporterExtensionCollection schemaImporterExtensionCollection = new SchemaImporterExtensionCollection();
				foreach (object obj in this.SchemaImporterExtensions)
				{
					SchemaImporterExtensionElement schemaImporterExtensionElement = (SchemaImporterExtensionElement)obj;
					schemaImporterExtensionCollection.Add(schemaImporterExtensionElement.Name, schemaImporterExtensionElement.Type);
				}
				return schemaImporterExtensionCollection;
			}
		}

		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private readonly ConfigurationProperty schemaImporterExtensions = new ConfigurationProperty(null, typeof(SchemaImporterExtensionElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
