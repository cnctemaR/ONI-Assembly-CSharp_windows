using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Describes a subset of objects to be rendered.
	///
	/// See Also: ScriptableRenderContext.DrawRenderers.</para>
	/// </summary>
	public struct FilterResults
	{
		internal IntPtr m_CullResults;
	}
}
