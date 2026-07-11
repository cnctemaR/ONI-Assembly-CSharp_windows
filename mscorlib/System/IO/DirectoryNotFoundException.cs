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
			: base("Directory not found")
		{
		}

		public DirectoryNotFoundException(string message)
			: base(message)
		{
		}

		public DirectoryNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DirectoryNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
