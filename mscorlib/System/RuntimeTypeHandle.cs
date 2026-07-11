using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[MonoTODO("Serialization needs tests")]
	[Serializable]
	public struct RuntimeTypeHandle : ISerializable
	{
		internal RuntimeTypeHandle(IntPtr val)
		{
			this.value = val;
		}

		private RuntimeTypeHandle(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MonoType monoType = (MonoType)info.GetValue("TypeObj", typeof(MonoType));
			this.value = monoType.TypeHandle.Value;
			if (this.value == IntPtr.Zero)
			{
				throw new SerializationException(Locale.GetText("Insufficient state."));
			}
		}

		public IntPtr Value
		{
			get
			{
				return this.value;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (this.value == IntPtr.Zero)
			{
				throw new SerializationException("Object fields may not be properly initialized");
			}
			info.AddValue("TypeObj", Type.GetTypeHandle(this), typeof(MonoType));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.value == ((RuntimeTypeHandle)obj).Value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals(RuntimeTypeHandle handle)
		{
			return this.value == handle.Value;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public ModuleHandle GetModuleHandle()
		{
			if (this.value == IntPtr.Zero)
			{
				throw new InvalidOperationException("Object fields may not be properly initialized");
			}
			return Type.GetTypeFromHandle(this).Module.ModuleHandle;
		}

		public static bool operator ==(RuntimeTypeHandle left, object right)
		{
			return right != null && right is RuntimeTypeHandle && left.Equals((RuntimeTypeHandle)right);
		}

		public static bool operator !=(RuntimeTypeHandle left, object right)
		{
			return right == null || !(right is RuntimeTypeHandle) || !left.Equals((RuntimeTypeHandle)right);
		}

		public static bool operator ==(object left, RuntimeTypeHandle right)
		{
			return left != null && left is RuntimeTypeHandle && ((RuntimeTypeHandle)left).Equals(right);
		}

		public static bool operator !=(object left, RuntimeTypeHandle right)
		{
			return left == null || !(left is RuntimeTypeHandle) || !((RuntimeTypeHandle)left).Equals(right);
		}

		private IntPtr value;
	}
}
