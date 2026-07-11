using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeCharSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeCharSchemaImporterExtension()
			: base("char", "System.Data.SqlTypes.SqlString", false)
		{
		}
	}
}
