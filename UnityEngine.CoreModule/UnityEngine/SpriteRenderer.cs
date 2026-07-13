using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform))]
	[NativeType("Runtime/Graphics/Mesh/SpriteRenderer.h")]
	public sealed class SpriteRenderer : Renderer
	{
		public void RegisterSpriteChangeCallback(UnityAction<SpriteRenderer> callback)
		{
			bool flag = this.m_SpriteChangeEvent == null;
			if (flag)
			{
				this.m_SpriteChangeEvent = new UnityEvent<SpriteRenderer>();
			}
			this.m_SpriteChangeEvent.AddListener(callback);
			this.hasSpriteChangeEvents = true;
		}

		public void UnregisterSpriteChangeCallback(UnityAction<SpriteRenderer> callback)
		{
			bool flag = this.m_SpriteChangeEvent != null;
			if (flag)
			{
				this.m_SpriteChangeEvent.RemoveListener(callback);
				bool flag2 = this.m_SpriteChangeEvent.GetCallsCount() == 0;
				if (flag2)
				{
					this.hasSpriteChangeEvents = false;
				}
			}
		}

		[RequiredByNativeCode]
		private void InvokeSpriteChanged()
		{
			try
			{
				UnityEvent<SpriteRenderer> spriteChangeEvent = this.m_SpriteChangeEvent;
				if (spriteChangeEvent != null)
				{
					spriteChangeEvent.Invoke(this);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		internal bool shouldSupportTiling
		{
			[NativeMethod("ShouldSupportTiling")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_shouldSupportTiling_Injected(intPtr);
			}
		}

		internal bool hasSpriteChangeEvents
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_hasSpriteChangeEvents_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_hasSpriteChangeEvents_Injected(intPtr, value);
			}
		}

		public Sprite sprite
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Sprite>(SpriteRenderer.get_sprite_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_sprite_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Sprite>(value));
			}
		}

		public SpriteDrawMode drawMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_drawMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_drawMode_Injected(intPtr, value);
			}
		}

		public Vector2 size
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				SpriteRenderer.get_size_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_size_Injected(intPtr, ref value);
			}
		}

		public float adaptiveModeThreshold
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_adaptiveModeThreshold_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_adaptiveModeThreshold_Injected(intPtr, value);
			}
		}

		public SpriteTileMode tileMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_tileMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_tileMode_Injected(intPtr, value);
			}
		}

		public Color color
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				SpriteRenderer.get_color_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_color_Injected(intPtr, ref value);
			}
		}

		public SpriteMaskInteraction maskInteraction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_maskInteraction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_maskInteraction_Injected(intPtr, value);
			}
		}

		public bool flipX
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_flipX_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_flipX_Injected(intPtr, value);
			}
		}

		public bool flipY
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_flipY_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_flipY_Injected(intPtr, value);
			}
		}

		public SpriteSortPoint spriteSortPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteRenderer.get_spriteSortPoint_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteRenderer.set_spriteSortPoint_Injected(intPtr, value);
			}
		}

		private IntPtr GetCurrentMeshDataPtr()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return SpriteRenderer.GetCurrentMeshDataPtr_Injected(intPtr);
		}

		internal unsafe Mesh.MeshDataArray GetCurrentMeshData()
		{
			IntPtr currentMeshDataPtr = this.GetCurrentMeshDataPtr();
			bool flag = currentMeshDataPtr == IntPtr.Zero;
			Mesh.MeshDataArray meshDataArray;
			if (flag)
			{
				meshDataArray = new Mesh.MeshDataArray(0);
			}
			else
			{
				Mesh.MeshDataArray meshDataArray2 = new Mesh.MeshDataArray(1);
				*meshDataArray2.m_Ptrs = currentMeshDataPtr;
				meshDataArray = meshDataArray2;
			}
			return meshDataArray;
		}

		[NativeMethod(Name = "GetSpriteBounds")]
		internal Bounds Internal_GetSpriteBounds(SpriteDrawMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Bounds bounds;
			SpriteRenderer.Internal_GetSpriteBounds_Injected(intPtr, mode, out bounds);
			return bounds;
		}

		internal void GetSecondaryTextureProperties([NotNull] MaterialPropertyBlock mbp)
		{
			if (mbp == null)
			{
				ThrowHelper.ThrowArgumentNullException(mbp, "mbp");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(mbp);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mbp, "mbp");
			}
			SpriteRenderer.GetSecondaryTextureProperties_Injected(intPtr, intPtr2);
		}

		internal Bounds GetSpriteBounds()
		{
			return this.Internal_GetSpriteBounds(this.drawMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_shouldSupportTiling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasSpriteChangeEvents_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_hasSpriteChangeEvents_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_sprite_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sprite_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteDrawMode get_drawMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_drawMode_Injected(IntPtr _unity_self, SpriteDrawMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_size_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_adaptiveModeThreshold_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_adaptiveModeThreshold_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteTileMode get_tileMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_tileMode_Injected(IntPtr _unity_self, SpriteTileMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_flipX_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flipX_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_flipY_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flipY_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteSortPoint get_spriteSortPoint_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spriteSortPoint_Injected(IntPtr _unity_self, SpriteSortPoint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCurrentMeshDataPtr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetSpriteBounds_Injected(IntPtr _unity_self, SpriteDrawMode mode, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSecondaryTextureProperties_Injected(IntPtr _unity_self, IntPtr mbp);

		private UnityEvent<SpriteRenderer> m_SpriteChangeEvent;
	}
}
