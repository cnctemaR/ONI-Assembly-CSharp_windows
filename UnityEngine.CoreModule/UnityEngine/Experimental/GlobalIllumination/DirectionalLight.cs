using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public struct DirectionalLight
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

		public float penumbraWidthRadian;

		[Obsolete("Directional lights support cookies now. In order to position the cookie projection in the world, a position and full orientation are necessary. Use the position and orientation members instead of the direction parameter.", true)]
		public Vector3 direction;
	}
}
