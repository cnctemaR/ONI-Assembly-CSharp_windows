using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class DirectoryNotFoundException : IOException
	{
		public DirectoryNotFoundException()
			: base(Environment.GetResourceString("Attempted to access a path that is not on the disk."))
		{
			base.SetErrorCode(-2147024893);
		}

		public DirectoryNotFoundException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147024893);
		}

		public DirectoryNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2147024893);
		}

		protected DirectoryNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
