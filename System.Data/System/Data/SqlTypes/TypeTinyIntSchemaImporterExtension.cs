using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeTinyIntSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeTinyIntSchemaImporterExtension()
			: base("tinyint", "System.Data.SqlTypes.SqlByte")
		{
		}
	}
}
