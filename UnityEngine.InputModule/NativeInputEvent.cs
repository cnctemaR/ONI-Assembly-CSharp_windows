using System;
using System.Runtime.InteropServices;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct NativeInputEvent
	{
		public NativeInputEvent(NativeInputEventType type, int sizeInBytes, int deviceId, double time)
		{
			this.type = type;
			this.sizeInBytes = (ushort)sizeInBytes;
			this.deviceId = (ushort)deviceId;
			this.eventId = 0;
			this.time = time;
		}

		[FieldOffset(0)]
		public NativeInputEventType type;

		[FieldOffset(4)]
		public ushort sizeInBytes;

		[FieldOffset(6)]
		public ushort deviceId;

		[FieldOffset(8)]
		public int eventId;

		[FieldOffset(12)]
		public double time;
	}
}
