using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The reflection probe is used to capture the surroundings into a texture which is passed to the shaders and used for reflections.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/ReflectionProbes.h")]
	public sealed class ReflectionProbe : Behaviour
	{
		[Obsolete("type property has been deprecated. Starting with Unity 5.4, the only supported reflection probe type is Cube.", true)]
		[NativeName("ProbeType")]
		public extern ReflectionProbeType type
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The size of the box area in which reflections will be applied to the objects. Measured in the probes's local space.</para>
		/// </summary>
		[NativeName("BoxSize")]
		public Vector3 size
		{
			get
			{
				Vector3 vector;
				this.get_size_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_size_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The center of the box area in which reflections will be applied to the objects. Measured in the probes's local space.</para>
		/// </summary>
		[NativeName("BoxOffset")]
		public Vector3 center
		{
			get
			{
				Vector3 vector;
				this.get_center_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_center_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The near clipping plane distance when rendering the probe.</para>
		/// </summary>
		[NativeName("Near")]
		public extern float nearClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The far clipping plane distance when rendering the probe.</para>
		/// </summary>
		[NativeName("Far")]
		public extern float farClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The intensity modifier that is applied to the texture of reflection probe in the shader.</para>
		/// </summary>
		[NativeName("IntensityMultiplier")]
		public extern float intensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bounding volume of the reflection probe (Read Only).</para>
		/// </summary>
		[NativeName("GlobalAABB")]
		public Bounds bounds
		{
			get
			{
				Bounds bounds;
				this.get_bounds_Injected(out bounds);
				return bounds;
			}
		}

		/// <summary>
		///   <para>Should this reflection probe use HDR rendering?</para>
		/// </summary>
		[NativeName("HDR")]
		public extern bool hdr
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Shadow drawing distance when rendering the probe.</para>
		/// </summary>
		public extern float shadowDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Resolution of the underlying reflection texture in pixels.</para>
		/// </summary>
		public extern int resolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>This is used to render parts of the reflecion probe's surrounding selectively.</para>
		/// </summary>
		public extern int cullingMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How the reflection probe clears the background.</para>
		/// </summary>
		public extern ReflectionProbeClearFlags clearFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The color with which the texture of reflection probe will be cleared.</para>
		/// </summary>
		public Color backgroundColor
		{
			get
			{
				Color color;
				this.get_backgroundColor_Injected(out color);
				return color;
			}
			set
			{
				this.set_backgroundColor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Distance around probe used for blending (used in deferred probes).</para>
		/// </summary>
		public extern float blendDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should this reflection probe use box projection?</para>
		/// </summary>
		public extern bool boxProjection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should reflection probe texture be generated in the Editor (ReflectionProbeMode.Baked) or should probe use custom specified texure (ReflectionProbeMode.Custom)?</para>
		/// </summary>
		public extern ReflectionProbeMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Reflection probe importance.</para>
		/// </summary>
		public extern int importance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the way the probe will refresh.
		///
		/// See Also: ReflectionProbeRefreshMode.</para>
		/// </summary>
		public extern ReflectionProbeRefreshMode refreshMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets this probe time-slicing mode
		///
		/// See Also: ReflectionProbeTimeSlicingMode.</para>
		/// </summary>
		public extern ReflectionProbeTimeSlicingMode timeSlicingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Reference to the baked texture of the reflection probe's surrounding.</para>
		/// </summary>
		public extern Texture bakedTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Reference to the baked texture of the reflection probe's surrounding. Use this to assign custom reflection texture.</para>
		/// </summary>
		public extern Texture customBakedTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Texture which is passed to the shader of the objects in the vicinity of the reflection probe (Read Only).</para>
		/// </summary>
		public extern Texture texture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>HDR decode values of the reflection probe texture.</para>
		/// </summary>
		public Vector4 textureHDRDecodeValues
		{
			[NativeName("CalculateHDRDecodeValues")]
			get
			{
				Vector4 vector;
				this.get_textureHDRDecodeValues_Injected(out vector);
				return vector;
			}
		}

		public int RenderProbe()
		{
			return this.RenderProbe(null);
		}

		/// <summary>
		///   <para>Refreshes the probe's cubemap.</para>
		/// </summary>
		/// <param name="targetTexture">Target RendeTexture in which rendering should be done. Specifying null will update the probe's default texture.</param>
		/// <returns>
		///   <para>
		///     An integer representing a RenderID which can subsequently be used to check if the probe has finished rendering while rendering in time-slice mode.
		///
		///     See Also: IsFinishedRendering
		///     See Also: timeSlicingMode
		///   </para>
		/// </returns>
		public int RenderProbe([DefaultValue("null")] RenderTexture targetTexture)
		{
			return this.ScheduleRender(this.timeSlicingMode, targetTexture);
		}

		/// <summary>
		///   <para>Checks if a probe has finished a time-sliced render.</para>
		/// </summary>
		/// <param name="renderId">An integer representing the RenderID as returned by the RenderProbe method.</param>
		/// <returns>
		///   <para>
		///     True if the render has finished, false otherwise.
		///
		///     See Also: timeSlicingMode
		///   </para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsFinishedRendering(int renderId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int ScheduleRender(ReflectionProbeTimeSlicingMode timeSlicingMode, RenderTexture targetTexture);

		/// <summary>
		///   <para>Utility method to blend 2 cubemaps into a target render texture.</para>
		/// </summary>
		/// <param name="src">Cubemap to blend from.</param>
		/// <param name="dst">Cubemap to blend to.</param>
		/// <param name="blend">Blend weight.</param>
		/// <param name="target">RenderTexture which will hold the result of the blend.</param>
		/// <returns>
		///   <para>Returns trues if cubemaps were blended, false otherwise.</para>
		/// </returns>
		[NativeHeader("Runtime/Camera/CubemapGPUUtility.h")]
		[FreeFunction("CubemapGPUBlend")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BlendCubemap(Texture src, Texture dst, float blend, RenderTexture target);

		[StaticAccessor("GetReflectionProbes()")]
		public static extern int minBakedCubemapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetReflectionProbes()")]
		public static extern int maxBakedCubemapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>HDR decode values of the default reflection probe texture.</para>
		/// </summary>
		[StaticAccessor("GetReflectionProbes()")]
		public static Vector4 defaultTextureHDRDecodeValues
		{
			get
			{
				Vector4 vector;
				ReflectionProbe.get_defaultTextureHDRDecodeValues_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Texture which is used outside of all reflection probes (Read Only).</para>
		/// </summary>
		[StaticAccessor("GetReflectionProbes()")]
		public static extern Texture defaultTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ReflectionProbe, ReflectionProbe.ReflectionProbeEvent> reflectionProbeChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Cubemap> defaultReflectionSet;

		[RequiredByNativeCode]
		private static void CallReflectionProbeEvent(ReflectionProbe probe, ReflectionProbe.ReflectionProbeEvent probeEvent)
		{
			Action<ReflectionProbe, ReflectionProbe.ReflectionProbeEvent> action = ReflectionProbe.reflectionProbeChanged;
			if (action != null)
			{
				action(probe, probeEvent);
			}
		}

		[RequiredByNativeCode]
		private static void CallSetDefaultReflection(Cubemap defaultReflectionCubemap)
		{
			Action<Cubemap> action = ReflectionProbe.defaultReflectionSet;
			if (action != null)
			{
				action(defaultReflectionCubemap);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_size_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_size_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_center_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_center_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_backgroundColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_backgroundColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_textureHDRDecodeValues_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_defaultTextureHDRDecodeValues_Injected(out Vector4 ret);

		/// <summary>
		///   <para>Types of events that occur when ReflectionProbe components are used in a scene.</para>
		/// </summary>
		public enum ReflectionProbeEvent
		{
			/// <summary>
			///   <para>An event that occurs when a Reflection Probe component is added to a scene or enabled in a scene.</para>
			/// </summary>
			ReflectionProbeAdded,
			/// <summary>
			///   <para>An event that occurs when a Reflection Probe component is unloaded from a scene or disabled in a scene.</para>
			/// </summary>
			ReflectionProbeRemoved
		}
	}
}
