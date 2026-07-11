using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	public struct MeshGenerationResult : IEquatable<MeshGenerationResult>
	{
		public MeshId MeshId { get; }

		public Mesh Mesh { get; }

		public MeshCollider MeshCollider { get; }

		public MeshGenerationStatus Status { get; }

		public MeshVertexAttributes Attributes { get; }

		public override bool Equals(object obj)
		{
			bool flag = !(obj is MeshGenerationResult);
			return !flag && this.Equals((MeshGenerationResult)obj);
		}

		public bool Equals(MeshGenerationResult other)
		{
			return this.MeshId.Equals(other.MeshId) && this.Mesh.Equals(other.Mesh) && this.MeshCollider.Equals(other.MeshCollider) && this.Status.Equals(other.Status) && this.Attributes.Equals(other.Attributes);
		}

		public static bool operator ==(MeshGenerationResult lhs, MeshGenerationResult rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MeshGenerationResult lhs, MeshGenerationResult rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(HashCodeHelper.Combine(HashCodeHelper.Combine(this.MeshId.GetHashCode(), this.Mesh.GetHashCode()), this.MeshCollider.GetHashCode()), this.Status.GetHashCode()), this.Attributes.GetHashCode());
		}
	}
}
