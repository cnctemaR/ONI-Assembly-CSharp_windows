using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyModificationAddAttributeOptionsInternal : IDisposable
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

		public AttributeDataInternal? Attribute
		{
			get
			{
				AttributeDataInternal? @default = Helper.GetDefault<AttributeDataInternal?>();
				Helper.TryMarshalGet<AttributeDataInternal>(this.m_Attribute, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<AttributeDataInternal>(ref this.m_Attribute, value);
			}
		}

		public LobbyAttributeVisibility Visibility
		{
			get
			{
				LobbyAttributeVisibility @default = Helper.GetDefault<LobbyAttributeVisibility>();
				Helper.TryMarshalGet<LobbyAttributeVisibility>(this.m_Visibility, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<LobbyAttributeVisibility>(ref this.m_Visibility, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Attribute);
		}

		private int m_ApiVersion;

		private IntPtr m_Attribute;

		private LobbyAttributeVisibility m_Visibility;
	}
}
