using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination
{
	[UsedByNativeCode]
	public struct LightDataGI
	{
		public void Init(ref DirectionalLight light)
		{
			this.instanceID = light.instanceID;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation.SetLookRotation(light.direction, Vector3.up);
			this.position = Vector3.zero;
			this.range = 0f;
			this.coneAngle = 0f;
			this.innerConeAngle = 0f;
			this.shape0 = light.penumbraWidthRadian;
			this.shape1 = 0f;
			this.type = LightType.Directional;
			this.mode = light.mode;
			this.shadow = ((!light.shadow) ? 0 : 1);
			this.falloff = FalloffType.Undefined;
		}

		public void Init(ref PointLight light)
		{
			this.instanceID = light.instanceID;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = Quaternion.identity;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = 0f;
			this.innerConeAngle = 0f;
			this.shape0 = light.sphereRadius;
			this.shape1 = 0f;
			this.type = LightType.Point;
			this.mode = light.mode;
			this.shadow = ((!light.shadow) ? 0 : 1);
			this.falloff = light.falloff;
		}

		public void Init(ref SpotLight light)
		{
			this.instanceID = light.instanceID;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = light.coneAngle;
			this.innerConeAngle = light.innerConeAngle;
			this.shape0 = light.sphereRadius;
			this.shape1 = 0f;
			this.type = LightType.Spot;
			this.mode = light.mode;
			this.shadow = ((!light.shadow) ? 0 : 1);
			this.falloff = light.falloff;
		}

		public void Init(ref RectangleLight light)
		{
			this.instanceID = light.instanceID;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = 0f;
			this.innerConeAngle = 0f;
			this.shape0 = light.width;
			this.shape1 = light.height;
			this.type = LightType.Rectangle;
			this.mode = light.mode;
			this.shadow = ((!light.shadow) ? 0 : 1);
			this.falloff = FalloffType.Undefined;
		}

		public void Init(ref DiscLight light)
		{
			this.instanceID = light.instanceID;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = 0f;
			this.innerConeAngle = 0f;
			this.shape0 = light.radius;
			this.shape1 = 0f;
			this.type = LightType.Disc;
			this.mode = light.mode;
			this.shadow = ((!light.shadow) ? 0 : 1);
			this.falloff = FalloffType.Undefined;
		}

		public void InitNoBake(int lightInstanceID)
		{
			this.instanceID = lightInstanceID;
			this.mode = LightMode.Unknown;
		}

		public int instanceID;

		public LinearColor color;

		public LinearColor indirectColor;

		public Quaternion orientation;

		public Vector3 position;

		public float range;

		public float coneAngle;

		public float innerConeAngle;

		public float shape0;

		public float shape1;

		public LightType type;

		public LightMode mode;

		public byte shadow;

		public FalloffType falloff;
	}
}
