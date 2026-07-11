using System;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>Collider for 2D physics representing shapes defined by the corresponding Tilemap.</para>
	/// </summary>
	[RequireComponent(typeof(Tilemap))]
	[NativeType(Header = "Modules/Tilemap/Public/TilemapCollider2D.h")]
	public sealed class TilemapCollider2D : Collider2D
	{
	}
}
