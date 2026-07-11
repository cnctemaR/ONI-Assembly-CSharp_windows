using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A heightmap based collider.</para>
	/// </summary>
	[NativeHeader("Modules/Terrain/Public/TerrainData.h")]
	[NativeHeader("Runtime/TerrainPhysics/TerrainCollider.h")]
	public class TerrainCollider : Collider
	{
		/// <summary>
		///   <para>The terrain that stores the heightmap.</para>
		/// </summary>
		public extern TerrainData terrainData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
