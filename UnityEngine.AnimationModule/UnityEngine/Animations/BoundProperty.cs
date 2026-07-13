using System;
using UnityEngine.Bindings;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/BoundProperty.h")]
	public readonly struct BoundProperty : IEquatable<BoundProperty>, IComparable<BoundProperty>
	{
		public int index
		{
			get
			{
				return this.m_Index;
			}
		}

		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		public static BoundProperty Null
		{
			get
			{
				return default(BoundProperty);
			}
		}

		public static bool operator ==(BoundProperty lhs, BoundProperty rhs)
		{
			return lhs.m_Index == rhs.m_Index && lhs.m_Version == rhs.m_Version;
		}

		public static bool operator !=(BoundProperty lhs, BoundProperty rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object compare)
		{
			bool flag;
			if (compare is BoundProperty)
			{
				BoundProperty boundProperty = (BoundProperty)compare;
				flag = this.Equals(boundProperty);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(BoundProperty boundProperty)
		{
			return boundProperty.m_Index == this.m_Index && boundProperty.m_Version == this.m_Version;
		}

		public int CompareTo(BoundProperty other)
		{
			return this.m_Index - other.m_Index;
		}

		public override int GetHashCode()
		{
			return (this.m_Version * 397) ^ this.m_Index;
		}

		private readonly int m_Index;

		private readonly int m_Version;
	}
}
