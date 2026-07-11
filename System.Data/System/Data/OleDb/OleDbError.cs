using System;

namespace System.Data.OleDb
{
	[Serializable]
	public sealed class OleDbError
	{
		internal OleDbError()
		{
		}

		public string Message
		{
			get
			{
				throw null;
			}
		}

		public int NativeError
		{
			get
			{
				throw null;
			}
		}

		public string Source
		{
			get
			{
				throw null;
			}
		}

		public string SQLState
		{
			get
			{
				throw null;
			}
		}

		[MonoTODO]
		public override string ToString()
		{
			throw null;
		}
	}
}
