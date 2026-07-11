using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	[TypeConverter("System.Data.Odbc.OdbcParameter+OdbcParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class OdbcParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
	{
		public OdbcParameter()
		{
		}

		public OdbcParameter(string name, OdbcType type)
		{
		}

		public OdbcParameter(string name, OdbcType type, int size)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public OdbcParameter(string parameterName, OdbcType odbcType, int size, ParameterDirection parameterDirection, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
		{
		}

		public OdbcParameter(string name, OdbcType type, int size, string sourcecolumn)
		{
		}

		public OdbcParameter(string name, object value)
		{
		}

		public override DbType DbType
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public override ParameterDirection Direction
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override bool IsNullable
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(OdbcType.NChar)]
		[RefreshProperties(RefreshProperties.All)]
		[DbProviderSpecificTypeProperty(true)]
		public OdbcType OdbcType
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override string ParameterName
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(0)]
		public byte Precision
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(0)]
		public byte Scale
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override int Size
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override string SourceColumn
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override DataRowVersion SourceVersion
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StringConverter))]
		public override object Value
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override void ResetDbType()
		{
		}

		public void ResetOdbcType()
		{
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
