using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeRealSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeRealSchemaImporterExtension()
			: base("real", "System.Data.SqlTypes.SqlSingle")
		{
		}
	}
}
