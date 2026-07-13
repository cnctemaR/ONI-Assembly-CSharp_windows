using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination
{
	[UsedByNativeCode]
	public struct LightDataGI
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

		[Obsolete("Please use cookieTextureEntityId instead.", false)]
		public int cookieID
		{
			get
			{
				return this.cookieTextureEntityId;
			}
			set
			{
				this.cookieTextureEntityId = value;
			}
		}

		public void Init(ref DirectionalLight light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = 0f;
			this.coneAngle = cookie.sizes.x;
			this.innerConeAngle = cookie.sizes.y;
			this.shape0 = light.penumbraWidthRadian;
			this.shape1 = 0f;
			this.type = LightType.Directional;
			this.mode = light.mode;
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = FalloffType.Undefined;
		}

		public void Init(ref PointLight light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = 0f;
			this.innerConeAngle = 0f;
			this.shape0 = light.sphereRadius;
			this.shape1 = 0f;
			this.type = LightType.Point;
			this.mode = light.mode;
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = light.falloff;
		}

		public void Init(ref SpotLight light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = light.coneAngle;
			this.innerConeAngle = light.innerConeAngle;
			this.shape0 = light.sphereRadius;
			this.shape1 = (float)light.angularFalloff;
			this.type = LightType.Spot;
			this.mode = light.mode;
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = light.falloff;
		}

		public void Init(ref RectangleLight light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
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
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = light.falloff;
		}

		public void Init(ref DiscLight light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
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
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = light.falloff;
		}

		public void Init(ref SpotLightBoxShape light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = 0f;
			this.innerConeAngle = 0f;
			this.shape0 = light.width;
			this.shape1 = light.height;
			this.type = LightType.SpotBoxShape;
			this.mode = light.mode;
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = FalloffType.Undefined;
		}

		public void Init(ref SpotLightPyramidShape light, ref Cookie cookie)
		{
			this.entityId = light.entityId;
			this.cookieTextureEntityId = cookie.entityId;
			this.cookieScale = cookie.scale;
			this.color = light.color;
			this.indirectColor = light.indirectColor;
			this.orientation = light.orientation;
			this.position = light.position;
			this.range = light.range;
			this.coneAngle = light.angle;
			this.innerConeAngle = 0f;
			this.shape0 = light.aspectRatio;
			this.shape1 = 0f;
			this.type = LightType.SpotPyramidShape;
			this.mode = light.mode;
			this.shadow = (light.shadow ? 1 : 0);
			this.falloff = light.falloff;
		}

		public void Init(ref DirectionalLight light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void Init(ref PointLight light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void Init(ref SpotLight light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void Init(ref RectangleLight light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void Init(ref DiscLight light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void Init(ref SpotLightBoxShape light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void Init(ref SpotLightPyramidShape light)
		{
			Cookie cookie = Cookie.Defaults();
			this.Init(ref light, ref cookie);
		}

		public void InitNoBake(EntityId lightEntityId)
		{
			this.entityId = lightEntityId;
			this.mode = LightMode.Unknown;
		}

		[Obsolete("Please use InitNoBake with an EntityId argument instead.", false)]
		public void InitNoBake(int lightInstanceID)
		{
			this.entityId = lightInstanceID;
			this.mode = LightMode.Unknown;
		}

		public EntityId entityId;

		public EntityId cookieTextureEntityId;

		public float cookieScale;

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
