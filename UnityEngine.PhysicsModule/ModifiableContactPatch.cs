using System;

namespace UnityEngine
{
	internal struct ModifiableContactPatch
	{
		public ModifiableMassProperties massProperties;

		public Vector3 normal;

		public float restitution;

		public float dynamicFriction;

		public float staticFriction;

		public byte startContactIndex;

		public byte contactCount;

		public byte materialFlags;

		public byte internalFlags;

		public ushort materialIndex;

		public ushort otherMaterialIndex;

		public enum Flags
		{
			HasFaceIndices = 1,
			HasModifiedMassRatios = 8,
			HasTargetVelocity = 16,
			HasMaxImpulse = 32,
			RegeneratePatches = 64
		}
	}
}
