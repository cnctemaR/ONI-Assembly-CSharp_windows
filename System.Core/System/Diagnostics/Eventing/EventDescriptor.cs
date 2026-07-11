using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct EventDescriptor
	{
		public EventDescriptor(int id, byte version, byte channel, byte level, byte opcode, int task, long keywords)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public byte Channel
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public int EventId
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public long Keywords
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		public byte Level
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public byte Opcode
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public int Task
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public byte Version
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}
	}
}
