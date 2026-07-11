using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>A helper structure used to initialize a LightDataGI structure as a point light.</para>
	/// </summary>
	public struct PointLight
	{
		/// <summary>
		///   <para>The light's instanceID.</para>
		/// </summary>
		public int instanceID;

		/// <summary>
		///   <para>True if the light casts shadows, otherwise False.</para>
		/// </summary>
		public bool shadow;

		/// <summary>
		///   <para>The lightmode.</para>
		/// </summary>
		public LightMode mode;

		/// <summary>
		///   <para>The light's position.</para>
		/// </summary>
		public Vector3 position;

		/// <summary>
		///   <para>The direct light color.</para>
		/// </summary>
		public LinearColor color;

		/// <summary>
		///   <para>The indirect light color.</para>
		/// </summary>
		public LinearColor indirectColor;

		/// <summary>
		///   <para>The light's range.</para>
		/// </summary>
		public float range;

		/// <summary>
		///   <para>The light's sphere radius, influencing soft shadows.</para>
		/// </summary>
		public float sphereRadius;

		/// <summary>
		///   <para>The falloff model to use for baking the point light.</para>
		/// </summary>
		public FalloffType falloff;
	}
}
