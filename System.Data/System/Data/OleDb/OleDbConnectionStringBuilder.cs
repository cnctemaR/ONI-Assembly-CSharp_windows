using System;
using System.Collections;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbConnectionStringBuilder : DbConnectionStringBuilder
	{
		public OleDbConnectionStringBuilder()
		{
			throw ADP.OleDb();
		}

		public OleDbConnectionStringBuilder(string connectionString)
		{
			throw ADP.OleDb();
		}

		public string DataSource
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public string FileName
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public object Item
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override ICollection Keys
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public int OleDbServices
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public bool PersistSecurityInfo
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public string Provider
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override void Clear()
		{
			throw ADP.OleDb();
		}

		public override bool ContainsKey(string keyword)
		{
			throw ADP.OleDb();
		}

		protected override void GetProperties(Hashtable propertyDescriptors)
		{
			throw ADP.OleDb();
		}

		public override bool Remove(string keyword)
		{
			throw ADP.OleDb();
		}

		public bool TryGetValue(string keyword, object value)
		{
			throw ADP.OleDb();
		}
	}
}
