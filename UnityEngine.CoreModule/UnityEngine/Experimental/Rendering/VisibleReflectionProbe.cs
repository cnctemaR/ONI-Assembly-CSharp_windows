using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Holds data of a visible reflection probe.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct VisibleReflectionProbe
	{
		/// <summary>
		///   <para>Probe texture.</para>
		/// </summary>
		public Texture texture
		{
			get
			{
				return VisibleReflectionProbe.GetTextureObject(this.textureId);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture GetTextureObject(int textureId);

		/// <summary>
		///   <para>Accessor to ReflectionProbe component.</para>
		/// </summary>
		public ReflectionProbe probe
		{
			get
			{
				return VisibleReflectionProbe.GetReflectionProbeObject(this.instanceId);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbe GetReflectionProbeObject(int instanceId);

		/// <summary>
		///   <para>Probe bounding box.</para>
		/// </summary>
		public Bounds bounds;

		/// <summary>
		///   <para>Probe transformation matrix.</para>
		/// </summary>
		public Matrix4x4 localToWorld;

		/// <summary>
		///   <para>Shader data for probe HDR texture decoding.</para>
		/// </summary>
		public Vector4 hdr;

		/// <summary>
		///   <para>Probe projection center.</para>
		/// </summary>
		public Vector3 center;

		/// <summary>
		///   <para>Probe blending distance.</para>
		/// </summary>
		public float blendDistance;

		/// <summary>
		///   <para>Probe importance.</para>
		/// </summary>
		public int importance;

		/// <summary>
		///   <para>Should probe use box projection.</para>
		/// </summary>
		public int boxProjection;

		private int instanceId;

		private int textureId;
	}
}
