using System;

namespace UnityEngine.Rendering
{
	public struct BatchPackedCullingViewID : IEquatable<BatchPackedCullingViewID>
	{
		public override int GetHashCode()
		{
			return this.handle.GetHashCode();
		}

		public bool Equals(BatchPackedCullingViewID other)
		{
			return this.handle == other.handle;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is BatchPackedCullingViewID);
			return !flag && this.Equals((BatchPackedCullingViewID)obj);
		}

		public static bool operator ==(BatchPackedCullingViewID lhs, BatchPackedCullingViewID rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BatchPackedCullingViewID lhs, BatchPackedCullingViewID rhs)
		{
			return !lhs.Equals(rhs);
		}

		public BatchPackedCullingViewID(int instanceID, int sliceIndex)
		{
			this.handle = (ulong)instanceID | (ulong)((ulong)((long)sliceIndex) << 32);
		}

		public int GetInstanceID()
		{
			return (int)(this.handle & (ulong)(-1));
		}

		public int GetSliceIndex()
		{
			return (int)(this.handle >> 32);
		}

		internal ulong handle;
	}
}
