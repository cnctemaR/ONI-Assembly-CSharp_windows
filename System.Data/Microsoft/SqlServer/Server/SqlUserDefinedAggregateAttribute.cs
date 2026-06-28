using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public sealed class SqlUserDefinedAggregateAttribute : Attribute
	{
		public SqlUserDefinedAggregateAttribute(Format f)
		{
			this.format = f;
			this.IsInvariantToDuplicates = false;
			this.IsInvariantToNulls = false;
			this.IsInvariantToOrder = false;
			this.IsNullIfEmpty = false;
			this.MaxByteSize = 8000;
		}

		public Format Format
		{
			get
			{
				return this.format;
			}
		}

		public bool IsInvariantToDuplicates
		{
			get
			{
				return this.isInvariantToDuplicates;
			}
			set
			{
				this.isInvariantToDuplicates = value;
			}
		}

		public bool IsInvariantToNulls
		{
			get
			{
				return this.isInvariantToNulls;
			}
			set
			{
				this.isInvariantToNulls = value;
			}
		}

		public bool IsInvariantToOrder
		{
			get
			{
				return this.isInvariantToOrder;
			}
			set
			{
				this.isInvariantToOrder = value;
			}
		}

		public bool IsNullIfEmpty
		{
			get
			{
				return this.isNullIfEmpty;
			}
			set
			{
				this.isNullIfEmpty = value;
			}
		}

		public int MaxByteSize
		{
			get
			{
				return this.maxByteSize;
			}
			set
			{
				this.maxByteSize = value;
			}
		}

		public const int MaxByteSizeValue = 8000;

		private Format format;

		private bool isInvariantToDuplicates;

		private bool isInvariantToNulls;

		private bool isInvariantToOrder;

		private bool isNullIfEmpty;

		private int maxByteSize;
	}
}
