using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	[ComVisible(true)]
	[Serializable]
	public class KeyNotFoundException : SystemException, ISerializable
	{
		public KeyNotFoundException()
			: base("The given key was not present in the dictionary.")
		{
		}

		public KeyNotFoundException(string message)
			: base(message)
		{
		}

		public KeyNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected KeyNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
