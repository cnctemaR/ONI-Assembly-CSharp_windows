using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class COMException : ExternalException
	{
		public COMException()
		{
		}

		public COMException(string message)
			: base(message)
		{
		}

		public COMException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public COMException(string message, int errorCode)
			: base(message, errorCode)
		{
		}

		protected COMException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} (0x{1:x}): {2} {3}{4}{5}", new object[]
			{
				this.GetType(),
				base.HResult,
				this.Message,
				(this.InnerException != null) ? this.InnerException.ToString() : string.Empty,
				Environment.NewLine,
				(this.StackTrace == null) ? string.Empty : this.StackTrace
			});
		}
	}
}
