using System;

namespace UnityEngine.SceneManagement
{
	/// <summary>
	///   <para>Used when loading a scene in a player.</para>
	/// </summary>
	public enum LoadSceneMode
	{
		/// <summary>
		///   <para>Closes all current loaded scenes and loads a scene.</para>
		/// </summary>
		Single,
		/// <summary>
		///   <para>Adds the scene to the current loaded scenes.</para>
		/// </summary>
		Additive
	}
}
