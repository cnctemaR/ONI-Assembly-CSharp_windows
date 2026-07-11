using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A StreamingController controls the streaming settings for an individual camera location.</para>
	/// </summary>
	[NativeHeader("Modules/Streaming/StreamingController.h")]
	[RequireComponent(typeof(Camera))]
	public class StreamingController : Behaviour
	{
		/// <summary>
		///   <para>Offset applied to the mipmap level chosen by the texture streaming system for any textures visible from this camera. This Offset can take either a positive or negative value.</para>
		/// </summary>
		public extern float streamingMipmapBias
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Initiate preloading of streaming data for this camera.</para>
		/// </summary>
		/// <param name="timeoutSeconds">Optional timeout before stopping preloading. Set to 0.0f when no timeout is required.</param>
		/// <param name="activateCameraOnTimeout">Set to True to activate the connected Camera component when timeout expires.</param>
		/// <param name="disableCameraCuttingFrom">Camera to deactivate on timeout (if Camera.activateCameraOnTime is True). This parameter can be null.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPreloading(float timeoutSeconds = 0f, bool activateCameraOnTimeout = false, Camera disableCameraCuttingFrom = null);

		/// <summary>
		///   <para>Abort preloading.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CancelPreloading();

		/// <summary>
		///   <para>Used to find out whether the StreamingController is currently preloading texture mipmaps.</para>
		/// </summary>
		/// <returns>
		///   <para>True if in a preloading state, otherwise False.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsPreloading();
	}
}
