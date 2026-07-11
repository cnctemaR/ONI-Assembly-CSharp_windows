using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class SqlUserDefinedTypeAttribute : Attribute
	{
		public SqlUserDefinedTypeAttribute(Format format)
		{
		}

		public Format Format
		{
			get
			{
				throw null;
			}
		}

		public bool IsByteOrdered
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public bool IsFixedLength
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
	}
}
