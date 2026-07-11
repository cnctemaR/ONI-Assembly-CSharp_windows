using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class VersionNotFoundException : DataException
	{
		public VersionNotFoundException()
		{
		}

		protected VersionNotFoundException(SerializationInfo info, StreamingContext context)
		{
		}

		public VersionNotFoundException(string s)
		{
		}

		public VersionNotFoundException(string message, Exception innerException)
		{
		}
	}
}
