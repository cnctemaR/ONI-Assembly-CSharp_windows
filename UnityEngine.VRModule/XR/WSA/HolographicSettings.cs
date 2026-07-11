using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	/// <summary>
	///   <para>The Holographic Settings contain functions which effect the performance and presentation of Holograms on Windows Holographic platforms.</para>
	/// </summary>
	[NativeHeader("Runtime/VR/HoloLens/HolographicSettings.h")]
	[StaticAccessor("HolographicSettings::GetInstance()", StaticAccessorType.Dot)]
	public class HolographicSettings
	{
		/// <summary>
		///   <para>Sets a point in 3d space that is the focal point of the scene for the user for this frame. This helps improve the visual fidelity of content around this point. This must be set every frame.</para>
		/// </summary>
		/// <param name="position">The position of the focal point in the scene, relative to the camera.</param>
		/// <param name="normal">Surface normal of the plane being viewed at the focal point.</param>
		/// <param name="velocity">A vector that describes how the focus point is moving in the scene at this point in time. This allows the HoloLens to compensate for both your head movement and the movement of the object in the scene.</param>
		public static void SetFocusPointForFrame(Vector3 position)
		{
			HolographicSettings.InternalSetFocusPointForFrameP(position);
		}

		/// <summary>
		///   <para>Sets a point in 3d space that is the focal point of the scene for the user for this frame. This helps improve the visual fidelity of content around this point. This must be set every frame.</para>
		/// </summary>
		/// <param name="position">The position of the focal point in the scene, relative to the camera.</param>
		/// <param name="normal">Surface normal of the plane being viewed at the focal point.</param>
		/// <param name="velocity">A vector that describes how the focus point is moving in the scene at this point in time. This allows the HoloLens to compensate for both your head movement and the movement of the object in the scene.</param>
		public static void SetFocusPointForFrame(Vector3 position, Vector3 normal)
		{
			HolographicSettings.InternalSetFocusPointForFramePN(position, normal);
		}

		/// <summary>
		///   <para>Sets a point in 3d space that is the focal point of the scene for the user for this frame. This helps improve the visual fidelity of content around this point. This must be set every frame.</para>
		/// </summary>
		/// <param name="position">The position of the focal point in the scene, relative to the camera.</param>
		/// <param name="normal">Surface normal of the plane being viewed at the focal point.</param>
		/// <param name="velocity">A vector that describes how the focus point is moving in the scene at this point in time. This allows the HoloLens to compensate for both your head movement and the movement of the object in the scene.</param>
		public static void SetFocusPointForFrame(Vector3 position, Vector3 normal, Vector3 velocity)
		{
			HolographicSettings.InternalSetFocusPointForFramePNV(position, normal, velocity);
		}

		[NativeName("SetFocusPointForFrame")]
		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		private static void InternalSetFocusPointForFrameP(Vector3 position)
		{
			HolographicSettings.InternalSetFocusPointForFrameP_Injected(ref position);
		}

		[NativeName("SetFocusPointForFrame")]
		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		private static void InternalSetFocusPointForFramePN(Vector3 position, Vector3 normal)
		{
			HolographicSettings.InternalSetFocusPointForFramePN_Injected(ref position, ref normal);
		}

		[NativeName("SetFocusPointForFrame")]
		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		private static void InternalSetFocusPointForFramePNV(Vector3 position, Vector3 normal, Vector3 velocity)
		{
			HolographicSettings.InternalSetFocusPointForFramePNV_Injected(ref position, ref normal, ref velocity);
		}

		/// <summary>
		///   <para>This method returns whether or not the display associated with the main camera reports as opaque.</para>
		/// </summary>
		public static bool IsDisplayOpaque
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		///   <para>Whether the app is displaying protected content.</para>
		/// </summary>
		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		public static extern bool IsContentProtectionEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The kind of reprojection the app is requesting to stabilize its holographic rendering relative to the user's head motion.</para>
		/// </summary>
		public static HolographicSettings.HolographicReprojectionMode ReprojectionMode
		{
			get
			{
				return HolographicSettings.HolographicReprojectionMode.Disabled;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>Option to allow developers to achieve higher framerate at the cost of high latency.  By default this option is off.</para>
		/// </summary>
		/// <param name="activated">True to enable or false to disable Low Latent Frame Presentation.</param>
		[Obsolete("Support for toggling latent frame presentation has been removed", true)]
		public static void ActivateLatentFramePresentation(bool activated)
		{
		}

		/// <summary>
		///   <para>Returns true if Holographic rendering is currently running with Latent Frame Presentation.  Default value is false.</para>
		/// </summary>
		[Obsolete("Support for toggling latent frame presentation has been removed, and IsLatentFramePresentation will always return true", false)]
		public static bool IsLatentFramePresentation
		{
			get
			{
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFocusPointForFrameP_Injected(ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFocusPointForFramePN_Injected(ref Vector3 position, ref Vector3 normal);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFocusPointForFramePNV_Injected(ref Vector3 position, ref Vector3 normal, ref Vector3 velocity);

		/// <summary>
		///   <para>Represents the kind of reprojection an app is requesting to stabilize its holographic rendering relative to the user's head motion.</para>
		/// </summary>
		public enum HolographicReprojectionMode
		{
			/// <summary>
			///   <para>The image should be stabilized for changes to both the user's head position and orientation. This is best for world-locked content that should remain physically stationary as the user walks around.</para>
			/// </summary>
			PositionAndOrientation,
			/// <summary>
			///   <para>The image should be stabilized only for changes to the user's head orientation, ignoring positional changes. This is best for body-locked content that should tag along with the user as they walk around, such as 360-degree video.</para>
			/// </summary>
			OrientationOnly,
			/// <summary>
			///   <para>The image should not be stabilized for the user's head motion, instead remaining fixed in the display. This is generally discouraged, as it is only comfortable for users when used sparingly, such as when the only visible content is a small cursor.</para>
			/// </summary>
			Disabled
		}
	}
}
