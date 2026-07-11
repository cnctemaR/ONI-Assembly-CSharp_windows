using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Information about a generated line of text.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct UILineInfo
	{
		/// <summary>
		///   <para>Index of the first character in the line.</para>
		/// </summary>
		public int startCharIdx;

		/// <summary>
		///   <para>Height of the line.</para>
		/// </summary>
		public int height;

		/// <summary>
		///   <para>The upper Y position of the line in pixels. This is used for text annotation such as the caret and selection box in the InputField.</para>
		/// </summary>
		public float topY;

		/// <summary>
		///   <para>Space in pixels between this line and the next line.</para>
		/// </summary>
		public float leading;
	}
}
