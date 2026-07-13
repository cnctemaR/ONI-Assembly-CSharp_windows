using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.U2D;

namespace UnityEngine.Tilemaps
{
	[RequireComponent(typeof(Tilemap))]
	[NativeHeader("Modules/Tilemap/TilemapRendererJobs.h")]
	[NativeType(Header = "Modules/Tilemap/Public/TilemapRenderer.h")]
	[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	public sealed class TilemapRenderer : Renderer
	{
		public Vector3Int chunkSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3Int vector3Int;
				TilemapRenderer.get_chunkSize_Injected(intPtr, out vector3Int);
				return vector3Int;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_chunkSize_Injected(intPtr, ref value);
			}
		}

		public Vector3 chunkCullingBounds
		{
			[FreeFunction("TilemapRendererBindings::GetChunkCullingBounds", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				TilemapRenderer.get_chunkCullingBounds_Injected(intPtr, out vector);
				return vector;
			}
			[FreeFunction("TilemapRendererBindings::SetChunkCullingBounds", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_chunkCullingBounds_Injected(intPtr, ref value);
			}
		}

		public int maxChunkCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapRenderer.get_maxChunkCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_maxChunkCount_Injected(intPtr, value);
			}
		}

		public int maxFrameAge
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapRenderer.get_maxFrameAge_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_maxFrameAge_Injected(intPtr, value);
			}
		}

		public TilemapRenderer.SortOrder sortOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapRenderer.get_sortOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_sortOrder_Injected(intPtr, value);
			}
		}

		[NativeProperty("RenderMode")]
		public TilemapRenderer.Mode mode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapRenderer.get_mode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_mode_Injected(intPtr, value);
			}
		}

		public TilemapRenderer.DetectChunkCullingBounds detectChunkCullingBounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapRenderer.get_detectChunkCullingBounds_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_detectChunkCullingBounds_Injected(intPtr, value);
			}
		}

		public SpriteMaskInteraction maskInteraction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapRenderer.get_maskInteraction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapRenderer.set_maskInteraction_Injected(intPtr, value);
			}
		}

		[RequiredByNativeCode]
		internal void RegisterSpriteAtlasRegistered()
		{
			SpriteAtlasManager.atlasRegistered += this.OnSpriteAtlasRegistered;
		}

		[RequiredByNativeCode]
		internal void UnregisterSpriteAtlasRegistered()
		{
			SpriteAtlasManager.atlasRegistered -= this.OnSpriteAtlasRegistered;
		}

		internal void OnSpriteAtlasRegistered(SpriteAtlas atlas)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TilemapRenderer.OnSpriteAtlasRegistered_Injected(intPtr, Object.MarshalledUnityObject.Marshal<SpriteAtlas>(atlas));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_chunkSize_Injected(IntPtr _unity_self, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_chunkSize_Injected(IntPtr _unity_self, [In] ref Vector3Int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_chunkCullingBounds_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_chunkCullingBounds_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_maxChunkCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxChunkCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_maxFrameAge_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxFrameAge_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TilemapRenderer.SortOrder get_sortOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortOrder_Injected(IntPtr _unity_self, TilemapRenderer.SortOrder value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TilemapRenderer.Mode get_mode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mode_Injected(IntPtr _unity_self, TilemapRenderer.Mode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TilemapRenderer.DetectChunkCullingBounds get_detectChunkCullingBounds_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_detectChunkCullingBounds_Injected(IntPtr _unity_self, TilemapRenderer.DetectChunkCullingBounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OnSpriteAtlasRegistered_Injected(IntPtr _unity_self, IntPtr atlas);

		public enum SortOrder
		{
			BottomLeft,
			BottomRight,
			TopLeft,
			TopRight
		}

		public enum Mode
		{
			Chunk,
			Individual,
			SRPBatch
		}

		public enum DetectChunkCullingBounds
		{
			Auto,
			Manual
		}
	}
}
