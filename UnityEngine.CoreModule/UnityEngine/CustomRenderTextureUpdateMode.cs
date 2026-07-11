using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Frequency of update or initialization of a Custom Render Texture.</para>
	/// </summary>
	public enum CustomRenderTextureUpdateMode
	{
		/// <summary>
		///   <para>Initialization/Update will occur once at load time and then can be triggered again by script.</para>
		/// </summary>
		OnLoad,
		/// <summary>
		///   <para>Initialization/Update will occur at every frame.</para>
		/// </summary>
		Realtime,
		/// <summary>
		///   <para>Initialization/Update will only occur when triggered by the script.</para>
		/// </summary>
		OnDemand
	}
}
