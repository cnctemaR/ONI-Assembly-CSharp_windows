using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeSmallIntSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeSmallIntSchemaImporterExtension()
			: base("smallint", "System.Data.SqlTypes.SqlInt16")
		{
		}
	}
}
