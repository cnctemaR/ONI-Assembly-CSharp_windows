using System;

namespace Epic.OnlineServices
{
	public class Handle : IEquatable<Handle>
	{
		public IntPtr InnerHandle { get; private set; }

		public Handle(IntPtr innerHandle)
		{
			this.InnerHandle = innerHandle;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Handle);
		}

		public override int GetHashCode()
		{
			return (int)(65536L + this.InnerHandle.ToInt64());
		}

		public bool Equals(Handle other)
		{
			return other != null && (this == other || (!(base.GetType() != other.GetType()) && this.InnerHandle == other.InnerHandle));
		}

		public static bool operator ==(Handle lhs, Handle rhs)
		{
			if (lhs == null)
			{
				return rhs == null;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Handle lhs, Handle rhs)
		{
			return !(lhs == rhs);
		}
	}
}
