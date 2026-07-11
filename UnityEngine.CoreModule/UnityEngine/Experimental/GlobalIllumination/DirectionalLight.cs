using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>A helper structure used to initialize a LightDataGI structure as a directional light.</para>
	/// </summary>
	public struct DirectionalLight
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
		///   <para>The direction of the light.</para>
		/// </summary>
		public Vector3 direction;

		/// <summary>
		///   <para>The direct light color.</para>
		/// </summary>
		public LinearColor color;

		/// <summary>
		///   <para>The indirect light color.</para>
		/// </summary>
		public LinearColor indirectColor;

		/// <summary>
		///   <para>The penumbra width for soft shadows in radians.</para>
		/// </summary>
		public float penumbraWidthRadian;
	}
}
