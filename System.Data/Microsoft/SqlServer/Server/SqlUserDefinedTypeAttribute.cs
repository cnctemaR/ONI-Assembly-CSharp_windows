using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class SqlUserDefinedTypeAttribute : Attribute
	{
		public SqlUserDefinedTypeAttribute(Format f)
		{
			this.format = f;
			this.IsByteOrdered = false;
			this.IsFixedLength = false;
			this.MaxByteSize = 8000;
		}

		public Format Format
		{
			get
			{
				return this.format;
			}
		}

		public bool IsByteOrdered
		{
			get
			{
				return this.isByteOrdered;
			}
			set
			{
				this.isByteOrdered = value;
			}
		}

		public bool IsFixedLength
		{
			get
			{
				return this.isFixedLength;
			}
			set
			{
				this.isFixedLength = value;
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

		private const int MaxByteSizeValue = 8000;

		private Format format;

		private bool isByteOrdered;

		private bool isFixedLength;

		private int maxByteSize;
	}
}
