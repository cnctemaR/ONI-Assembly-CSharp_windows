using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PageQueryInternal : IDisposable
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

		public int StartIndex
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_StartIndex, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_StartIndex, value);
			}
		}

		public int MaxCount
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_MaxCount, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_MaxCount, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private int m_StartIndex;

		private int m_MaxCount;
	}
}
