using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputTrackingFacade.h")]
	[NativeConditional("ENABLE_VR")]
	[UsedByNativeCode]
	public struct InputDevice : IEquatable<InputDevice>
	{
		internal InputDevice(ulong deviceId)
		{
			this.m_DeviceId = deviceId;
		}

		public bool IsValid
		{
			get
			{
				return InputTracking.IsDeviceValid(this.m_DeviceId);
			}
		}

		public bool SendHapticImpulse(uint channel, float amplitude, float duration = 1f)
		{
			return InputTracking.SendHapticImpulse(this.m_DeviceId, channel, amplitude, duration);
		}

		public bool SendHapticBuffer(uint channel, byte[] buffer)
		{
			return InputTracking.SendHapticBuffer(this.m_DeviceId, channel, buffer);
		}

		public bool TryGetHapticCapabilities(out HapticCapabilities capabilities)
		{
			return InputTracking.TryGetHapticCapabilities(this.m_DeviceId, out capabilities);
		}

		public void StopHaptics()
		{
			InputTracking.StopHaptics(this.m_DeviceId);
		}

		public override bool Equals(object obj)
		{
			return obj is InputDevice && this.Equals((InputDevice)obj);
		}

		public bool Equals(InputDevice other)
		{
			return this.m_DeviceId == other.m_DeviceId;
		}

		public override int GetHashCode()
		{
			return this.m_DeviceId.GetHashCode();
		}

		public static bool operator ==(InputDevice a, InputDevice b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(InputDevice a, InputDevice b)
		{
			return !(a == b);
		}

		private ulong m_DeviceId;
	}
}
