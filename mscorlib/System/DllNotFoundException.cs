using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class DllNotFoundException : TypeLoadException
	{
		public DllNotFoundException()
			: base(Environment.GetResourceString("Dll was not found."))
		{
			base.SetErrorCode(-2146233052);
		}

		public DllNotFoundException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233052);
		}

		public DllNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233052);
		}

		protected DllNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
