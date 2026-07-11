using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	[UsedByNativeCode]
	public struct MeshInfo : IEquatable<MeshInfo>
	{
		public TrackableId MeshId { get; set; }

		public MeshChangeState ChangeState { get; set; }

		public int PriorityHint { get; set; }

		public override bool Equals(object obj)
		{
			return obj is MeshInfo && this.Equals((MeshInfo)obj);
		}

		public bool Equals(MeshInfo other)
		{
			return this.MeshId.Equals(other.MeshId) && this.ChangeState.Equals(other.ChangeState) && this.PriorityHint.Equals(other.PriorityHint);
		}

		public static bool operator ==(MeshInfo lhs, MeshInfo rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MeshInfo lhs, MeshInfo rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(this.MeshId.GetHashCode(), this.ChangeState.GetHashCode()), this.PriorityHint);
		}
	}
}
