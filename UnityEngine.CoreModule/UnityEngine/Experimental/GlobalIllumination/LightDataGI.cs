using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>The interop structure to pass light information to the light baking backends. There are helper structures for Directional, Point, Spot and Rectangle lights to correctly initialize this structure.</para>
	/// </summary>
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

		/// <summary>
		///   <para>Initialize a light so that the baking backends ignore it.</para>
		/// </summary>
		/// <param name="lightInstanceID"></param>
		public void InitNoBake(int lightInstanceID)
		{
			this.instanceID = lightInstanceID;
			this.mode = LightMode.Unknown;
		}

		/// <summary>
		///   <para>The light's instanceID.</para>
		/// </summary>
		public int instanceID;

		/// <summary>
		///   <para>The color of the light.</para>
		/// </summary>
		public LinearColor color;

		/// <summary>
		///   <para>The indirect color of the light.</para>
		/// </summary>
		public LinearColor indirectColor;

		/// <summary>
		///   <para>The orientation of the light.</para>
		/// </summary>
		public Quaternion orientation;

		/// <summary>
		///   <para>The position of the light.</para>
		/// </summary>
		public Vector3 position;

		/// <summary>
		///   <para>The range of the light. Unused for directional lights.</para>
		/// </summary>
		public float range;

		/// <summary>
		///   <para>The cone angle for spot lights.</para>
		/// </summary>
		public float coneAngle;

		/// <summary>
		///   <para>The inner cone angle for spot lights.</para>
		/// </summary>
		public float innerConeAngle;

		/// <summary>
		///   <para>The light's sphere radius for point and spot lights, or the width for rectangle lights.</para>
		/// </summary>
		public float shape0;

		/// <summary>
		///   <para>The height for rectangle lights.</para>
		/// </summary>
		public float shape1;

		/// <summary>
		///   <para>The type of the light.</para>
		/// </summary>
		public LightType type;

		/// <summary>
		///   <para>The lightmap mode for the light.</para>
		/// </summary>
		public LightMode mode;

		/// <summary>
		///   <para>Set to 1 for shadow casting lights, 0 otherwise.</para>
		/// </summary>
		public byte shadow;

		/// <summary>
		///   <para>The falloff model to use for baking point and spot lights.</para>
		/// </summary>
		public FalloffType falloff;
	}
}
