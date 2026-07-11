using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class that specifies some information about a renderable character.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct UICharInfo
	{
		/// <summary>
		///   <para>Position of the character cursor in local (text generated) space.</para>
		/// </summary>
		public Vector2 cursorPos;

		/// <summary>
		///   <para>Character width.</para>
		/// </summary>
		public float charWidth;
	}
}
