using System;
using System.Runtime.Serialization;
using Unity;

namespace System.Reflection
{
	[CLSCompliant(false)]
	public sealed class Pointer : ISerializable
	{
		private unsafe Pointer(void* ptr, Type ptrType)
		{
			this._ptr = ptr;
			this._ptrType = ptrType;
		}

		public unsafe static object Box(void* ptr, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsPointer)
			{
				throw new ArgumentException("Type must be a Pointer.", "ptr");
			}
			if (!type.IsRuntimeImplemented())
			{
				throw new ArgumentException("Type must be a type provided by the runtime.", "ptr");
			}
			return new Pointer(ptr, type);
		}

		public unsafe static void* Unbox(object ptr)
		{
			if (!(ptr is Pointer))
			{
				throw new ArgumentException("Type must be a Pointer.", "ptr");
			}
			return ((Pointer)ptr)._ptr;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}

		internal Type GetPointerType()
		{
			return this._ptrType;
		}

		internal IntPtr GetPointerValue()
		{
			return (IntPtr)this._ptr;
		}

		internal Pointer()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private unsafe readonly void* _ptr;

		private readonly Type _ptrType;
	}
}
