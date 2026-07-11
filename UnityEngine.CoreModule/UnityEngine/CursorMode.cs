using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Determines whether the mouse cursor is rendered using software rendering or, on supported platforms, hardware rendering.</para>
	/// </summary>
	public enum CursorMode
	{
		/// <summary>
		///   <para>Use hardware cursors on supported platforms.</para>
		/// </summary>
		Auto,
		/// <summary>
		///   <para>Force the use of software cursors.</para>
		/// </summary>
		ForceSoftware
	}
}
