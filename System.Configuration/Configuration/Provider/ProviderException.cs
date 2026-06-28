using System;
using System.Runtime.Serialization;

namespace System.Configuration.Provider
{
	[Serializable]
	public class ProviderException : Exception
	{
		public ProviderException()
		{
		}

		protected ProviderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ProviderException(string message)
			: base(message)
		{
		}

		public ProviderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
