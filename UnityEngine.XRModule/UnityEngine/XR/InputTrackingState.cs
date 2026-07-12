using System;

namespace UnityEngine.XR
{
	[Flags]
	public enum InputTrackingState : uint
	{
		None = 0U,
		Position = 1U,
		Rotation = 2U,
		Velocity = 4U,
		AngularVelocity = 8U,
		Acceleration = 16U,
		AngularAcceleration = 32U,
		All = 63U
	}
}
