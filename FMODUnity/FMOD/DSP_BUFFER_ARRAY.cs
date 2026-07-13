using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP_BUFFER_ARRAY
	{
		public int numchannels
		{
			get
			{
				if (this.buffernumchannels != IntPtr.Zero && this.numbuffers != 0)
				{
					return Marshal.ReadInt32(this.buffernumchannels);
				}
				return 0;
			}
			set
			{
				if (this.buffernumchannels != IntPtr.Zero && this.numbuffers != 0)
				{
					Marshal.WriteInt32(this.buffernumchannels, value);
				}
			}
		}

		public IntPtr buffer
		{
			get
			{
				if (this.buffers != IntPtr.Zero && this.numbuffers != 0)
				{
					return Marshal.ReadIntPtr(this.buffers);
				}
				return IntPtr.Zero;
			}
			set
			{
				if (this.buffers != IntPtr.Zero && this.numbuffers != 0)
				{
					Marshal.WriteIntPtr(this.buffers, value);
				}
			}
		}

		public int numbuffers;

		public IntPtr buffernumchannels;

		public IntPtr bufferchannelmask;

		public IntPtr buffers;

		public SPEAKERMODE speakermode;
	}
}
