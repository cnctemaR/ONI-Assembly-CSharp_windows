using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeBitSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeBitSchemaImporterExtension()
			: base("bit", "System.Data.SqlTypes.SqlBoolean")
		{
		}
	}
}
