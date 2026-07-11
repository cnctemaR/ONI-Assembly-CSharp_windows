using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;

namespace System.Data.SqlClient
{
	[TypeConverter("System.Data.SqlClient.SqlParameter+SqlParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class SqlParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
	{
		public SqlParameter()
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value, string xmlSchemaCollectionDatabase, string xmlSchemaCollectionOwningSchema, string xmlSchemaCollectionName)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, string sourceColumn)
		{
		}

		public SqlParameter(string parameterName, object value)
		{
		}

		[Browsable(false)]
		public SqlCompareOptions CompareInfo
		{
			get
			{
				throw null;
			}
			set
			{
			}
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

		[Browsable(false)]
		public int LocaleId
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Offset
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
		[DbProviderSpecificTypeProperty(true)]
		public SqlDbType SqlDbType
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SqlValue
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public string UdtTypeName
		{
			[CompilerGenerated]
			get
			{
				throw null;
			}
			[CompilerGenerated]
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

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string XmlSchemaCollectionOwningSchema
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

		public void ResetSqlDbType()
		{
		}

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
