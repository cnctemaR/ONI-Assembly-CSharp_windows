using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP_STATE
	{
		public DSP_STATE_FUNCTIONS functions
		{
			get
			{
				return Marshal.PtrToStructure<DSP_STATE_FUNCTIONS>(this.functions_internal);
			}
		}

		public IntPtr instance;

		public IntPtr plugindata;

		public uint channelmask;

		public int source_speakermode;

		public IntPtr sidechaindata;

		public int sidechainchannels;

		private IntPtr functions_internal;

		public int systemobject;
	}
}
