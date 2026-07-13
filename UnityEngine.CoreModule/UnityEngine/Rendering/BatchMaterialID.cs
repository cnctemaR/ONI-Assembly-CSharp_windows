using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeClass("BatchMaterialID")]
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct BatchMaterialID : IEquatable<BatchMaterialID>
	{
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is BatchMaterialID;
			return flag && this.Equals((BatchMaterialID)obj);
		}

		public bool Equals(BatchMaterialID other)
		{
			return this.value == other.value;
		}

		public int CompareTo(BatchMaterialID other)
		{
			return this.value.CompareTo(other.value);
		}

		public static bool operator ==(BatchMaterialID a, BatchMaterialID b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BatchMaterialID a, BatchMaterialID b)
		{
			return !a.Equals(b);
		}

		public static readonly BatchMaterialID Null = new BatchMaterialID
		{
			value = 0U
		};

		public uint value;
	}
}
