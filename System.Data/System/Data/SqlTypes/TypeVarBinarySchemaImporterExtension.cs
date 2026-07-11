using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeVarBinarySchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeVarBinarySchemaImporterExtension()
			: base("varbinary", "System.Data.SqlTypes.SqlBinary", false)
		{
		}
	}
}
