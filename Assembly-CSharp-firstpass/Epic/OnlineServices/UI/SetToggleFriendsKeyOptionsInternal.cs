using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetToggleFriendsKeyOptionsInternal : IDisposable
	{
		public int ApiVersion
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ApiVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ApiVersion, value);
			}
		}

		public KeyCombination KeyCombination
		{
			get
			{
				KeyCombination @default = Helper.GetDefault<KeyCombination>();
				Helper.TryMarshalGet<KeyCombination>(this.m_KeyCombination, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<KeyCombination>(ref this.m_KeyCombination, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private KeyCombination m_KeyCombination;
	}
}
