using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public struct PointLight
	{
		[Obsolete("Please use entityId instead.", false)]
		public int instanceID
		{
			get
			{
				return this.entityId;
			}
			set
			{
				this.entityId = value;
			}
		}

		public EntityId entityId;

		public bool shadow;

		public LightMode mode;

		public Vector3 position;

		public Quaternion orientation;

		public LinearColor color;

		public LinearColor indirectColor;

		public float range;

		public float sphereRadius;

		public FalloffType falloff;
	}
}
