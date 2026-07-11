using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	public class SqlFacetAttribute : Attribute
	{
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

		public bool IsNullable
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int MaxSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int Precision
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int Scale
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
