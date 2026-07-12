using System;

namespace UnityEngine.XR
{
	[Flags]
	public enum InputDeviceCharacteristics : uint
	{
		None = 0U,
		HeadMounted = 1U,
		Camera = 2U,
		HeldInHand = 4U,
		HandTracking = 8U,
		EyeTracking = 16U,
		TrackedDevice = 32U,
		Controller = 64U,
		TrackingReference = 128U,
		Left = 256U,
		Right = 512U,
		Simulated6DOF = 1024U
	}
}
