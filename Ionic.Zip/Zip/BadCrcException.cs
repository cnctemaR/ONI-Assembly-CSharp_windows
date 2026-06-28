using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Ionic.Zip
{
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00009")]
	[Serializable]
	public class BadCrcException : ZipException
	{
		public BadCrcException()
		{
		}

		public BadCrcException(string message)
			: base(message)
		{
		}

		protected BadCrcException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
