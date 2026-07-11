using System;

namespace Mono
{
	internal struct RuntimeEventHandle
	{
		internal RuntimeEventHandle(IntPtr v)
		{
			this.value = v;
		}

		public IntPtr Value
		{
			get
			{
				return this.value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.value == ((RuntimeEventHandle)obj).Value;
		}

		public bool Equals(RuntimeEventHandle handle)
		{
			return this.value == handle.Value;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public static bool operator ==(RuntimeEventHandle left, RuntimeEventHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeEventHandle left, RuntimeEventHandle right)
		{
			return !left.Equals(right);
		}

		private IntPtr value;
	}
}
