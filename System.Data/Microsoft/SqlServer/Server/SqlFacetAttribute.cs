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
				return this.m_IsFixedLength;
			}
			set
			{
				this.m_IsFixedLength = value;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.m_MaxSize;
			}
			set
			{
				this.m_MaxSize = value;
			}
		}

		public int Precision
		{
			get
			{
				return this.m_Precision;
			}
			set
			{
				this.m_Precision = value;
			}
		}

		public int Scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		public bool IsNullable
		{
			get
			{
				return this.m_IsNullable;
			}
			set
			{
				this.m_IsNullable = value;
			}
		}

		private bool m_IsFixedLength;

		private int m_MaxSize;

		private int m_Scale;

		private int m_Precision;

		private bool m_IsNullable;
	}
}
