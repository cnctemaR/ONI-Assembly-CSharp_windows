using System;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[MonoTODO("Serialization needs tests")]
	[Serializable]
	public struct RuntimeFieldHandle : ISerializable
	{
		internal RuntimeFieldHandle(IntPtr v)
		{
			this.value = v;
		}

		private RuntimeFieldHandle(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MonoField monoField = (MonoField)info.GetValue("FieldObj", typeof(MonoField));
			this.value = monoField.FieldHandle.Value;
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
			info.AddValue("FieldObj", (MonoField)FieldInfo.GetFieldFromHandle(this), typeof(MonoField));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.value == ((RuntimeFieldHandle)obj).Value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals(RuntimeFieldHandle handle)
		{
			return this.value == handle.Value;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public static bool operator ==(RuntimeFieldHandle left, RuntimeFieldHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeFieldHandle left, RuntimeFieldHandle right)
		{
			return !left.Equals(right);
		}

		private IntPtr value;
	}
}
