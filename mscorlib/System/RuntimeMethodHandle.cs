using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System
{
	[MonoTODO("Serialization needs tests")]
	[ComVisible(true)]
	[Serializable]
	public struct RuntimeMethodHandle : ISerializable
	{
		internal RuntimeMethodHandle(IntPtr v)
		{
			this.value = v;
		}

		private RuntimeMethodHandle(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MonoMethod monoMethod = (MonoMethod)info.GetValue("MethodObj", typeof(MonoMethod));
			this.value = monoMethod.MethodHandle.Value;
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
			info.AddValue("MethodObj", (MonoMethod)MethodBase.GetMethodFromHandle(this), typeof(MonoMethod));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetFunctionPointer(IntPtr m);

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public IntPtr GetFunctionPointer()
		{
			return RuntimeMethodHandle.GetFunctionPointer(this.value);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.value == ((RuntimeMethodHandle)obj).Value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals(RuntimeMethodHandle handle)
		{
			return this.value == handle.Value;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public static bool operator ==(RuntimeMethodHandle left, RuntimeMethodHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeMethodHandle left, RuntimeMethodHandle right)
		{
			return !left.Equals(right);
		}

		private IntPtr value;
	}
}
