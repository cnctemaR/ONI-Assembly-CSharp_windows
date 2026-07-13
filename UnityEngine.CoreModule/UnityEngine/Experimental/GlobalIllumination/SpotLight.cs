using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public struct SpotLight
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

		public float coneAngle;

		public float innerConeAngle;

		public FalloffType falloff;

		public AngularFalloffType angularFalloff;
	}
}
