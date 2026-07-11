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
			: base(Environment.GetResourceString("The given key was not present in the dictionary."))
		{
			base.SetErrorCode(-2146232969);
		}

		public KeyNotFoundException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146232969);
		}

		public KeyNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146232969);
		}

		protected KeyNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
