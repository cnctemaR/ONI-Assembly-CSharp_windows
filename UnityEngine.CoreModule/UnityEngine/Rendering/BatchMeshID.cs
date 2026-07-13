using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeClass("BatchMeshID")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	public struct BatchMeshID : IEquatable<BatchMeshID>
	{
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is BatchMeshID;
			return flag && this.Equals((BatchMeshID)obj);
		}

		public bool Equals(BatchMeshID other)
		{
			return this.value == other.value;
		}

		public int CompareTo(BatchMeshID other)
		{
			return this.value.CompareTo(other.value);
		}

		public static bool operator ==(BatchMeshID a, BatchMeshID b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BatchMeshID a, BatchMeshID b)
		{
			return !a.Equals(b);
		}

		public static readonly BatchMeshID Null = new BatchMeshID
		{
			value = 0U
		};

		public uint value;
	}
}
