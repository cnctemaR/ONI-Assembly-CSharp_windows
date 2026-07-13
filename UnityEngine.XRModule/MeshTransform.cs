using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	[UsedByNativeCode]
	public readonly struct MeshTransform : IEquatable<MeshTransform>
	{
		public MeshId MeshId { get; }

		public ulong Timestamp { get; }

		public Vector3 Position { get; }

		public Quaternion Rotation { get; }

		public Vector3 Scale { get; }

		public MeshTransform(in MeshId meshId, ulong timestamp, in Vector3 position, in Quaternion rotation, in Vector3 scale)
		{
			this.MeshId = meshId;
			this.Timestamp = timestamp;
			this.Position = position;
			this.Rotation = rotation;
			this.Scale = scale;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is MeshTransform)
			{
				MeshTransform meshTransform = (MeshTransform)obj;
				flag = this.Equals(meshTransform);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(MeshTransform other)
		{
			return this.MeshId.Equals(other.MeshId) && this.Timestamp == other.Timestamp && this.Position.Equals(other.Position) && this.Rotation.Equals(other.Rotation) && this.Scale.Equals(other.Scale);
		}

		public static bool operator ==(MeshTransform lhs, MeshTransform rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MeshTransform lhs, MeshTransform rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return HashCodeHelper.Combine(this.MeshId.GetHashCode(), this.Timestamp.GetHashCode(), this.Position.GetHashCode(), this.Rotation.GetHashCode(), this.Scale.GetHashCode());
		}
	}
}
