using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps
{
	[RequireComponent(typeof(Tilemap))]
	[NativeType(Header = "Modules/Tilemap/Public/TilemapCollider2D.h")]
	public sealed class TilemapCollider2D : Collider2D
	{
		public bool useDelaunayMesh
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapCollider2D.get_useDelaunayMesh_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapCollider2D.set_useDelaunayMesh_Injected(intPtr, value);
			}
		}

		public uint maximumTileChangeCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapCollider2D.get_maximumTileChangeCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapCollider2D.set_maximumTileChangeCount_Injected(intPtr, value);
			}
		}

		public float extrusionFactor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapCollider2D.get_extrusionFactor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TilemapCollider2D.set_extrusionFactor_Injected(intPtr, value);
			}
		}

		public bool hasTilemapChanges
		{
			[NativeMethod("HasTilemapChanges")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TilemapCollider2D.get_hasTilemapChanges_Injected(intPtr);
			}
		}

		[NativeMethod(Name = "ProcessTileChangeQueue")]
		public void ProcessTilemapChanges()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TilemapCollider2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TilemapCollider2D.ProcessTilemapChanges_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useDelaunayMesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useDelaunayMesh_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_maximumTileChangeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maximumTileChangeCount_Injected(IntPtr _unity_self, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_extrusionFactor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_extrusionFactor_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasTilemapChanges_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ProcessTilemapChanges_Injected(IntPtr _unity_self);
	}
}
