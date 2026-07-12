using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeClass("BatchID")]
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	public struct BatchID : IEquatable<BatchID>
	{
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is BatchID;
			return flag && this.Equals((BatchID)obj);
		}

		public bool Equals(BatchID other)
		{
			return this.value == other.value;
		}

		public int CompareTo(BatchID other)
		{
			return this.value.CompareTo(other.value);
		}

		public static bool operator ==(BatchID a, BatchID b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BatchID a, BatchID b)
		{
			return !a.Equals(b);
		}

		public static readonly BatchID Null = new BatchID
		{
			value = 0U
		};

		public uint value;
	}
}
