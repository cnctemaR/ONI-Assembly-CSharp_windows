using System;

namespace Mono
{
	internal struct RuntimePropertyHandle
	{
		internal RuntimePropertyHandle(IntPtr v)
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
			return obj != null && !(base.GetType() != obj.GetType()) && this.value == ((RuntimePropertyHandle)obj).Value;
		}

		public bool Equals(RuntimePropertyHandle handle)
		{
			return this.value == handle.Value;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public static bool operator ==(RuntimePropertyHandle left, RuntimePropertyHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimePropertyHandle left, RuntimePropertyHandle right)
		{
			return !left.Equals(right);
		}

		private IntPtr value;
	}
}
