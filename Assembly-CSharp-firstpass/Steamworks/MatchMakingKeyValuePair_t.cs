using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public struct MatchMakingKeyValuePair_t
	{
		private MatchMakingKeyValuePair_t(string strKey, string strValue)
		{
			this.m_szKey_ = null;
			this.m_szValue_ = null;
			this.m_szKey = strKey;
			this.m_szValue = strValue;
		}

		public string m_szKey
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_szKey_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_szKey_, 256);
			}
		}

		public string m_szValue
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_szValue_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_szValue_, 256);
			}
		}

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_szKey_;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_szValue_;
	}
}
