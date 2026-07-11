using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	[DefaultProperty("Driver")]
	[TypeConverter("System.Data.Odbc.OdbcConnectionStringBuilder+OdbcConnectionStringBuilderConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class OdbcConnectionStringBuilder : DbConnectionStringBuilder
	{
		public OdbcConnectionStringBuilder()
		{
		}

		public OdbcConnectionStringBuilder(string connectionString)
		{
		}

		[DisplayName("Driver")]
		[RefreshProperties(RefreshProperties.All)]
		public string Driver
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Dsn")]
		[RefreshProperties(RefreshProperties.All)]
		public string Dsn
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override object this[string keyword]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override ICollection Keys
		{
			get
			{
				throw null;
			}
		}

		public override void Clear()
		{
		}

		public override bool ContainsKey(string keyword)
		{
			throw null;
		}

		public override bool Remove(string keyword)
		{
			throw null;
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			value = null;
			throw null;
		}
	}
}
