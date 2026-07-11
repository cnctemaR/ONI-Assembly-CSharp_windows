using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbError
	{
		internal OleDbError()
		{
		}

		public string Message
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public int NativeError
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public string Source
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public string SQLState
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override string ToString()
		{
			throw ADP.OleDb();
		}
	}
}
