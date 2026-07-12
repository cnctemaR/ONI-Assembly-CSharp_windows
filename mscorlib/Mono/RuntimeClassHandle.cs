using System;
using System.Runtime.CompilerServices;

namespace Mono
{
	internal struct RuntimeClassHandle
	{
		internal unsafe RuntimeClassHandle(RuntimeStructs.MonoClass* value)
		{
			this.value = value;
		}

		internal unsafe RuntimeClassHandle(IntPtr ptr)
		{
			this.value = (RuntimeStructs.MonoClass*)(void*)ptr;
		}

		internal unsafe RuntimeStructs.MonoClass* Value
		{
			get
			{
				return this.value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.value == ((RuntimeClassHandle)obj).Value;
		}

		public unsafe override int GetHashCode()
		{
			return ((IntPtr)((void*)this.value)).GetHashCode();
		}

		public bool Equals(RuntimeClassHandle handle)
		{
			return this.value == handle.Value;
		}

		public static bool operator ==(RuntimeClassHandle left, object right)
		{
			if (right != null && right is RuntimeClassHandle)
			{
				RuntimeClassHandle runtimeClassHandle = (RuntimeClassHandle)right;
				return left.Equals(runtimeClassHandle);
			}
			return false;
		}

		public static bool operator !=(RuntimeClassHandle left, object right)
		{
			return !(left == right);
		}

		public static bool operator ==(object left, RuntimeClassHandle right)
		{
			return left != null && left is RuntimeClassHandle && ((RuntimeClassHandle)left).Equals(right);
		}

		public static bool operator !=(object left, RuntimeClassHandle right)
		{
			return !(left == right);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern IntPtr GetTypeFromClass(RuntimeStructs.MonoClass* klass);

		internal RuntimeTypeHandle GetTypeHandle()
		{
			return new RuntimeTypeHandle(RuntimeClassHandle.GetTypeFromClass(this.value));
		}

		private unsafe RuntimeStructs.MonoClass* value;
	}
}
