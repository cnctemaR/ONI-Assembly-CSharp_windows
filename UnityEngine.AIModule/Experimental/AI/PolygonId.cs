using System;

namespace UnityEngine.Experimental.AI
{
	[Obsolete("The experimental PolygonId struct has been deprecated without replacement.")]
	public struct PolygonId : IEquatable<PolygonId>
	{
		public bool IsNull()
		{
			return this.polyRef == 0UL;
		}

		public static bool operator ==(PolygonId x, PolygonId y)
		{
			return x.polyRef == y.polyRef;
		}

		public static bool operator !=(PolygonId x, PolygonId y)
		{
			return x.polyRef != y.polyRef;
		}

		public override int GetHashCode()
		{
			return this.polyRef.GetHashCode();
		}

		public bool Equals(PolygonId rhs)
		{
			return rhs == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null || !(obj is PolygonId);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				PolygonId polygonId = (PolygonId)obj;
				flag2 = polygonId == this;
			}
			return flag2;
		}

		internal ulong polyRef;
	}
}
