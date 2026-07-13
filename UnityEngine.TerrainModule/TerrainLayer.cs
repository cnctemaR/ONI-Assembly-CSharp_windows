using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Terrain/Public/TerrainLayerScriptingInterface.h")]
	[NativeHeader("TerrainScriptingClasses.h")]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TerrainLayer : Object
	{
		public TerrainLayer()
		{
			TerrainLayer.Internal_Create(this);
		}

		[FreeFunction("TerrainLayerScriptingInterface::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] TerrainLayer layer);

		public Texture2D diffuseTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture2D>(TerrainLayer.get_diffuseTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_diffuseTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture2D>(value));
			}
		}

		public Texture2D normalMapTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture2D>(TerrainLayer.get_normalMapTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_normalMapTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture2D>(value));
			}
		}

		public Texture2D maskMapTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture2D>(TerrainLayer.get_maskMapTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_maskMapTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture2D>(value));
			}
		}

		public Vector2 tileSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				TerrainLayer.get_tileSize_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_tileSize_Injected(intPtr, ref value);
			}
		}

		public Vector2 tileOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				TerrainLayer.get_tileOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_tileOffset_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("SpecularColor")]
		public Color specular
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				TerrainLayer.get_specular_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_specular_Injected(intPtr, ref value);
			}
		}

		public float metallic
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainLayer.get_metallic_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_metallic_Injected(intPtr, value);
			}
		}

		public float smoothness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainLayer.get_smoothness_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_smoothness_Injected(intPtr, value);
			}
		}

		public float normalScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainLayer.get_normalScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_normalScale_Injected(intPtr, value);
			}
		}

		public Vector4 diffuseRemapMin
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				TerrainLayer.get_diffuseRemapMin_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_diffuseRemapMin_Injected(intPtr, ref value);
			}
		}

		public Vector4 diffuseRemapMax
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				TerrainLayer.get_diffuseRemapMax_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_diffuseRemapMax_Injected(intPtr, ref value);
			}
		}

		public Vector4 maskMapRemapMin
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				TerrainLayer.get_maskMapRemapMin_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_maskMapRemapMin_Injected(intPtr, ref value);
			}
		}

		public Vector4 maskMapRemapMax
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				TerrainLayer.get_maskMapRemapMax_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_maskMapRemapMax_Injected(intPtr, ref value);
			}
		}

		public TerrainLayerSmoothnessSource smoothnessSource
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TerrainLayer.get_smoothnessSource_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainLayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainLayer.set_smoothnessSource_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_diffuseTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_diffuseTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_normalMapTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_normalMapTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_maskMapTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskMapTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_tileSize_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_tileSize_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_tileOffset_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_tileOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_specular_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_specular_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_metallic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_metallic_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_smoothness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_smoothness_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_normalScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_normalScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_diffuseRemapMin_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_diffuseRemapMin_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_diffuseRemapMax_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_diffuseRemapMax_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_maskMapRemapMin_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskMapRemapMin_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_maskMapRemapMax_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskMapRemapMax_Injected(IntPtr _unity_self, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TerrainLayerSmoothnessSource get_smoothnessSource_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_smoothnessSource_Injected(IntPtr _unity_self, TerrainLayerSmoothnessSource value);
	}
}
