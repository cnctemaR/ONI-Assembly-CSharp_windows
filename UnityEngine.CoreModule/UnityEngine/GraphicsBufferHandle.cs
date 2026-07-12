using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
	[NativeClass("GfxBufferID")]
	public readonly struct GraphicsBufferHandle : IEquatable<GraphicsBufferHandle>
	{
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is GraphicsBufferHandle;
			return flag && this.Equals((GraphicsBufferHandle)obj);
		}

		public bool Equals(GraphicsBufferHandle other)
		{
			return this.value == other.value;
		}

		public int CompareTo(GraphicsBufferHandle other)
		{
			return this.value.CompareTo(other.value);
		}

		public static bool operator ==(GraphicsBufferHandle a, GraphicsBufferHandle b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(GraphicsBufferHandle a, GraphicsBufferHandle b)
		{
			return !a.Equals(b);
		}

		public readonly uint value;
	}
}
