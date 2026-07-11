using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	public sealed class InvalidUdtException : SystemException
	{
		internal InvalidUdtException()
		{
		}

		[MonoTODO]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
