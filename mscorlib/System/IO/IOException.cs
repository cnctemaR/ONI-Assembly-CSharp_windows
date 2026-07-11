using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class IOException : SystemException
	{
		public IOException()
			: base("I/O Error")
		{
		}

		public IOException(string message)
			: base(message)
		{
		}

		public IOException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IOException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public IOException(string message, int hresult)
			: base(message)
		{
			base.HResult = hresult;
		}
	}
}
