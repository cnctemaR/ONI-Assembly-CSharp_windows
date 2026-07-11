using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The trail renderer is used to make trails behind objects in the scene as they move about.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/TrailRenderer.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public sealed class TrailRenderer : Renderer
	{
		/// <summary>
		///   <para>Get the number of line segments in the trail.</para>
		/// </summary>
		[Obsolete("Use positionCount instead (UnityUpgradable) -> positionCount", false)]
		public int numPositions
		{
			get
			{
				return this.positionCount;
			}
		}

		/// <summary>
		///   <para>How long does the trail take to fade out.</para>
		/// </summary>
		public extern float time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The width of the trail at the spawning point.</para>
		/// </summary>
		public extern float startWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The width of the trail at the end of the trail.</para>
		/// </summary>
		public extern float endWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set an overall multiplier that is applied to the TrailRenderer.widthCurve to get the final width of the trail.</para>
		/// </summary>
		public extern float widthMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Does the GameObject of this Trail Renderer auto destruct?</para>
		/// </summary>
		public extern bool autodestruct
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Creates trails when the GameObject moves.</para>
		/// </summary>
		public extern bool emitting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set this to a value greater than 0, to get rounded corners between each segment of the trail.</para>
		/// </summary>
		public extern int numCornerVertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set this to a value greater than 0, to get rounded corners on each end of the trail.</para>
		/// </summary>
		public extern int numCapVertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set the minimum distance the trail can travel before a new vertex is added to it.</para>
		/// </summary>
		public extern float minVertexDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set the color at the start of the trail.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Set the color at the end of the trail.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Get the number of line segments in the trail.</para>
		/// </summary>
		[NativeProperty("PositionsCount")]
		public extern int positionCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Set the position of a vertex in the trail.</para>
		/// </summary>
		/// <param name="index">Which position to set.</param>
		/// <param name="position">The new position.</param>
		public void SetPosition(int index, Vector3 position)
		{
			this.SetPosition_Injected(index, ref position);
		}

		/// <summary>
		///   <para>Get the position of a vertex in the trail.</para>
		/// </summary>
		/// <param name="index">The index of the position to retrieve.</param>
		/// <returns>
		///   <para>The position at the specified index in the array.</para>
		/// </returns>
		public Vector3 GetPosition(int index)
		{
			Vector3 vector;
			this.GetPosition_Injected(index, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Configures a trail to generate Normals and Tangents. With this data, Scene lighting can affect the trail via Normal Maps and the Unity Standard Shader, or your own custom-built Shaders.</para>
		/// </summary>
		public extern bool generateLightingData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Choose whether the U coordinate of the trail texture is tiled or stretched.</para>
		/// </summary>
		public extern LineTextureMode textureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Select whether the trail will face the camera, or the orientation of the Transform Component.</para>
		/// </summary>
		public extern LineAlignment alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Removes all points from the TrailRenderer.
		/// Useful for restarting a trail from a new position.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		/// <summary>
		///   <para>Creates a snapshot of TrailRenderer and stores it in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the trail.</param>
		/// <param name="camera">The camera used for determining which way view space trails will face.</param>
		/// <param name="useTransform">Include the rotation and scale of the Transform in the baked mesh.</param>
		public void BakeMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeMesh(mesh, Camera.main, useTransform);
		}

		/// <summary>
		///   <para>Creates a snapshot of TrailRenderer and stores it in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the trail.</param>
		/// <param name="camera">The camera used for determining which way view space trails will face.</param>
		/// <param name="useTransform">Include the rotation and scale of the Transform in the baked mesh.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false);

		/// <summary>
		///   <para>Set the curve describing the width of the trail at various points along its length.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Set the color gradient describing the color of the trail at various points along its length.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Get the positions of all vertices in the trail.</para>
		/// </summary>
		/// <param name="positions">The array of positions to retrieve.</param>
		/// <returns>
		///   <para>How many positions were actually stored in the output array.</para>
		/// </returns>
		[FreeFunction(Name = "TrailRendererScripting::GetPositions", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetPositions([NotNull] [Out] Vector3[] positions);

		/// <summary>
		///   <para>Sets the positions of all vertices in the trail.</para>
		/// </summary>
		/// <param name="positions">The array of positions to set.</param>
		[FreeFunction(Name = "TrailRendererScripting::SetPositions", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPositions([NotNull] Vector3[] positions);

		/// <summary>
		///   <para>Adds a position to the trail.</para>
		/// </summary>
		/// <param name="position">The position to add to the trail.</param>
		[FreeFunction(Name = "TrailRendererScripting::AddPosition", HasExplicitThis = true)]
		public void AddPosition(Vector3 position)
		{
			this.AddPosition_Injected(ref position);
		}

		/// <summary>
		///   <para>Add an array of positions to the trail.</para>
		/// </summary>
		/// <param name="positions">The positions to add to the trail.</param>
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
