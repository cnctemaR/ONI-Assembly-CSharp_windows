using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UpdateLobbyOptionsInternal : IDisposable
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

		public LobbyModification LobbyModificationHandle
		{
			get
			{
				LobbyModification @default = Helper.GetDefault<LobbyModification>();
				Helper.TryMarshalGet<LobbyModification>(this.m_LobbyModificationHandle, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LobbyModificationHandle, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private IntPtr m_LobbyModificationHandle;
	}
}
