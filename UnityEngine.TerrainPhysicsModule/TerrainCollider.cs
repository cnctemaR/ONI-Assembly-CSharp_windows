using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Terrain/Public/TerrainData.h")]
	[NativeHeader("Runtime/TerrainPhysics/TerrainCollider.h")]
	public class TerrainCollider : Collider
	{
		public extern TerrainData terrainData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
