using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
	{
		public override DbType DbType
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override ParameterDirection Direction
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override bool IsNullable
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public int Offset
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public OleDbType OleDbType
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override string ParameterName
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new byte Precision
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new byte Scale
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override int Size
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override string SourceColumn
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override DataRowVersion SourceVersion
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override object Value
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public OleDbParameter()
		{
		}

		public OleDbParameter(string name, OleDbType dataType)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter(string name, OleDbType dataType, int size)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter(string parameterName, OleDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter(string parameterName, OleDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter(string name, OleDbType dataType, int size, string srcColumn)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter(string name, object value)
		{
			throw ADP.OleDb();
		}

		public override void ResetDbType()
		{
			throw ADP.OleDb();
		}

		public override string ToString()
		{
			throw ADP.OleDb();
		}

		object ICloneable.Clone()
		{
			throw ADP.OleDb();
		}

		public void ResetOleDbType()
		{
			throw ADP.OleDb();
		}
	}
}
