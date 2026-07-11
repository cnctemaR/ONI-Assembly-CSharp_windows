using System;

namespace System.Data.OleDb
{
	public sealed class OleDbInfoMessageEventArgs : EventArgs
	{
		internal OleDbInfoMessageEventArgs()
		{
		}

		public int ErrorCode
		{
			[MonoTODO]
			get
			{
				throw null;
			}
		}

		public OleDbErrorCollection Errors
		{
			[MonoTODO]
			get
			{
				throw null;
			}
		}

		public string Message
		{
			[MonoTODO]
			get
			{
				throw null;
			}
		}

		public string Source
		{
			[MonoTODO]
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
