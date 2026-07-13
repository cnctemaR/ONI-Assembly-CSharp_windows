using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/Flare.h")]
	public sealed class Flare : Object
	{
		public Flare()
		{
			Flare.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Flare self);

		internal Texture texture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(Flare.get_texture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Flare.set_texture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(value));
			}
		}

		internal unsafe Flare.FlareElement[] elements
		{
			get
			{
				Flare.FlareElement[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Flare.get_elements_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Flare.FlareElement[] array;
					blittableArrayWrapper.Unmarshal<Flare.FlareElement>(ref array);
					array2 = array;
				}
				return array2;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Flare.FlareElement> span = new Span<Flare.FlareElement>(value);
				fixed (Flare.FlareElement* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Flare.set_elements_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		internal bool useFog
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Flare.get_useFog_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Flare.set_useFog_Injected(intPtr, value);
			}
		}

		internal int textureLayout
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Flare.get_textureLayout_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Flare>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Flare.set_textureLayout_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_texture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_elements_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_elements_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useFog_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useFog_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_textureLayout_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_textureLayout_Injected(IntPtr _unity_self, int value);

		[UsedByNativeCode]
		[Serializable]
		internal struct FlareElement
		{
			[NativeName("m_ImageIndex")]
			public uint imageIndex;

			[NativeName("m_Position")]
			public float position;

			[NativeName("m_Size")]
			public float size;

			[NativeName("m_Color")]
			public Color color;

			[NativeName("m_UseLightColor")]
			public bool useLightColor;

			[NativeName("m_Rotate")]
			public bool rotate;

			[NativeName("m_Zoom")]
			public bool zoom;

			[NativeName("m_Fade")]
			public bool fade;
		}

		[Serializable]
		internal enum FlareLayout
		{
			LayoutLargeRestSmall,
			LayoutMixed,
			Layout1x1,
			Layout2x2,
			Layout3x3,
			Layout4x4
		}
	}
}
