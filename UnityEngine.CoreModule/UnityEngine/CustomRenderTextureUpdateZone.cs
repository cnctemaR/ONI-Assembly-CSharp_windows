using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Structure describing an Update Zone.</para>
	/// </summary>
	[UsedByNativeCode]
	[Serializable]
	public struct CustomRenderTextureUpdateZone
	{
		/// <summary>
		///   <para>Position of the center of the Update Zone within the Custom Render Texture.</para>
		/// </summary>
		public Vector3 updateZoneCenter;

		/// <summary>
		///   <para>Size of the Update Zone.</para>
		/// </summary>
		public Vector3 updateZoneSize;

		/// <summary>
		///   <para>Rotation of the Update Zone.</para>
		/// </summary>
		public float rotation;

		/// <summary>
		///   <para>Shader Pass used to update the Custom Render Texture for this Update Zone.</para>
		/// </summary>
		public int passIndex;

		/// <summary>
		///   <para>If true, and if the texture is double buffered, a request is made to swap the buffers before the next update. Otherwise, the buffers will not be swapped.</para>
		/// </summary>
		public bool needSwap;
	}
}
