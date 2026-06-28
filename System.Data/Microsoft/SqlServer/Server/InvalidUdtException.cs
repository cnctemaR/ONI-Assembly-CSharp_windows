using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	public sealed class InvalidUdtException : SystemException
	{
		[MonoTODO]
		internal InvalidUdtException()
		{
		}

		[MonoTODO]
		internal InvalidUdtException(string message)
		{
		}

		[MonoTODO]
		internal InvalidUdtException(string message, Exception innerException)
		{
		}

		[MonoTODO]
		internal InvalidUdtException(Type t, string reason)
		{
		}

		[MonoTODO]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
