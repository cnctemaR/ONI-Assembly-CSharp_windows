using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>A session-unique identifier for trackables in the environment, e.g., planes and feature points.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/XRManagedBindings.h")]
	public struct TrackableId : IEquatable<TrackableId>
	{
		/// <summary>
		///   <para>Generates a nicely formatted version of the id.</para>
		/// </summary>
		/// <returns>
		///   <para>A string unique to this id</para>
		/// </returns>
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

		/// <summary>
		///   <para>Represents an invalid id.</para>
		/// </summary>
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
