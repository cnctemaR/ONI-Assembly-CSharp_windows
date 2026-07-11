using System;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps
{
	[RequireComponent(typeof(Tilemap))]
	[NativeType(Header = "Modules/Tilemap/Public/TilemapCollider2D.h")]
	public sealed class TilemapCollider2D : Collider2D
	{
	}
}
