using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Missing : ISerializable
	{
		internal Missing()
		{
		}

		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		public static readonly Missing Value = new Missing();
	}
}
