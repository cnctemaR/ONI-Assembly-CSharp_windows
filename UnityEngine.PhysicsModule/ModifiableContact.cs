using System;

namespace UnityEngine
{
	internal struct ModifiableContact
	{
		public Vector3 contact;

		public float separation;

		public Vector3 targetVelocity;

		public float maxImpulse;

		public Vector3 normal;

		public float restitution;

		public uint materialFlags;

		public ushort materialIndex;

		public ushort otherMaterialIndex;

		public float staticFriction;

		public float dynamicFriction;
	}
}
