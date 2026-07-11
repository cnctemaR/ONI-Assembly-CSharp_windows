using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Core Camera related properties in CullingParameters.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct CoreCameraValues
	{
		private int filterMode;

		private uint cullingMask;

		private int guid;

		private int renderImmediateObjects;
	}
}
