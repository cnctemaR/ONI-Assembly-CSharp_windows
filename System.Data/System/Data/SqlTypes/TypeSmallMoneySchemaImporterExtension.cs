using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeSmallMoneySchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeSmallMoneySchemaImporterExtension()
			: base("smallmoney", "System.Data.SqlTypes.SqlMoney")
		{
		}
	}
}
