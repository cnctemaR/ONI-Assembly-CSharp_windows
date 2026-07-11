using System;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class UriFormatException : FormatException, ISerializable
	{
		public UriFormatException()
			: base(global::Locale.GetText("Invalid URI format"))
		{
		}

		public UriFormatException(string message)
			: base(message)
		{
		}

		protected UriFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
