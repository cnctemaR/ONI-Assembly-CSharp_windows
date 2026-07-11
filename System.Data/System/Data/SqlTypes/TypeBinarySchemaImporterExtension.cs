using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeBinarySchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeBinarySchemaImporterExtension()
			: base("binary", "System.Data.SqlTypes.SqlBinary", false)
		{
		}
	}
}
