using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class DriveNotFoundException : IOException
	{
		public DriveNotFoundException()
			: base("Attempted to access a drive that is not available.")
		{
			base.HResult = -2147024893;
		}

		public DriveNotFoundException(string message)
			: base(message)
		{
			base.HResult = -2147024893;
		}

		public DriveNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024893;
		}

		protected DriveNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int ErrorCode = -2147024893;
	}
}
