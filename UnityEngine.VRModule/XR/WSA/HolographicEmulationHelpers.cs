using System;

namespace UnityEngine.XR.WSA
{
	internal static class HolographicEmulationHelpers
	{
		public static Vector3 CalcExpectedCameraPosition(SimulatedHead head, SimulatedBody body)
		{
			Vector3 vector = body.position;
			vector.y += body.height - 1.776f;
			vector.y -= head.diameter / 2f;
			vector.y += 0.11599995f;
			Vector3 eulerAngles = head.eulerAngles;
			eulerAngles.y += body.rotation;
			Quaternion quaternion = Quaternion.Euler(eulerAngles);
			vector += quaternion * (0.0985f * Vector3.forward);
			return vector;
		}

		public const float k_DefaultBodyHeight = 1.776f;

		public const float k_DefaultHeadDiameter = 0.2319999f;

		public const float k_ForwardOffset = 0.0985f;
	}
}
