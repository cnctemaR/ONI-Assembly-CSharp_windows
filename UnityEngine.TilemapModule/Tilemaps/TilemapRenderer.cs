using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>The tile map renderer is used to render the tile map marked out by a component.</para>
	/// </summary>
	[NativeType(Header = "Modules/Tilemap/Public/TilemapRenderer.h")]
	[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
	[NativeHeader("Modules/Tilemap/TilemapRendererJobs.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[RequireComponent(typeof(Tilemap))]
	public sealed class TilemapRenderer : Renderer
	{
		/// <summary>
		///   <para>Size in number of tiles of each chunk created by the TilemapRenderer.</para>
		/// </summary>
		public Vector3Int chunkSize
		{
			get
			{
				Vector3Int vector3Int;
				this.get_chunkSize_Injected(out vector3Int);
				return vector3Int;
			}
			set
			{
				this.set_chunkSize_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Bounds used for culling of Tilemap chunks.</para>
		/// </summary>
		public Vector3 chunkCullingBounds
		{
			[FreeFunction("TilemapRendererBindings::GetChunkCullingBounds", HasExplicitThis = true)]
			get
			{
				Vector3 vector;
				this.get_chunkCullingBounds_Injected(out vector);
				return vector;
			}
			[FreeFunction("TilemapRendererBindings::SetChunkCullingBounds", HasExplicitThis = true)]
			set
			{
				this.set_chunkCullingBounds_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Maximum number of chunks the TilemapRenderer caches in memory.</para>
		/// </summary>
		public extern int maxChunkCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Maximum number of frames the TilemapRenderer keeps unused chunks in memory.</para>
		/// </summary>
		public extern int maxFrameAge
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Active sort order for the TilemapRenderer.</para>
		/// </summary>
		public extern TilemapRenderer.SortOrder sortOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns whether the TilemapRenderer automatically detects the bounds to extend chunk culling by.</para>
		/// </summary>
		public extern TilemapRenderer.DetectChunkCullingBounds detectChunkCullingBounds
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specifies how the Tilemap interacts with the masks.</para>
		/// </summary>
		public extern SpriteMaskInteraction maskInteraction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_chunkSize_Injected(out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_chunkSize_Injected(ref Vector3Int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_chunkCullingBounds_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_chunkCullingBounds_Injected(ref Vector3 value);

		/// <summary>
		///   <para>Sort order for all tiles rendered by the TilemapRenderer.</para>
		/// </summary>
		public enum SortOrder
		{
			/// <summary>
			///   <para>Sorts tiles for rendering starting from the tile with the lowest X and the lowest Y cell positions.</para>
			/// </summary>
			BottomLeft,
			/// <summary>
			///   <para>Sorts tiles for rendering starting from the tile with the highest X and the lowest Y cell positions.</para>
			/// </summary>
			BottomRight,
			/// <summary>
			///   <para>Sorts tiles for rendering starting from the tile with the lowest X and the highest Y cell positions.</para>
			/// </summary>
			TopLeft,
			/// <summary>
			///   <para>Sorts tiles for rendering starting from the tile with the highest X and the lowest Y cell positions.</para>
			/// </summary>
			TopRight
		}

		/// <summary>
		///   <para>Returns whether the TilemapRenderer automatically detects the bounds to extend chunk culling by.</para>
		/// </summary>
		public enum DetectChunkCullingBounds
		{
			/// <summary>
			///   <para>The TilemapRenderer will automatically detect the bounds of extension by inspecting the Sprite/s used in the Tilemap.</para>
			/// </summary>
			Auto,
			/// <summary>
			///   <para>The user adds in the values used for extend the bounds for culling of Tilemap chunks.</para>
			/// </summary>
			Manual
		}
	}
}
