using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public class CustomAttributeFormatException : FormatException
	{
		public CustomAttributeFormatException()
			: base(Locale.GetText("The Binary format of the custom attribute is invalid."))
		{
		}

		public CustomAttributeFormatException(string message)
			: base(message)
		{
		}

		public CustomAttributeFormatException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected CustomAttributeFormatException(SerializationInfo info, StreamingContext context)
		{
		}
	}
}
