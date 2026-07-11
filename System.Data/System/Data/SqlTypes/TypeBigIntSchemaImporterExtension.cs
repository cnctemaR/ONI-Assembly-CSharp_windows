using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeBigIntSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeBigIntSchemaImporterExtension()
			: base("bigint", "System.Data.SqlTypes.SqlInt64")
		{
		}
	}
}
