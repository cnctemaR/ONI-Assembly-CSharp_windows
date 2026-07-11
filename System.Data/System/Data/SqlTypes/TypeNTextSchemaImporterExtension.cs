using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeNTextSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeNTextSchemaImporterExtension()
			: base("ntext", "System.Data.SqlTypes.SqlString", false)
		{
		}
	}
}
