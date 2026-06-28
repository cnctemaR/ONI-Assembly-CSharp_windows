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
				throw new NotImplementedException();
			}
		}

		public OleDbErrorCollection Errors
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Message
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Source
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public override string ToString()
		{
			throw new NotImplementedException();
		}
	}
}
