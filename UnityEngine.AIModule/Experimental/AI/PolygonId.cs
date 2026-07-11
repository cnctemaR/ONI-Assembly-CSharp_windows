using System;

namespace UnityEngine.Experimental.AI
{
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
			bool flag;
			if (obj == null || !(obj is PolygonId))
			{
				flag = false;
			}
			else
			{
				PolygonId polygonId = (PolygonId)obj;
				flag = polygonId == this;
			}
			return flag;
		}

		internal ulong polyRef;
	}
}
