using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	public class SqlFacetAttribute : Attribute
	{
		public SqlFacetAttribute()
		{
			this.isFixedLength = false;
			this.isNullable = false;
			this.maxSize = 0;
			this.precision = 0;
			this.scale = 0;
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

		public bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.maxSize;
			}
			set
			{
				this.maxSize = value;
			}
		}

		public int Precision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
			}
		}

		public int Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		private bool isFixedLength;

		private bool isNullable;

		private int maxSize;

		private int precision;

		private int scale;
	}
}
