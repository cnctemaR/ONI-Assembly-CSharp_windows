using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AttributeDataInternal : IDisposable
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

		public string Key
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Key, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Key, value);
			}
		}

		public AttributeDataValueInternal Value
		{
			get
			{
				AttributeDataValueInternal @default = Helper.GetDefault<AttributeDataValueInternal>();
				Helper.TryMarshalGet<AttributeDataValueInternal>(this.m_Value, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<AttributeDataValueInternal>(ref this.m_Value, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose<AttributeDataValueInternal>(ref this.m_Value);
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Key;

		private AttributeDataValueInternal m_Value;
	}
}
