using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/TrailRenderer.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public sealed class TrailRenderer : Renderer
	{
		[Obsolete("Use positionCount instead (UnityUpgradable) -> positionCount", false)]
		public int numPositions
		{
			get
			{
				return this.positionCount;
			}
		}

		public extern float time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float startWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float endWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float widthMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autodestruct
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool emitting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int numCornerVertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int numCapVertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float minVertexDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color startColor
		{
			get
			{
				Color color;
				this.get_startColor_Injected(out color);
				return color;
			}
			set
			{
				this.set_startColor_Injected(ref value);
			}
		}

		public Color endColor
		{
			get
			{
				Color color;
				this.get_endColor_Injected(out color);
				return color;
			}
			set
			{
				this.set_endColor_Injected(ref value);
			}
		}

		[NativeProperty("PositionsCount")]
		public extern int positionCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public void SetPosition(int index, Vector3 position)
		{
			this.SetPosition_Injected(index, ref position);
		}

		public Vector3 GetPosition(int index)
		{
			Vector3 vector;
			this.GetPosition_Injected(index, out vector);
			return vector;
		}

		public extern float shadowBias
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool generateLightingData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern LineTextureMode textureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern LineAlignment alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		public void BakeMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeMesh(mesh, Camera.main, useTransform);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false);

		public AnimationCurve widthCurve
		{
			get
			{
				return this.GetWidthCurveCopy();
			}
			set
			{
				this.SetWidthCurve(value);
			}
		}

		public Gradient colorGradient
		{
			get
			{
				return this.GetColorGradientCopy();
			}
			set
			{
				this.SetColorGradient(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationCurve GetWidthCurveCopy();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetWidthCurve([NotNull] AnimationCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Gradient GetColorGradientCopy();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColorGradient([NotNull] Gradient curve);

		[FreeFunction(Name = "TrailRendererScripting::GetPositions", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetPositions([NotNull] [Out] Vector3[] positions);

		[FreeFunction(Name = "TrailRendererScripting::SetPositions", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPositions([NotNull] Vector3[] positions);

		[FreeFunction(Name = "TrailRendererScripting::AddPosition", HasExplicitThis = true)]
		public void AddPosition(Vector3 position)
		{
			this.AddPosition_Injected(ref position);
		}

		[FreeFunction(Name = "TrailRendererScripting::AddPositions", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddPositions([NotNull] Vector3[] positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_startColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_startColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_endColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_endColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPosition_Injected(int index, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPosition_Injected(int index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddPosition_Injected(ref Vector3 position);
	}
}
