using System;
using UnityEngine.Bindings;

namespace UnityEngine.XR
{
	[NativeConditional("ENABLE_VR")]
	public struct HapticCapabilities : IEquatable<HapticCapabilities>
	{
		public uint numChannels
		{
			get
			{
				return this.m_NumChannels;
			}
			internal set
			{
				this.m_NumChannels = value;
			}
		}

		public bool supportsImpulse
		{
			get
			{
				return this.m_SupportsImpulse;
			}
			internal set
			{
				this.m_SupportsImpulse = value;
			}
		}

		public bool supportsBuffer
		{
			get
			{
				return this.m_SupportsBuffer;
			}
			internal set
			{
				this.m_SupportsBuffer = value;
			}
		}

		public uint bufferFrequencyHz
		{
			get
			{
				return this.m_BufferFrequencyHz;
			}
			internal set
			{
				this.m_BufferFrequencyHz = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is HapticCapabilities && this.Equals((HapticCapabilities)obj);
		}

		public bool Equals(HapticCapabilities other)
		{
			return this.numChannels == other.numChannels && this.supportsImpulse == other.supportsImpulse && this.supportsBuffer == other.supportsBuffer && this.bufferFrequencyHz == other.bufferFrequencyHz;
		}

		public override int GetHashCode()
		{
			return this.numChannels.GetHashCode() ^ (this.supportsImpulse.GetHashCode() << 1) ^ (this.supportsBuffer.GetHashCode() >> 1) ^ (this.bufferFrequencyHz.GetHashCode() << 2);
		}

		public static bool operator ==(HapticCapabilities a, HapticCapabilities b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HapticCapabilities a, HapticCapabilities b)
		{
			return !(a == b);
		}

		private uint m_NumChannels;

		private bool m_SupportsImpulse;

		private bool m_SupportsBuffer;

		private uint m_BufferFrequencyHz;
	}
}
