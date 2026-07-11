using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>LODGroup lets you group multiple Renderers into LOD levels.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/LOD/LODGroup.h")]
	[NativeHeader("Runtime/Graphics/LOD/LODGroupManager.h")]
	[NativeHeader("Runtime/Graphics/LOD/LODUtility.h")]
	[StaticAccessor("GetLODGroupManager()", StaticAccessorType.Dot)]
	public class LODGroup : Component
	{
		/// <summary>
		///   <para>The local reference point against which the LOD distance is calculated.</para>
		/// </summary>
		public Vector3 localReferencePoint
		{
			get
			{
				Vector3 vector;
				this.get_localReferencePoint_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_localReferencePoint_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The size of the LOD object in local space.</para>
		/// </summary>
		public extern float size
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The number of LOD levels.</para>
		/// </summary>
		public extern int lodCount
		{
			[NativeMethod("GetLODCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The LOD fade mode used.</para>
		/// </summary>
		public extern LODFadeMode fadeMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specify if the cross-fading should be animated by time. The animation duration is specified globally as crossFadeAnimationDuration.</para>
		/// </summary>
		public extern bool animateCrossFading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enable / Disable the LODGroup - Disabling will turn off all renderers.</para>
		/// </summary>
		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Recalculate the bounding region for the LODGroup (Relatively slow, do not call often).</para>
		/// </summary>
		[FreeFunction("UpdateLODGroupBoundingBox", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateBounds();

		/// <summary>
		///   <para>Returns the array of LODs.</para>
		/// </summary>
		/// <returns>
		///   <para>The LOD array.</para>
		/// </returns>
		[FreeFunction("GetLODs_Binding", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern LOD[] GetLODs();

		[Obsolete("Use SetLODs instead.")]
		public void SetLODS(LOD[] lods)
		{
			this.SetLODs(lods);
		}

		/// <summary>
		///   <para>Set the LODs for the LOD group. This will remove any existing LODs configured on the LODGroup.</para>
		/// </summary>
		/// <param name="lods">The LODs to use for this group.</param>
		[FreeFunction("SetLODs_Binding", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLODs(LOD[] lods);

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="index">The LOD level to use. Passing index &lt; 0 will return to standard LOD processing.</param>
		[FreeFunction("ForceLODLevel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ForceLOD(int index);

		/// <summary>
		///   <para>The cross-fading animation duration in seconds. ArgumentException will be thrown if it is set to zero or a negative value.</para>
		/// </summary>
		[StaticAccessor("GetLODGroupManager()")]
		public static extern float crossFadeAnimationDuration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_localReferencePoint_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_localReferencePoint_Injected(ref Vector3 value);
	}
}
