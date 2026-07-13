using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType(Header = "Modules/SpriteMask/Public/SpriteMask.h")]
	[RejectDragAndDropMaterial]
	public sealed class SpriteMask : Renderer
	{
		public int frontSortingLayerID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_frontSortingLayerID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_frontSortingLayerID_Injected(intPtr, value);
			}
		}

		public int frontSortingOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_frontSortingOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_frontSortingOrder_Injected(intPtr, value);
			}
		}

		public int backSortingLayerID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_backSortingLayerID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_backSortingLayerID_Injected(intPtr, value);
			}
		}

		public int backSortingOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_backSortingOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_backSortingOrder_Injected(intPtr, value);
			}
		}

		public float alphaCutoff
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_alphaCutoff_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_alphaCutoff_Injected(intPtr, value);
			}
		}

		public Sprite sprite
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Sprite>(SpriteMask.get_sprite_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_sprite_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Sprite>(value));
			}
		}

		public bool isCustomRangeActive
		{
			[NativeMethod("IsCustomRangeActive")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_isCustomRangeActive_Injected(intPtr);
			}
			[NativeMethod("SetCustomRangeActive")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_isCustomRangeActive_Injected(intPtr, value);
			}
		}

		public SpriteSortPoint spriteSortPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_spriteSortPoint_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_spriteSortPoint_Injected(intPtr, value);
			}
		}

		public SpriteMask.MaskSource maskSource
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteMask.get_maskSource_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteMask.set_maskSource_Injected(intPtr, value);
			}
		}

		internal Renderer cachedSupportedRenderer
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Renderer>(SpriteMask.get_cachedSupportedRenderer_Injected(intPtr));
			}
		}

		internal Bounds GetSpriteBounds()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Bounds bounds;
			SpriteMask.GetSpriteBounds_Injected(intPtr, out bounds);
			return bounds;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_frontSortingLayerID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frontSortingLayerID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_frontSortingOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frontSortingOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_backSortingLayerID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_backSortingLayerID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_backSortingOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_backSortingOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_alphaCutoff_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_alphaCutoff_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_sprite_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sprite_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isCustomRangeActive_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isCustomRangeActive_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteSortPoint get_spriteSortPoint_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spriteSortPoint_Injected(IntPtr _unity_self, SpriteSortPoint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMask.MaskSource get_maskSource_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskSource_Injected(IntPtr _unity_self, SpriteMask.MaskSource value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_cachedSupportedRenderer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSpriteBounds_Injected(IntPtr _unity_self, out Bounds ret);

		public enum MaskSource
		{
			Sprite,
			SupportedRenderers
		}
	}
}
