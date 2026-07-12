using System;
using System.Runtime.Serialization;

namespace System.Reflection
{
	public sealed class Missing : ISerializable
	{
		private Missing()
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}

		public static readonly Missing Value = new Missing();
	}
}
