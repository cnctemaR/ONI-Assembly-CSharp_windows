using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/XRManagedBindings.h")]
	[UsedByNativeCode]
	public struct TrackableId : IEquatable<TrackableId>
	{
		public override string ToString()
		{
			return string.Format("{0}-{1}", this.m_SubId1.ToString("X16"), this.m_SubId2.ToString("X16"));
		}

		public override int GetHashCode()
		{
			return this.m_SubId1.GetHashCode() ^ this.m_SubId2.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is TrackableId && this.Equals((TrackableId)obj);
		}

		public bool Equals(TrackableId other)
		{
			return this.m_SubId1 == other.m_SubId1 && this.m_SubId2 == other.m_SubId2;
		}

		public static bool operator ==(TrackableId id1, TrackableId id2)
		{
			return id1.m_SubId1 == id2.m_SubId1 && id1.m_SubId2 == id2.m_SubId2;
		}

		public static bool operator !=(TrackableId id1, TrackableId id2)
		{
			return id1.m_SubId1 != id2.m_SubId1 || id1.m_SubId2 != id2.m_SubId2;
		}

		public static TrackableId InvalidId
		{
			get
			{
				return TrackableId.s_InvalidId;
			}
		}

		private static TrackableId s_InvalidId = default(TrackableId);

		private ulong m_SubId1;

		private ulong m_SubId2;
	}
}
