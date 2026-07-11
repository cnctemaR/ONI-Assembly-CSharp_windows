using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Renders a billboard from a BillboardAsset.</para>
	/// </summary>
	public sealed class BillboardRenderer : Renderer
	{
		/// <summary>
		///   <para>The BillboardAsset to render.</para>
		/// </summary>
		public extern BillboardAsset billboard
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
