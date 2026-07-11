using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeDecimalSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeDecimalSchemaImporterExtension()
			: base("decimal", "System.Data.SqlTypes.SqlDecimal", false)
		{
		}
	}
}
