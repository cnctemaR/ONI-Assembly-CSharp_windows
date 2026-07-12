using System;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Data.SqlClient
{
	public class SqlEnclaveSession
	{
		public SqlEnclaveSession(byte[] sessionKey, long sessionId)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public long SessionId
		{
			[CompilerGenerated]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		public byte[] GetSessionKey()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
