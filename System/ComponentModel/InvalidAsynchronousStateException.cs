using System;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
	[Serializable]
	public class InvalidAsynchronousStateException : ArgumentException
	{
		public InvalidAsynchronousStateException()
			: this("Invalid asynchrinous state is occured")
		{
		}

		public InvalidAsynchronousStateException(string message)
			: this(message, null)
		{
		}

		public InvalidAsynchronousStateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InvalidAsynchronousStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
