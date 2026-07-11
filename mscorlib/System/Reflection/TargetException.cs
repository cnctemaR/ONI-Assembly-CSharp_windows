using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public class TargetException : ApplicationException
	{
		public TargetException()
			: base(Locale.GetText("Unable to invoke an invalid target."))
		{
		}

		public TargetException(string message)
			: base(message)
		{
		}

		public TargetException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected TargetException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
