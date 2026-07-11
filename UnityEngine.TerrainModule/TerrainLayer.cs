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

		public extern Texture2D diffuseTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Texture2D normalMapTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Texture2D maskMapTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 tileSize
		{
			get
			{
				Vector2 vector;
				this.get_tileSize_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_tileSize_Injected(ref value);
			}
		}

		public Vector2 tileOffset
		{
			get
			{
				Vector2 vector;
				this.get_tileOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_tileOffset_Injected(ref value);
			}
		}

		[NativeProperty("SpecularColor")]
		public Color specular
		{
			get
			{
				Color color;
				this.get_specular_Injected(out color);
				return color;
			}
			set
			{
				this.set_specular_Injected(ref value);
			}
		}

		public extern float metallic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float smoothness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float normalScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector4 diffuseRemapMin
		{
			get
			{
				Vector4 vector;
				this.get_diffuseRemapMin_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_diffuseRemapMin_Injected(ref value);
			}
		}

		public Vector4 diffuseRemapMax
		{
			get
			{
				Vector4 vector;
				this.get_diffuseRemapMax_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_diffuseRemapMax_Injected(ref value);
			}
		}

		public Vector4 maskMapRemapMin
		{
			get
			{
				Vector4 vector;
				this.get_maskMapRemapMin_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_maskMapRemapMin_Injected(ref value);
			}
		}

		public Vector4 maskMapRemapMax
		{
			get
			{
				Vector4 vector;
				this.get_maskMapRemapMax_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_maskMapRemapMax_Injected(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_tileSize_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_tileSize_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_tileOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_tileOffset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_specular_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_specular_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_diffuseRemapMin_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_diffuseRemapMin_Injected(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_diffuseRemapMax_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_diffuseRemapMax_Injected(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_maskMapRemapMin_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_maskMapRemapMin_Injected(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_maskMapRemapMax_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_maskMapRemapMax_Injected(ref Vector4 value);
	}
}
