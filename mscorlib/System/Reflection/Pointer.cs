using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[CLSCompliant(false)]
	[Serializable]
	public sealed class Pointer : ISerializable
	{
		private Pointer()
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException("Pointer deserializatioon not supported.");
		}

		public unsafe static object Box(void* ptr, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsPointer)
			{
				throw new ArgumentException("type");
			}
			return new Pointer
			{
				data = ptr,
				type = type
			};
		}

		public unsafe static void* Unbox(object ptr)
		{
			Pointer pointer = ptr as Pointer;
			if (pointer == null)
			{
				throw new ArgumentException("ptr");
			}
			return pointer.data;
		}

		private unsafe void* data;

		private Type type;
	}
}
