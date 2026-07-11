using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public sealed class SqlUserDefinedAggregateAttribute : Attribute
	{
		public SqlUserDefinedAggregateAttribute(Format format)
		{
		}

		public Format Format
		{
			get
			{
				throw null;
			}
		}

		public bool IsInvariantToDuplicates
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool IsInvariantToNulls
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool IsInvariantToOrder
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool IsNullIfEmpty
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int MaxByteSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public const int MaxByteSizeValue = 8000;
	}
}
