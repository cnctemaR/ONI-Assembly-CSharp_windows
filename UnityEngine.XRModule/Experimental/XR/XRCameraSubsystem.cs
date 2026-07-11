using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Provides access to a device's camera.</para>
	/// </summary>
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Camera/XRCameraSubsystem.h")]
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	public class XRCameraSubsystem : Subsystem<XRCameraSubsystemDescriptor>
	{
		/// <summary>
		///   <para>The frame during which the camera subsystem was last successfully updated.</para>
		/// </summary>
		public extern int LastUpdatedFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>True if the XRCameraSubsystem should try to provide light estimation.</para>
		/// </summary>
		public extern bool LightEstimationRequested
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set current Material to be used while rendering to the render target.</para>
		/// </summary>
		public extern Material Material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set current Camera component within the app to be used by this XRCameraInstance.</para>
		/// </summary>
		public extern Camera Camera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool TryGetAverageBrightness(ref float averageBrightness);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool TryGetAverageColorTemperature(ref float averageColorTemperature);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool TryGetProjectionMatrix(ref Matrix4x4 projectionMatrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool TryGetDisplayMatrix(ref Matrix4x4 displayMatrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool TryGetTimestamp(ref long timestampNs);

		public bool TryGetShaderName(ref string shaderName)
		{
			return this.Internal_TryGetShaderName(ref shaderName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_TryGetShaderName(ref string shaderName);

		public void GetTextures(List<Texture2D> texturesOut)
		{
			if (texturesOut == null)
			{
				throw new ArgumentNullException("texturesOut");
			}
			this.GetTexturesAsList(texturesOut);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTexturesAsList(List<Texture2D> textures);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D[] GetTexturesAsFixedArray();

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<FrameReceivedEventArgs> FrameReceived;

		[RequiredByNativeCode]
		private void InvokeFrameReceivedEvent()
		{
			if (this.FrameReceived != null)
			{
				this.FrameReceived(new FrameReceivedEventArgs
				{
					m_CameraSubsystem = this
				});
			}
		}
	}
}
