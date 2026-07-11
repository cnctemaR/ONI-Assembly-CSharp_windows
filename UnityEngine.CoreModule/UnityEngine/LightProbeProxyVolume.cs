using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Light Probe Proxy Volume component offers the possibility to use higher resolution lighting for large non-static GameObjects.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/LightProbeProxyVolume.h")]
	public sealed class LightProbeProxyVolume : Behaviour
	{
		/// <summary>
		///   <para>Checks if Light Probe Proxy Volumes are supported.</para>
		/// </summary>
		public static extern bool isFeatureSupported
		{
			[NativeName("IsFeatureSupported")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The world-space bounding box in which the 3D grid of interpolated Light Probes is generated.</para>
		/// </summary>
		[NativeName("GlobalAABB")]
		public Bounds boundsGlobal
		{
			get
			{
				Bounds bounds;
				this.get_boundsGlobal_Injected(out bounds);
				return bounds;
			}
		}

		/// <summary>
		///   <para>The size of the bounding box in which the 3D grid of interpolated Light Probes is generated.</para>
		/// </summary>
		[NativeName("BoundingBoxSizeCustom")]
		public Vector3 sizeCustom
		{
			get
			{
				Vector3 vector;
				this.get_sizeCustom_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_sizeCustom_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The local-space origin of the bounding box in which the 3D grid of interpolated Light Probes is generated.</para>
		/// </summary>
		[NativeName("BoundingBoxOriginCustom")]
		public Vector3 originCustom
		{
			get
			{
				Vector3 vector;
				this.get_originCustom_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_originCustom_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Interpolated Light Probe density.</para>
		/// </summary>
		public extern float probeDensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The 3D grid resolution on the z-axis.</para>
		/// </summary>
		public extern int gridResolutionX
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The 3D grid resolution on the y-axis.</para>
		/// </summary>
		public extern int gridResolutionY
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The 3D grid resolution on the z-axis.</para>
		/// </summary>
		public extern int gridResolutionZ
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bounding box mode for generating the 3D grid of interpolated Light Probes.</para>
		/// </summary>
		public extern LightProbeProxyVolume.BoundingBoxMode boundingBoxMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The resolution mode for generating the grid of interpolated Light Probes.</para>
		/// </summary>
		public extern LightProbeProxyVolume.ResolutionMode resolutionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mode in which the interpolated Light Probe positions are generated.</para>
		/// </summary>
		public extern LightProbeProxyVolume.ProbePositionMode probePositionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the way the Light Probe Proxy Volume refreshes.</para>
		/// </summary>
		public extern LightProbeProxyVolume.RefreshMode refreshMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Determines how many Spherical Harmonics bands will be evaluated to compute the ambient color.</para>
		/// </summary>
		public extern LightProbeProxyVolume.QualityMode qualityMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Triggers an update of the Light Probe Proxy Volume.</para>
		/// </summary>
		public void Update()
		{
			this.SetDirtyFlag(true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetDirtyFlag(bool flag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_boundsGlobal_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_sizeCustom_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_sizeCustom_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_originCustom_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_originCustom_Injected(ref Vector3 value);

		/// <summary>
		///   <para>The resolution mode for generating a grid of interpolated Light Probes.</para>
		/// </summary>
		public enum ResolutionMode
		{
			/// <summary>
			///   <para>The automatic mode uses a number of interpolated Light Probes per unit area, and uses the bounding volume size to compute the resolution. The final resolution value is a power of 2.</para>
			/// </summary>
			Automatic,
			/// <summary>
			///   <para>The custom mode allows you to specify the 3D grid resolution.</para>
			/// </summary>
			Custom
		}

		/// <summary>
		///   <para>The bounding box mode for generating a grid of interpolated Light Probes.</para>
		/// </summary>
		public enum BoundingBoxMode
		{
			/// <summary>
			///   <para>The bounding box encloses the current Renderer and all the relevant Renderers down the hierarchy, in local space.</para>
			/// </summary>
			AutomaticLocal,
			/// <summary>
			///   <para>The bounding box encloses the current Renderer and all the relevant Renderers down the hierarchy, in world space.</para>
			/// </summary>
			AutomaticWorld,
			/// <summary>
			///   <para>A custom local-space bounding box is used. The user is able to edit the bounding box.</para>
			/// </summary>
			Custom
		}

		/// <summary>
		///   <para>The mode in which the interpolated Light Probe positions are generated.</para>
		/// </summary>
		public enum ProbePositionMode
		{
			/// <summary>
			///   <para>Divide the volume in cells based on resolution, and generate interpolated Light Probes positions in the corner/edge of the cells.</para>
			/// </summary>
			CellCorner,
			/// <summary>
			///   <para>Divide the volume in cells based on resolution, and generate interpolated Light Probe positions in the center of the cells.</para>
			/// </summary>
			CellCenter
		}

		/// <summary>
		///   <para>An enum describing the way a Light Probe Proxy Volume refreshes in the Player.</para>
		/// </summary>
		public enum RefreshMode
		{
			/// <summary>
			///   <para>Automatically detects updates in Light Probes and triggers an update of the Light Probe volume.</para>
			/// </summary>
			Automatic,
			/// <summary>
			///   <para>Causes Unity to update the Light Probe Proxy Volume every frame.</para>
			/// </summary>
			EveryFrame,
			/// <summary>
			///   <para>Use this option to indicate that the Light Probe Proxy Volume is never to be automatically updated by Unity.</para>
			/// </summary>
			ViaScripting
		}

		/// <summary>
		///   <para>An enum describing the Quality option used by the Light Probe Proxy Volume component.</para>
		/// </summary>
		public enum QualityMode
		{
			/// <summary>
			///   <para>This option will use only two SH coefficients bands: L0 and L1. The coefficients are sampled from the Light Probe Proxy Volume 3D Texture. Using this option might increase the draw call batch sizes by not having to change the L2 coefficients per Renderer.</para>
			/// </summary>
			Low,
			/// <summary>
			///   <para>This option will use L0 and L1 SH coefficients from the Light Probe Proxy Volume 3D Texture. The L2 coefficients are constant per Renderer. By having to provide the L2 coefficients, draw call batches might be broken.</para>
			/// </summary>
			Normal
		}
	}
}
