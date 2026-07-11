using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeFloatSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeFloatSchemaImporterExtension()
			: base("float", "System.Data.SqlTypes.SqlDouble")
		{
		}
	}
}
